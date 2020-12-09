using BehaviourTree;

public class StateFind : State
{
    public StateFind(StateMachine stateMachine, BehaviourTreeState behaviourTreeState) : base(stateMachine, behaviourTreeState) { }

    public override void Update()
    {
        if (behaviourTreeState.rootNode.NodeState != NodeStates.RUNNING)
        {
            if (!isUpdatedFirstTime)
            {
                isUpdatedFirstTime = true;
                stateMachine.ChangeState(stateMachine.stateExecute);
            }
        }
    }

    public override void FixedUpdate()
    {
        //throw new System.NotImplementedException();
    }
}
