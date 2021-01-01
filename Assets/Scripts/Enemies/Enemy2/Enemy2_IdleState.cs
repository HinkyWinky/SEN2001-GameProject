using System;
using Game.AI;

namespace Game
{
    [Serializable] public class Enemy2_IdleState : State
    {
        private Enemy2 enemy;
        public AnimData idleAnimData;

        public override void BuildState(StateMachine stateMachine)
        {
            base.BuildState(stateMachine);
            enemy = stateMachine as Enemy2;
        }

        public override void StateEnter()
        {
            base.StateEnter();
            machine.animX.StartAnimation(idleAnimData);
        }

        public override void StateExit()
        {
        }

        public override void StateUpdate()
        {
            if (!machine.isStateUpdatedFirstTime)
            {
                machine.isStateUpdatedFirstTime = true;
            }

            if (!GameManager.Cur.Player.IsDeath)
                machine.ChangeState(enemy.executeTreeState);
        }
    }
}
