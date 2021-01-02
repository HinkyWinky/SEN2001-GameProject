namespace Game.AI
{
    public abstract class CheckLeaf : Leaf
    {
        public abstract NodeStates Check();

        public override NodeStates OnEvaluate()
        {
            switch (Check())
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
