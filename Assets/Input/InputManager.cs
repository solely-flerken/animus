using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        public InputSystem_Actions InputActions;
        public InputSystem_Actions.PlayerActions PlayerActions;
        public InputSystem_Actions.UIActions UIActions;

        private void Awake()
        {
            InputActions = new InputSystem_Actions();
            PlayerActions = InputActions.Player;
            UIActions = InputActions.UI;
        }
    }
}