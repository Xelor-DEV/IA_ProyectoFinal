using UnityEngine;

public enum BufferType 
{ 
    Int, 
    Float 
}

public class RandomBuffer : MonoBehaviour
{
    [Header("Buffer Configuration")]
    [Tooltip("Tamaño del buffer (cantidad de valores aleatorios)")]
    [SerializeField] private int bufferSize = 100;

    [Tooltip("Tipo de valores a generar")]
    [SerializeField] private BufferType bufferType = BufferType.Float;

    [Tooltip("Valor mínimo del rango aleatorio")]
    [SerializeField] private float minValue = 0f;

    [Tooltip("Valor máximo del rango aleatorio")]
    [SerializeField] private float maxValue = 1f;

    [Header("Debug Options")]
    [Tooltip("Mostrar valor actual en el Inspector?")]
    [SerializeField] private bool showCurrentValue = false;

    [Tooltip("Valor actual del buffer (solo lectura)")]
    [SerializeField] private float currentValueDisplay;

    private int[] intBuffer;
    private float[] floatBuffer;

    private int currentIndex = 0;

    void Start()
    {
        InitializeBuffer();
    }

    void InitializeBuffer()
    {
        currentIndex = 0;

        switch (bufferType)
        {
            case BufferType.Int:
                IntializeIntBuffer();
                break;
            case BufferType.Float:
                InitializeFloatBuffer();
                break;
            default:
                InitializeFloatBuffer();
                break;
        }
    }

    private void IntializeIntBuffer()
    {
        // Crear e inicializar buffer de enteros
        intBuffer = new int[bufferSize];
        int minInt = Mathf.FloorToInt(minValue);
        int maxInt = Mathf.FloorToInt(maxValue);

        for (int i = 0; i < bufferSize; ++i)
        {
            intBuffer[i] = Random.Range(minInt, maxInt + 1);
        }
    }

    private void InitializeFloatBuffer()
    {
        // Crear e inicializar buffer de floats
        floatBuffer = new float[bufferSize];

        for (int i = 0; i < bufferSize; ++i)
        {
            floatBuffer[i] = Random.Range(minValue, maxValue);
        }
    }

    // Obtiene el siguiente valor del buffer
    public float GetNext()
    {
        if (bufferType == BufferType.Int)
        {
            if (intBuffer == null || intBuffer.Length == 0)
            {
                Debug.LogError("Buffer de enteros no inicializado!");
                return 0;
            }

            int value = intBuffer[currentIndex];
            UpdateCurrentValueDisplay(value);
            currentIndex = (currentIndex + 1) % bufferSize;
            return value;
        }
        else
        {
            if (floatBuffer == null || floatBuffer.Length == 0)
            {
                Debug.LogError("Buffer de floats no inicializado!");
                return 0;
            }

            float value = floatBuffer[currentIndex];
            UpdateCurrentValueDisplay(value);
            currentIndex = (currentIndex + 1) % bufferSize;
            return value;
        }
    }

    void UpdateCurrentValueDisplay(float value)
    {
        if (showCurrentValue)
        {
            currentValueDisplay = value;
        }
    }

    // Método para regenerar el buffer manualmente
    [ContextMenu("Regenerar Buffer")]
    public void RegenerateBuffer()
    {
        InitializeBuffer();
        Debug.Log("Buffer regenerado!");
    }
}