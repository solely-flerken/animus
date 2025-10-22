using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Events;
using Input;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Environment.PointOfInterest;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI.Chat
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

        private readonly List<string> _messageHistory = new();
        private string _currentMessage;
        private int _historyIndex = -1;

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
            _messageInput.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);

            CloseChat();

            UIActions.Enable();
            UIActions.Submit.performed += OnToggleChat;
            UIActions.ScrollWheel.performed += OnScroll;

            EventSystem.OnDisplayMessageInChat += LogMessage;
        }

        private void OnDestroy()
        {
            UIActions.Submit.performed -= OnToggleChat;
            UIActions.Cancel.performed -= OnToggleChat;
            UIActions.ScrollWheel.performed -= OnScroll;
            UIActions.Disable();

            EventSystem.OnDisplayMessageInChat -= LogMessage;
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

            _historyIndex = -1;
        }

        private void OnMessageSubmit(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            if (_messageHistory.Count == 0 || message != _messageHistory[0])
            {
                _messageHistory.Insert(0, message);
            }

            if (message.StartsWith('/'))
            {
                ExecuteCommand(message);
            }
            else
            {
                LogMessage($"Player: {message}");
                EventSystem.InvokeChatMessage(message);
            }
        }

        private void ExecuteCommand(string command)
        {
            var args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return;

            var mainCommand = args[0];
            var parameters = args.Length > 1 ? args[1..] : null;

            Agent agent;
            switch (mainCommand.ToLower())
            {
                case "/clear":
                    ClearConsole();
                    break;
                case "/npc" when parameters?.Length >= 3 && parameters[1] == "goto" && parameters[2] == "poi":
                    agent = AgentRegistry.Instance.allItems.FirstOrDefault(x =>
                        x.animusAgent.gameKey == parameters[0]);
                    if (agent == null)
                    {
                        LogMessage($"No NPC with the gameKey: {parameters[0]}");
                        return;
                    }

                    var poi = PointOfInterestRegistry.Instance.GetRandomPoi();
                    if (poi == null)
                    {
                        return;
                    }

                    LogMessage($"NPC {agent.animusAgent.name} moving to POI {poi.name}");
                    agent.GoToPoi(poi);
                    break;
                case "/talk" when parameters?.Length >= 2 && !string.IsNullOrEmpty(parameters[1]):
                    agent = AgentRegistry.Instance.allItems.FirstOrDefault(x => x.animusAgent.gameKey == parameters[0]);
                    if (agent == null)
                    {
                        LogMessage($"No NPC with the gameKey: {parameters[0]}");
                        return;
                    }

                    // TODO:
                    var animusEvent = new DialogEvent
                    {
                        EventType = AnimusEventType.Dialog,
                        EventSource = null,
                        EventTarget = new List<AnimusEntity> { agent.animusAgent },
                    };

                    AnimusEventSystem.InvokeDialogEvent(animusEvent);
                    break;
                default:
                    LogMessage("Invalid or unknown command: " + command);
                    break;
            }
        }

        private void LogMessage(string message)
        {
            var newMessageLabel = new Label(message);
            _chatView.Add(newMessageLabel);

            GlobalAudioManager.Instance.Play("submit-message");

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

        private void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.UpArrow:
                {
                    evt.StopPropagation();

                    if (_historyIndex == -1)
                    {
                        _currentMessage = _messageInput.value;
                    }
                    
                    if (_historyIndex < _messageHistory.Count - 1)
                    {
                        _historyIndex++;
                    }

                    if (_historyIndex >= 0)
                    {
                        _messageInput.value = _messageHistory[_historyIndex];
                        _messageInput.selectIndex = _messageInput.text.Length;
                    }

                    break;
                }
                case KeyCode.DownArrow:
                {
                    evt.StopPropagation();

                    if (_historyIndex >= 0)
                    {
                        _historyIndex--;
                        if (_historyIndex >= 0)
                        {
                            _messageInput.value = _messageHistory[_historyIndex];
                            _messageInput.selectIndex = _messageInput.text.Length;
                        }
                        else
                        {
                            _messageInput.value = _currentMessage;
                        }
                    }

                    break;
                }
            }
        }
    }
}