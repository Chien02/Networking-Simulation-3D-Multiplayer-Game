using UnityEngine;

namespace NetworkingSimulation.Player
{
    public interface IPlayerInteractable
    {
        string InteractionPrompt { get; }

        bool CanInteract(PlayerInteractorContext context);

        void Interact(PlayerInteractorContext context);
    }

    public readonly struct PlayerInteractorContext
    {
        public PlayerInteractorContext(GameObject player, Transform cameraTransform, RaycastHit hit, PlayerInputReader input)
        {
            Player = player;
            CameraTransform = cameraTransform;
            Hit = hit;
            Input = input;
        }

        public GameObject Player { get; }

        public Transform CameraTransform { get; }

        public RaycastHit Hit { get; }

        public PlayerInputReader Input { get; }
    }
}
