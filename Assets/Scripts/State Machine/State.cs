namespace Game.AI
{
    public abstract class State : IState
    {
        protected StateMachine machine;

        public virtual void BuildState(StateMachine stateMachine)
        {
            machine = stateMachine;
        }

        public virtual void StateEnter()
        {
            machine.isUpdatedFirstTime = false;
        }

        public virtual void StateExit()
        {
        }

        public virtual void StateUpdate()
        {
        }

        public virtual void StateFixedUpdate()
        {
        }
    }
}
