using System;
using Audio;
using Events;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI
{
    public class Chat : UserInterfaceBase
    {
        private static Chat _instance;

        private static InputSystem_Actions.UIActions UIActions => InputManager.UIActions;
        private static InputSystem_Actions.PlayerActions PlayerActions => InputManager.PlayerActions;

        private VisualElement _chatBox;
        private ScrollView _chatView;
        private TextField _messageInput;
        private bool _isEnabled;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            IsVisibleInitially = false;

            Root = GetComponent<UIDocument>().rootVisualElement;
            _chatBox = Root.Q<VisualElement>("chatBox");
            _chatView = Root.Q<ScrollView>("chatView");
            _messageInput = Root.Q<TextField>("messageInput");

            Root.style.display = DisplayStyle.None;

            UIActions.Enable();
            UIActions.Submit.performed += OnToggleChat;
            UIActions.ScrollWheel.performed += OnScroll;
        }

        private void OnDestroy()
        {
            UIActions.Submit.performed -= OnToggleChat;
            UIActions.ScrollWheel.performed -= OnScroll;
            UIActions.Disable();
        }

        protected override void Show()
        {
            Root.style.display = DisplayStyle.Flex;

            _chatView.SetEnabled(true);
            _chatBox.RemoveFromClassList("chat-closed");
            _chatBox.AddToClassList("chat-open");

            _messageInput.style.display = DisplayStyle.Flex;
        }

        protected override void Hide()
        {
            Root.style.display = DisplayStyle.Flex;

            _chatView.SetEnabled(false);
            _chatBox.RemoveFromClassList("chat-open");
            _chatBox.AddToClassList("chat-closed");

            _messageInput.style.display = DisplayStyle.None;
        }

        private void OnScroll(InputAction.CallbackContext context)
        {
            if (!_isEnabled) return;

            var scrollDelta = context.ReadValue<Vector2>();
            _chatView.scrollOffset += new Vector2(0, -scrollDelta.y * 20f);
        }

        private void OnToggleChat(InputAction.CallbackContext context)
        {
            _isEnabled = !_isEnabled;

            if (_isEnabled)
            {
                Show();
                UIActions.Cancel.performed += OnToggleChat;
                PlayerActions.Disable();
                _messageInput.RegisterCallback<GeometryChangedEvent>(OnInputGeometryChanged);
            }
            else
            {
                OnMessageSubmit();
                Hide();
                UIActions.Cancel.performed -= OnToggleChat;
                _messageInput.Blur();
                PlayerActions.Enable();
            }
        }

        private void OnMessageSubmit()
        {
            var command = _messageInput.value;

            if (string.IsNullOrEmpty(command)) return;

            GlobalAudioManager.Instance.Play("submit-message");

            ExecuteCommand(command);
            _messageInput.value = "";
        }

        private void ExecuteCommand(string command)
        {
            var args = command.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var mainCommand = args[0];
            var parameter = args.Length > 1 ? args[1] : null;

            switch (mainCommand)
            {
                case "/clear":
                    ClearConsole();
                    break;
                case not null when mainCommand.StartsWith('/'):
                    LogMessage("Unknown command: " + command);
                    break;
                default:
                    LogMessage("Player: " + command);
                    EventSystem.InvokeChatMessage(command);
                    break;
            }
        }

        private void LogMessage(string message)
        {
            var newMessageLabel = new Label(message);
            _chatView.Add(newMessageLabel);

            _chatView.schedule
                .Execute(_ => { _chatView.verticalScroller.value = _chatView.verticalScroller.highValue; })
                .ExecuteLater(10);
        }

        private void ClearConsole()
        {
            _chatView.Clear();
        }

        private void OnInputGeometryChanged(GeometryChangedEvent evt)
        {
            _messageInput.Focus();
            _messageInput.UnregisterCallback<GeometryChangedEvent>(OnInputGeometryChanged);
        }
    }
}