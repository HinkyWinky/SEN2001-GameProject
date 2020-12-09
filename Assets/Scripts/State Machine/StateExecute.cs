using BehaviourTree;

public class StateExecute : State
{
    public StateExecute(StateMachine stateMachine, BehaviourTreeState behaviourTreeState) : base(stateMachine, behaviourTreeState) { }

    public override void Update()
    {
        if (behaviourTreeState.rootNode.NodeState == NodeStates.FAILURE)
        {
            if (!isUpdatedFirstTime)
            {
                isUpdatedFirstTime = true;
                stateMachine.ChangeState(stateMachine.stateFind);
            }
        }
    }

    public override void FixedUpdate()
    {
        //throw new System.NotImplementedException();
    }
}
