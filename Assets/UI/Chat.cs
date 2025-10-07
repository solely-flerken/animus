using System;
using System.Linq;
using Audio;
using Events;
using Input;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Environment.PointOfInterest;
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
        private bool _isChatOpen;

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

            CloseChat();
            
            UIActions.Enable();
            UIActions.Submit.performed += OnToggleChat;
            UIActions.ScrollWheel.performed += OnScroll;
            
            EventSystem.OnDisplayMessageInChat += OnMessageSubmit;
        }

        private void OnDestroy()
        {
            UIActions.Submit.performed -= OnToggleChat;
            UIActions.Cancel.performed -= OnToggleChat;
            UIActions.ScrollWheel.performed -= OnScroll;
            UIActions.Disable();

            EventSystem.OnDisplayMessageInChat -= OnMessageSubmit;
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
            if (!_isChatOpen) return;

            var scrollDelta = context.ReadValue<Vector2>();
            _chatView.scrollOffset += new Vector2(0, -scrollDelta.y * 20f);
        }

        private void OnToggleChat(InputAction.CallbackContext context)
        {
            _isChatOpen = !_isChatOpen;

            if (_isChatOpen)
            {
                OpenChat();
            }
            else
            {
                if (context.action != UIActions.Cancel)
                {
                    OnMessageSubmit(_messageInput.value);
                    _messageInput.value = "";
                }

                CloseChat();
            }
        }

        private void OpenChat()
        {
            Show();
            UIActions.Cancel.performed += OnToggleChat;
            PlayerActions.Disable();
            _messageInput.RegisterCallback<GeometryChangedEvent>(OnInputGeometryChanged);
        }

        private void CloseChat()
        {
            Hide();
            UIActions.Cancel.performed -= OnToggleChat;
            _messageInput.Blur();
            _messageInput.value = "";
            PlayerActions.Enable();
        }

        private void OnMessageSubmit(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            GlobalAudioManager.Instance.Play("submit-message");

            ExecuteCommand(message);
        }

        private void ExecuteCommand(string command)
        {
            var args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return;

            var mainCommand = args[0];
            var parameters = args.Length > 1 ? args[1..] : null;

            switch (mainCommand.ToLower())
            {
                case "/clear":
                    ClearConsole();
                    break;
                // TODO: Refactor. Just for quick testing
                case "/npc" when args.Length >= 3 && args[1] == "goto" && args[2] == "poi":
                    var agents = FindObjectsByType<Agent>(FindObjectsSortMode.None);
                    if (agents.Length == 0) return;
                    var poi = PointOfInterestRegistry.Instance?.GetRandomPoi();
                    if (poi == null) return;
                    LogMessage($"NPC {agents[0].agentModel.name} moving to POI {poi.name}");
                    agents[0].GoToPoi(poi);
                    break;
                case "/mock":
                    if (parameters == null)
                    {
                        return;
                    }

                    switch (parameters[0])
                    {
                        // TODO: Refactor
                        case "response":
                            var actionKey = parameters[1];
                            LogMessage($"Mocking action received: {actionKey}");

                            var action = new ActionPayload
                            {
                                agentId =
                                    FindObjectsByType<Agent>(FindObjectsSortMode.None).First(a => a).agentModel.id,
                                action = actionKey,
                                dialogue = "Oh, hello there!"
                            };
                            AnimusEventSystem.InvokeActionReceived(action);
                            break;
                    }

                    break;
                case not null when mainCommand.StartsWith('/'):
                    LogMessage("Unknown command: " + command);
                    break;
                default:
                    LogMessage(command);
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