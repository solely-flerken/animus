using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        public static InputSystem_Actions InputActions;
        public static InputSystem_Actions.PlayerActions PlayerActions;
        public static InputSystem_Actions.UIActions UIActions;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InputActions = new InputSystem_Actions();
            PlayerActions = InputActions.Player;
            UIActions = InputActions.UI;
        }
    }
}