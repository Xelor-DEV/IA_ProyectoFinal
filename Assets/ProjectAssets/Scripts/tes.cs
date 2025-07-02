using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float verticalSpeed = 5f;

    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    private Vector2 movementInput;
    private float verticalInput;

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * movementInput.y + right * movementInput.x) * moveSpeed * Time.deltaTime;
        Vector3 verticalMove = Vector3.up * verticalInput * verticalSpeed * Time.deltaTime;

        transform.Translate(moveDirection + verticalMove, Space.World);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnVerticalInput(InputAction.CallbackContext context)
    {
        verticalInput = context.ReadValue<float>();
    }
}