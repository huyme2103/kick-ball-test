using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController : MonoBehaviour
{
    private static readonly int BlendHash =
        Animator.StringToHash("Blend");

    private const float MovementThreshold = 0.01f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 720f;

    private Vector3 movementInput;
    private float verticalVelocity;

[SerializeField]
private float pushForce = 4f;
    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (characterController == null)
        {
            characterController =
                GetComponent<CharacterController>();
        }
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
    }

    private void Update()
    {
        ReadInput();
        HandleMovement();
        HandleRotation();
        HandleAnimation();
    }

    private void ReadInput()
    {
        Vector2 moveValue = moveAction != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        movementInput = new Vector3(
            moveValue.x,
            0f,
            moveValue.y
        );
    }

    private void HandleMovement()
    {
        Vector3 moveDirection =
            movementInput.normalized * moveSpeed;

        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity +=
                gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        characterController.Move(
            moveDirection * Time.deltaTime
        );
    }

    private void HandleRotation()
    {
        if (movementInput.sqrMagnitude <= MovementThreshold)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(
                movementInput,
                Vector3.up
            );

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    private void HandleAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(
            BlendHash,
            movementInput.magnitude,
            0.1f,
            Time.deltaTime
        );
    }

private void OnControllerColliderHit(
    ControllerColliderHit hit)
{
    Rigidbody rigidbody =
        hit.collider.attachedRigidbody;

    if (rigidbody == null)
    {
        return;
    }

    BallController ball =
        rigidbody.GetComponent<BallController>();

    if (ball == null || ball.IsFlying)
    {
        return;
    }

    Vector3 pushDirection =
        new Vector3(
            hit.moveDirection.x,
            0f,
            hit.moveDirection.z
        );

    rigidbody.AddForce(
        pushDirection * pushForce,
        ForceMode.Impulse
    );
}
#if UNITY_EDITOR
    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        gravity = Mathf.Min(0f, gravity);
        rotationSpeed = Mathf.Max(0f, rotationSpeed);

        if (characterController == null)
        {
            characterController =
                GetComponent<CharacterController>();
        }
    }
#endif
}