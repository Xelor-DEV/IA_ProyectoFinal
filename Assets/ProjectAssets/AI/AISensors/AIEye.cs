using UnityEngine;
using System.Collections;

[System.Serializable]
public class DataViewBase
{
    #region Vision Configuration
    [Header("Vision Range Settings")]
    [SerializeField, Range(0, 180)] private float visionAngle = 30f;
    [SerializeField] private float visionHeight = 1.0f;
    [SerializeField] private float maxDetectionDistance = 10f;
    [SerializeField] private LayerMask scanLayers;

    [Header("Visualization Settings")]
    [SerializeField] protected Color outOfSightColor = Color.red;
    [SerializeField] protected bool drawGizmos = false;

    [Header("Dependencies")]
    [SerializeField] private HealthManager owner;
    #endregion

    #region Properties
    public float VisionAngle => visionAngle;
    public float VisionHeight => visionHeight;
    public float MaxDistance => maxDetectionDistance;
    public LayerMask ScanLayers => scanLayers;
    public HealthManager Owner => owner;
    public Mesh VisionMesh { get; protected set; }
    #endregion

    #region Core Functionality
    public virtual void Initialize()
    {
        VisionMesh = CreateWedgeMesh();
    }

    public virtual bool IsInSight(Transform target)
    {
        return false;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 10;
        int numTriangles = (segments * 4) + 4;
        int numVertices = numTriangles * 3;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -visionAngle, 0) * Vector3.forward * maxDetectionDistance;
        Vector3 bottomRight = Quaternion.Euler(0, visionAngle, 0) * Vector3.forward * maxDetectionDistance;

        Vector3 topCenter = bottomCenter + Vector3.up * visionHeight;
        Vector3 topLeft = bottomLeft + Vector3.up * visionHeight;
        Vector3 topRight = bottomRight + Vector3.up * visionHeight;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -visionAngle;
        float deltaAngle = (visionAngle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * maxDetectionDistance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * maxDetectionDistance;

            topRight = bottomRight + Vector3.up * visionHeight;
            topLeft = bottomLeft + Vector3.up * visionHeight;

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
            // top 
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
            // bottom 
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;

        }


        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;

    }
    #endregion

    #region Gizmos
    public virtual void DrawVisionGizmos()
    {
        if (!drawGizmos || VisionMesh == null || Owner == null) return;

        Gizmos.color = outOfSightColor;
        Gizmos.DrawMesh(VisionMesh, Owner.transform.position, Owner.transform.rotation);
    }
    #endregion
}

[System.Serializable]
public class DataView : DataViewBase
{
    #region Occlusion Settings
    [Header("Occlusion Settings")]
    [SerializeField] private LayerMask _occlusionLayers;
    [SerializeField] private bool _checkInsideObjects = true;
    [SerializeField] private Color _inSightColor = Color.green;
    #endregion

    #region Vision State
    public bool TargetInSight { get; private set; }
    #endregion

    #region Detection Logic
    public override bool IsInSight(Transform target)
    {
        TargetInSight = false;
        if (target == null || Owner == null) return false;

        Vector3 origin = Owner.AimOffset.position;
        Vector3 targetPosition = target.position;
        Vector3 direction = targetPosition - origin;

        if (!ValidateDistance(direction) || !ValidateHeight(targetPosition) || !ValidateAngle(direction))
            return TargetInSight;

        if (CheckOcclusion(origin, targetPosition)) return TargetInSight;

        TargetInSight = true;
        return TargetInSight;
    }

    private bool ValidateDistance(Vector3 direction) => direction.magnitude <= MaxDistance;
    private bool ValidateHeight(Vector3 targetPos) =>
        Mathf.Abs(targetPos.y - Owner.transform.position.y) <= VisionHeight;

    private bool ValidateAngle(Vector3 direction)
    {
        float horizontalAngle = Vector3.Angle(direction.normalized, Owner.transform.forward);
        return horizontalAngle <= VisionAngle;
    }

    private bool CheckOcclusion(Vector3 origin, Vector3 target)
    {
        return Physics.Linecast(origin, target, _occlusionLayers) && _checkInsideObjects;
    }
    #endregion

    #region Gizmos
    public override void DrawVisionGizmos()
    {
        if (!drawGizmos || VisionMesh == null || Owner == null) return;

        Gizmos.color = TargetInSight ? _inSightColor : outOfSightColor;
        Gizmos.DrawMesh(VisionMesh, Owner.transform.position, Owner.transform.rotation);
    }
    #endregion
}

public class AIEye : MonoBehaviour
{
    #region Scan Settings
    [Header("Scan Configuration")]
    [SerializeField] private DataView _mainVision = new DataView();
    [SerializeField, Min(0.1f)] private float _minScanInterval = 1f;
    [SerializeField, Min(0.1f)] private float _maxScanInterval = 1f;
    #endregion

    #region Scan State
    [Header("Scan Results")]
    [SerializeField] private HealthManager _detectedEnemy;
    [SerializeField] private HealthManager _detectedAlly;
    [SerializeField] private Vector3 _currentTarget;
    [SerializeField] private int _enemiesInView;
    #endregion

    #region Dependencies
    [Header("Component References")]
    [SerializeField] private HealthManager linkedHealth;
    [SerializeField] private Transform _aimOffset;
    #endregion

    #region Runtime Variables
    private float[] _scanIntervals;
    private int _currentIntervalIndex;
    private float _scanTimer;
    #endregion

    #region Properties
    public HealthManager DetectedEnemy => _detectedEnemy;
    public HealthManager DetectedAlly => _detectedAlly;
    public Vector3 CurrentTarget => _currentTarget;

    public float EnemyDistance =>
        _detectedEnemy ? Vector3.Distance(transform.position, _detectedEnemy.transform.position) : -1f;

    public Vector3 EnemyDirection =>
        _detectedEnemy ? (_detectedEnemy.transform.position - transform.position).normalized : Vector3.zero;
    #endregion

    #region Initialization
    protected virtual void Awake()
    {
        InitializeVision();
        SetupScanIntervals();
    }

    private void OnValidate()
    {
        if (_mainVision != null)
        {
            _mainVision.Initialize();
        }
    }

    private void InitializeVision()
    {
        _mainVision.Initialize();
    }

    private void SetupScanIntervals()
    {
        _scanIntervals = new float[10];
        for (int i = 0; i < _scanIntervals.Length; i++)
        {
            _scanIntervals[i] = Random.Range(_minScanInterval, _maxScanInterval);
        }
    }
    #endregion

    #region Scan Logic
    protected virtual void Update()
    {
        UpdateScanTimer();
        HandleEnemyState();
    }

    private void UpdateScanTimer()
    {
        _scanTimer += Time.deltaTime;
        if (_scanTimer >= _scanIntervals[_currentIntervalIndex])
        {
            PerformScan();
            AdvanceScanInterval();
            ResetTimer();
        }
    }

    private void AdvanceScanInterval()
    {
        _currentIntervalIndex = (_currentIntervalIndex + 1) % _scanIntervals.Length;
    }

    private void ResetTimer() => _scanTimer = 0f;

    protected virtual void PerformScan()
    {
        if (linkedHealth.IsDead) return;

        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            _mainVision.MaxDistance,
            _mainVision.ScanLayers
        );

        ProcessDetectedObjects(hitColliders);
    }

    private void ProcessDetectedObjects(Collider[] colliders)
    {
        _enemiesInView = 0;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (IsSelf(col.gameObject)) continue;

            HealthManager targetHealth = col.GetComponent<HealthManager>();
            if (IsValidTarget(targetHealth))
            {
                EvaluateTarget(targetHealth, ref closestDistance);
            }
        }
    }

    private bool IsSelf(GameObject obj) => obj.GetInstanceID() == gameObject.GetInstanceID();

    private bool IsValidTarget(HealthManager targetHealth)
    {
        return targetHealth != null &&
               targetHealth.gameObject.activeSelf &&
               !targetHealth.IsDead &&
               targetHealth.IsVisible &&
               _mainVision.IsInSight(targetHealth.AimOffset);
    }

    private void EvaluateTarget(HealthManager target, ref float closestDistance)
    {
        if (IsAlly(target))
        {
            _detectedAlly = target;
            return;
        }

        _enemiesInView++;
        float currentDistance = Vector3.Distance(transform.position, target.transform.position);
        if (currentDistance < closestDistance)
        {
            closestDistance = currentDistance;
            _detectedEnemy = target;
        }
    }

    protected virtual bool IsAlly(HealthManager target)
    {
        return linkedHealth.AlliedFactions.Contains(target.Faction);
    }

    private void HandleEnemyState()
    {
        if (_detectedEnemy && (_detectedEnemy.IsDead || !_detectedEnemy.IsVisible))
        {
            _detectedEnemy = null;
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        _mainVision.DrawVisionGizmos();
    }
    #endregion
}