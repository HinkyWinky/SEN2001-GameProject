namespace BehaviourTree
{
    public abstract class Decorator : Node
    {
        protected Node childNode;

        protected override void OnReset() { }
    }
}
