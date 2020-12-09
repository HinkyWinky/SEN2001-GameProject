using BehaviourTree;

public abstract class State
{
    protected StateMachine stateMachine;
    protected BehaviourTreeState behaviourTreeState;

    protected bool isUpdatedFirstTime;

    protected State(StateMachine stateMachine, BehaviourTreeState behaviourTreeState)
    {
        this.stateMachine = stateMachine;
        this.behaviourTreeState = behaviourTreeState;
    }

    public virtual void Enter()
    {
        isUpdatedFirstTime = false;
        behaviourTreeState.StartEvaluateBehaviourTree();
    }
    public virtual void Exit()
    {
        behaviourTreeState.StopEvaluateBehaviourTree();
    }
    public abstract void Update();
    public abstract void FixedUpdate();
}
