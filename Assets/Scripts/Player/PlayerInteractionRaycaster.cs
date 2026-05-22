using UnityEngine;

namespace NetworkingSimulation.Player
{
    public sealed class PlayerInteractionRaycaster : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private Transform cameraTransform;

        [Header("Interaction")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactionMask = ~0;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

        private IPlayerInteractable currentInteractable;
        private RaycastHit currentHit;

        public IPlayerInteractable CurrentInteractable => currentInteractable;

        public string CurrentPrompt => currentInteractable?.InteractionPrompt;

        private void Awake()
        {
            if (inputReader == null)
            {
                inputReader = GetComponent<PlayerInputReader>();
            }

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.InteractPressed += TryInteract;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.InteractPressed -= TryInteract;
            }
        }

        private void Update()
        {
            RefreshCurrentTarget();
        }

        private void RefreshCurrentTarget()
        {
            currentInteractable = null;

            if (cameraTransform == null)
            {
                return;
            }

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (!Physics.Raycast(ray, out currentHit, interactionRange, interactionMask, triggerInteraction))
            {
                return;
            }

            IPlayerInteractable interactable = currentHit.collider.GetComponentInParent<IPlayerInteractable>();
            if (interactable == null)
            {
                return;
            }

            PlayerInteractorContext context = BuildContext();
            if (interactable.CanInteract(context))
            {
                currentInteractable = interactable;
            }
        }

        private void TryInteract()
        {
            RefreshCurrentTarget();
            currentInteractable?.Interact(BuildContext());
        }

        private PlayerInteractorContext BuildContext()
        {
            return new PlayerInteractorContext(gameObject, cameraTransform, currentHit, inputReader);
        }
    }
}
