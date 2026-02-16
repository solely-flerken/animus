using System.Collections.Generic;
using Core.UI.Scripts;
using Features.Interaction.Scripts.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.Interaction.Scripts.UI
{
    public class InteractionUIManager : UserInterfaceBase
    {
        public static InteractionUIManager Instance;

        [SerializeField] private Sprite selectionArrow;
        
        private VisualElement _actionList;
        private IReadOnlyList<InteractionAction> _actions;
        private readonly List<VisualElement> _actionElements = new();
        private int _selectedIndex;

        private bool HasActions => _actions is { Count: > 0 };
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            _actionList = Root.Q<VisualElement>("actionList");

            Hide();
        }

        public void Show(Interactable interactable)
        {
            _actions = interactable.Actions;
            
            if (!HasActions)
            {
                Hide();
                return;
            }

            _selectedIndex = 0;
            
            RebuildList();
            UpdateSelection();
            
            base.Show();
        }

        public new void Hide()
        {
            _actions = null;
            _actionElements.Clear();
            _actionList?.Clear();
            _selectedIndex = 0;
            
            base.Hide();
        }

        public void SelectNext()
        {
            if (_actions == null || _actions.Count <= 1) return;
            _selectedIndex = (_selectedIndex + 1) % _actions.Count;
            UpdateSelection();
        }

        public void SelectPrevious()
        {
            if (_actions == null || _actions.Count <= 1) return;
            _selectedIndex = (_selectedIndex - 1 + _actions.Count) % _actions.Count;
            UpdateSelection();
        }

        public InteractionAction GetSelectedAction()
        {
            if (_actions == null || _actions.Count == 0) return null;
            return _actions[_selectedIndex];
        }

        private void RebuildList()
        {
            _actionList.Clear();
            _actionElements.Clear();

            foreach (var action in _actions)
            {
                var item = CreateActionItem(action);
                _actionList.Add(item);
                _actionElements.Add(item);
            }
        }
        
        private VisualElement CreateActionItem(InteractionAction action)
        {
            var item = new VisualElement();
            item.AddToClassList("action-item");
                
            var arrow = new VisualElement { name = "arrow" };
            arrow.AddToClassList("selection-arrow");
            
            if (selectionArrow != null)
                arrow.style.backgroundImage = new StyleBackground(selectionArrow);
            
            item.Add(arrow);
                
            var label = new Label(action.Label);
            label.Bind(BindableProperty<string>.Bind(() => action.Label), nameof(Label.text));
            label.AddToClassList("action-label");
            item.Add(label);

            return item;
        }
        
        private void UpdateSelection()
        {
            for (var i = 0; i < _actionElements.Count; i++)
            {
                var visibility = i == _selectedIndex ? Visibility.Visible : Visibility.Hidden;
                _actionElements[i].Q<VisualElement>("arrow").style.visibility = visibility;
            }
        }
    }
}