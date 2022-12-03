using UnityEngine;

namespace InexperiencedDeveloper.Input
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; private set; }

        private void Awake()
        {
            InputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }
    }
}

