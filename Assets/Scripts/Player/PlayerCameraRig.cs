using System;
using UnityEngine;

namespace NetworkingSimulation.Player
{
    public enum PlayerCameraPerspective
    {
        ThirdPerson,
        FirstPerson
    }

    public sealed class PlayerCameraRig : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform thirdPersonTarget;
        [SerializeField] private Transform firstPersonAnchor;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private GameObject playerVisual;

        [Header("Perspective")]
        [SerializeField] private PlayerCameraPerspective startingPerspective = PlayerCameraPerspective.ThirdPerson;

        [Header("Third Person")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float targetHeight = 1.6f;

        [Header("First Person")]
        [SerializeField] private float fallbackEyeHeight = 1.65f;

        [Header("Look")]
        [SerializeField] private float sensitivity = 0.12f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        [Header("Collision")]
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private float minimumDistance = 0.75f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private float yaw;
        private float pitch = 20f;

        public event Action<PlayerCameraPerspective> PerspectiveChanged;

        public PlayerCameraPerspective CurrentPerspective { get; private set; }

        public float Yaw => yaw;

        private void Awake()
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = NormalizePitch(angles.x);

            if (inputReader == null && thirdPersonTarget != null)
            {
                inputReader = thirdPersonTarget.GetComponentInParent<PlayerInputReader>();
            }

            if (playerRoot == null && thirdPersonTarget != null)
            {
                playerRoot = thirdPersonTarget.root;
            }

            SetPerspective(startingPerspective);
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.PerspectiveTogglePressed += TogglePerspective;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.PerspectiveTogglePressed -= TogglePerspective;
            }
        }

        private void LateUpdate()
        {
            if (thirdPersonTarget == null && firstPersonAnchor == null && playerRoot == null)
            {
                return;
            }

            Vector2 lookInput = inputReader != null ? inputReader.Look : Vector2.zero;
            yaw += lookInput.x * sensitivity;
            pitch = Mathf.Clamp(pitch - lookInput.y * sensitivity, minPitch, maxPitch);

            if (CurrentPerspective == PlayerCameraPerspective.FirstPerson)
            {
                ApplyFirstPersonCamera();
                return;
            }

            ApplyThirdPersonCamera();
        }

        public void SetPerspective(PlayerCameraPerspective perspective)
        {
            CurrentPerspective = perspective;
            ApplyVisualState();
            PerspectiveChanged?.Invoke(CurrentPerspective);
        }

        public void TogglePerspective()
        {
            SetPerspective(CurrentPerspective == PlayerCameraPerspective.ThirdPerson
                ? PlayerCameraPerspective.FirstPerson
                : PlayerCameraPerspective.ThirdPerson);
        }

        private void ApplyThirdPersonCamera()
        {
            if (thirdPersonTarget == null)
            {
                return;
            }

            Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 focusPoint = thirdPersonTarget.position + Vector3.up * targetHeight;
            Vector3 desiredPosition = focusPoint - orbitRotation * Vector3.forward * distance;
            Vector3 cameraVector = desiredPosition - focusPoint;
            float resolvedDistance = distance;

            if (Physics.SphereCast(
                    focusPoint,
                    collisionRadius,
                    cameraVector.normalized,
                    out RaycastHit hit,
                    distance,
                    collisionMask,
                    QueryTriggerInteraction.Ignore))
            {
                resolvedDistance = Mathf.Max(minimumDistance, hit.distance - collisionRadius);
            }

            transform.position = focusPoint - orbitRotation * Vector3.forward * resolvedDistance;
            transform.rotation = Quaternion.LookRotation(focusPoint - transform.position, Vector3.up);
        }

        private void ApplyFirstPersonCamera()
        {
            if (playerRoot != null)
            {
                playerRoot.rotation = Quaternion.Euler(0f, yaw, 0f);
            }

            Transform anchor = firstPersonAnchor != null ? firstPersonAnchor : playerRoot;
            if (anchor == null)
            {
                return;
            }

            Vector3 position = firstPersonAnchor != null
                ? firstPersonAnchor.position
                : anchor.position + Vector3.up * fallbackEyeHeight;

            transform.SetPositionAndRotation(position, Quaternion.Euler(pitch, yaw, 0f));
        }

        private void ApplyVisualState()
        {
            if (playerVisual != null)
            {
                playerVisual.SetActive(CurrentPerspective == PlayerCameraPerspective.ThirdPerson);
            }
        }

        private static float NormalizePitch(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
