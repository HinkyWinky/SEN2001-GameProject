using Sirenix.OdinInspector;

namespace Game.AI
{
    public abstract class Decorator : Node
    {
        [ShowInInspector, ReadOnly] protected Node childNode;

        public override void OnReset() { }
    }
}
