namespace Game.AI
{
    public interface IState
    {
        void StateEnter();
        void StateExit();
        void StateUpdate();
        void StateFixedUpdate();
    }
}
