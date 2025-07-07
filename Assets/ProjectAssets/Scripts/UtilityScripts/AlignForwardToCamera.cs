using UnityEngine;

public class AlignForwardToCamera : MonoBehaviour
{
    [Tooltip("The object that will align its forward direction to the camera")]
    [SerializeField] private GameObject lookerObject;

    [Tooltip("The camera whose forward direction will be followed")]
    [SerializeField] private GameObject cameraObject;

    void Update()
    {
        if (lookerObject != null && cameraObject != null)
        {
            lookerObject.transform.forward = cameraObject.transform.forward;
        }
    }
}
