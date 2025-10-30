namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusObject : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Object;

        public bool IsInteractable { get; set; }

        private void Start()
        {
            AnimusEntityRegistry.Instance.Register(this);
        }

        private void OnDisable()
        {
            AnimusEntityRegistry.Instance?.Unregister(this);
        }
    }
}