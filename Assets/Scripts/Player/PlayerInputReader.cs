using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkingSimulation.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string playerActionMap = "Player";
        [SerializeField] private string moveActionName = "Move";
        [SerializeField] private string lookActionName = "Look";
        [SerializeField] private string jumpActionName = "Jump";
        [SerializeField] private string sprintActionName = "Sprint";
        [SerializeField] private string interactActionName = "Interact";
        [SerializeField] private string togglePerspectiveActionName = "TogglePerspective";

        private InputActionMap playerMap;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private InputAction interactAction;
        private InputAction togglePerspectiveAction;

        public event Action JumpPressed;
        public event Action InteractPressed;
        public event Action PerspectiveTogglePressed;

        public Vector2 Move => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;

        public Vector2 Look => lookAction?.ReadValue<Vector2>() ?? Vector2.zero;

        public bool IsSprinting => sprintAction?.IsPressed() ?? false;

        public InputActionAsset InputActions => inputActions;

        private void Awake()
        {
            BindActions();
        }

        private void OnEnable()
        {
            BindActions();

            if (jumpAction != null)
            {
                jumpAction.performed += OnJumpPerformed;
            }

            if (interactAction != null)
            {
                interactAction.performed += OnInteractPerformed;
            }

            if (togglePerspectiveAction != null)
            {
                togglePerspectiveAction.performed += OnTogglePerspectivePerformed;
            }

            playerMap?.Enable();
        }

        private void OnDisable()
        {
            if (jumpAction != null)
            {
                jumpAction.performed -= OnJumpPerformed;
            }

            if (interactAction != null)
            {
                interactAction.performed -= OnInteractPerformed;
            }

            if (togglePerspectiveAction != null)
            {
                togglePerspectiveAction.performed -= OnTogglePerspectivePerformed;
            }

            playerMap?.Disable();
        }

        private void BindActions()
        {
            if (inputActions == null || playerMap != null)
            {
                return;
            }

            playerMap = inputActions.FindActionMap(playerActionMap, true);
            moveAction = playerMap.FindAction(moveActionName, true);
            lookAction = playerMap.FindAction(lookActionName, true);
            jumpAction = playerMap.FindAction(jumpActionName, true);
            sprintAction = playerMap.FindAction(sprintActionName, true);
            interactAction = playerMap.FindAction(interactActionName, true);
            togglePerspectiveAction = playerMap.FindAction(togglePerspectiveActionName, true);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpPressed?.Invoke();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            InteractPressed?.Invoke();
        }

        private void OnTogglePerspectivePerformed(InputAction.CallbackContext context)
        {
            PerspectiveTogglePressed?.Invoke();
        }
    }
}
