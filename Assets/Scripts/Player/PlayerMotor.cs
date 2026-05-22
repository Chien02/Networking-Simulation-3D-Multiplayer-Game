using UnityEngine;

namespace NetworkingSimulation.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private PlayerCameraRig cameraRig;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float rotationSpeed = 12f;

        [Header("Jumping And Gravity")]
        [SerializeField] private float jumpHeight = 1.4f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float groundedStickForce = -2f;

        private CharacterController characterController;
        private float verticalVelocity;
        private bool jumpRequested;

        public bool IsGrounded => characterController != null && characterController.isGrounded;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (inputReader == null)
            {
                inputReader = GetComponent<PlayerInputReader>();
            }

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }

            if (cameraRig == null && cameraTransform != null)
            {
                cameraRig = cameraTransform.GetComponent<PlayerCameraRig>();
            }
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed += RequestJump;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed -= RequestJump;
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            Vector3 horizontalVelocity = CalculateHorizontalVelocity();

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedStickForce;
            }

            if (jumpRequested && characterController.isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            jumpRequested = false;
            verticalVelocity += gravity * deltaTime;

            Vector3 motion = horizontalVelocity + Vector3.up * verticalVelocity;
            characterController.Move(motion * deltaTime);

            if (cameraRig != null && cameraRig.CurrentPerspective == PlayerCameraPerspective.FirstPerson)
            {
                AlignToCameraYaw();
            }
            else
            {
                RotateToward(horizontalVelocity, deltaTime);
            }
        }

        private Vector3 CalculateHorizontalVelocity()
        {
            Vector2 moveInput = inputReader != null ? inputReader.Move : Vector2.zero;
            moveInput = Vector2.ClampMagnitude(moveInput, 1f);

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (cameraTransform != null)
            {
                forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            }

            Vector3 direction = forward * moveInput.y + right * moveInput.x;
            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }

            float speed = inputReader != null && inputReader.IsSprinting ? sprintSpeed : walkSpeed;
            return direction * speed;
        }

        private void RotateToward(Vector3 horizontalVelocity, float deltaTime)
        {
            Vector3 direction = Vector3.ProjectOnPlane(horizontalVelocity, Vector3.up);
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * deltaTime);
        }

        private void AlignToCameraYaw()
        {
            transform.rotation = Quaternion.Euler(0f, cameraRig.Yaw, 0f);
        }

        private void RequestJump()
        {
            jumpRequested = true;
        }
    }
}
