using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Audio.Scripts;
using Core.Input.Scripts;
using Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using Features.Player.Scripts;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Features.Chat.Scripts
{
    public class Chat : UserInterfaceBase
    {
        public static Chat Instance;

        public static event Action OnChatClosed;
        
        private static InputSystem_Actions.UIActions UIActions => InputManager.UIActions;
        private static InputSystem_Actions.PlayerActions PlayerActions => InputManager.PlayerActions;

        [Header("Settings")] 
        [SerializeField] private bool enableTypewriter = true;
        [SerializeField] private int typewriterWpm = 300;
        
        private VisualElement _chatBox;
        private ScrollView _chatView;
        private TextField _messageInput;
        private bool _isChatOpen;

        private readonly List<string> _messageHistory = new();
        private string _currentMessage;
        private int _historyIndex = -1;

        private CancellationTokenSource _destroyCts;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _destroyCts = new CancellationTokenSource();
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

            // EventSystem.OnMessage += LogMessage;
        }

        private void OnDestroy()
        {
            _destroyCts?.Cancel();
            _destroyCts?.Dispose();
            
            UIActions.Submit.performed -= OnToggleChat;
            UIActions.Cancel.performed -= OnToggleChat;
            UIActions.ScrollWheel.performed -= OnScroll;
            UIActions.Disable();

            // EventSystem.OnMessage -= LogMessage;
        }

        public override void Show()
        {
            Root.style.display = DisplayStyle.Flex;

            _chatView.SetEnabled(true);
            _chatBox.RemoveFromClassList("chat-closed");
            _chatBox.AddToClassList("chat-open");

            _messageInput.style.display = DisplayStyle.Flex;
        }

        public override void Hide()
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
            if (_isChatOpen)
            {
                if (context.action != UIActions.Cancel)
                {
                    OnMessageSubmit(_messageInput.value);
                    _messageInput.value = "";
                }

                CloseChat();
            }
            else
            {
                OpenChat();
            }
        }

        public void OpenChat(string prefillText = null)
        {
            _isChatOpen = true;
            Show();
            UIActions.Cancel.performed += OnToggleChat;
            PlayerActions.Disable();
            
            if (!string.IsNullOrEmpty(prefillText))
            {
                _messageInput.value = prefillText;
            }
            
            StartCoroutine(FocusAndPositionCursor());
        }

        private void CloseChat()
        {
            _isChatOpen = false;
            Hide();
            UIActions.Cancel.performed -= OnToggleChat;
            _messageInput.Blur();
            _messageInput.value = "";
            PlayerActions.Enable();

            _historyIndex = -1;
            
            OnChatClosed?.Invoke();
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
                LogMessage("Player", message, true).Forget();
            }
        }

        private void ExecuteCommand(string command)
        {
            var args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return;

            var mainCommand = args[0];
            var parameters = args.Length > 1 ? args[1..] : null;

            AnimusAgent animusAgent;
            switch (mainCommand.ToLower())
            {
                case "/clear":
                    ClearConsole();
                    break;
                case "/npc" when parameters?.Length >= 3 && parameters[1] == "goto" && parameters[2] == "poi":
                    animusAgent = AnimusGameManager.EntityRegistry.GetAll<AnimusAgent>().FirstOrDefault(x =>
                        x.gameKey == parameters[0]);
                    if (animusAgent == null)
                    {
                        LogMessage("System",$"No NPC with the gameKey: {parameters[0]}", true).Forget();
                        return;
                    }

                    var poi = AnimusGameManager.EntityRegistry.GetRandom<AnimusLocation>();
                    if (poi == null)
                    {
                        return;
                    }

                    LogMessage("System", $"NPC {animusAgent.name} moving to POI {poi.name}", true).Forget();
                    _ = animusAgent.GoToPoi(poi);
                    break;
                case "/talk" when parameters?.Length >= 2:
                    animusAgent = AnimusGameManager.EntityRegistry.GetAll<AnimusAgent>()
                        .FirstOrDefault(x => x.gameKey == parameters[0]);
                    if (animusAgent == null)
                    {
                        LogMessage("System", $"No NPC with the gameKey: {parameters[0]}", true).Forget();
                        return;
                    }

                    var messageText = string.Join(' ', parameters.Skip(1));

                    var source = AnimusGameManager.EntityRegistry.GetAll<AnimusPlayer>().First();
                    source.GetComponent<AnimusPlayerController>().PlayerSpeak(messageText.Trim(), animusAgent.gameKey);
                    LogMessage($"Player to {animusAgent.name}", messageText.Trim(), true).Forget();
                    break;
                default:
                    LogMessage("System", "Invalid or unknown command: " + command, true).Forget();
                    break;
            }
        }

        public async UniTask LogMessage(string sender, string message, bool isInstant)
        {
            try
            {
                var prefix = string.IsNullOrEmpty(sender) ? "" : $"{sender}: ";

                var newMessageLabel = new Label(string.Empty);
                newMessageLabel.style.whiteSpace = WhiteSpace.Normal;

                if (_chatView == null) return;
                
                _chatView.Add(newMessageLabel);

                GlobalAudioManager.Instance?.Play("submit-message");

                newMessageLabel.text = prefix;

                var showInstantly = !enableTypewriter || isInstant;

                if (showInstantly)
                {
                    newMessageLabel.text += message;
                    await Task.Yield();
                    ScrollToBottom();
                }
                else
                {
                    await TypeTextAsync(newMessageLabel, message, _destroyCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // This is expected behavior when the game stops or chat is destroyed.
                // Do nothing.
            }
            catch (Exception e)
            {
                Debug.LogError($"Chat LogMessage Error: {e}");
            }
        }
        
        private async UniTask TypeTextAsync(Label label, string textToType, CancellationToken token)
        {
            var charsPerSecond = (typewriterWpm * 5f) / 60f;
            var delayMs = Mathf.Max(1, (int)(1000f / charsPerSecond));

            for (var i = 0; i < textToType.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                
                if (label == null) return;
                
                label.text += textToType[i];
                
                if (i % 3 == 0) ScrollToBottom();
                
                await UniTask.Delay(delayMs, cancellationToken: token);
            }
            ScrollToBottom();
        }
        
        private void ScrollToBottom()
        {
            _chatView.schedule
                .Execute(_ => { _chatView.verticalScroller.value = _chatView.verticalScroller.highValue; })
                .ExecuteLater(10);
        }
        
        private void ClearConsole()
        {
            _chatView.Clear();
        }

        private IEnumerator FocusAndPositionCursor()
        {
            yield return new WaitForEndOfFrame();
            _messageInput.Focus();
            _messageInput.cursorIndex = _messageInput.text.Length;
            _messageInput.selectIndex = _messageInput.text.Length;
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