namespace Game.AI
{
    public abstract class ActionLeaf : Leaf
    {
        protected abstract NodeStates Action();

        public override NodeStates OnEvaluate()
        {
            switch (Action())
            {
                case NodeStates.FAILURE:
                    return NodeStates.FAILURE;
                case NodeStates.RUNNING:
                    return NodeStates.RUNNING;
                case NodeStates.SUCCESS:
                    return NodeStates.SUCCESS;
                default:
                    return NodeStates.FAILURE;
            }
        }
    }
}
