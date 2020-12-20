using System;
using Game.AI;

[Serializable]
public class Enemy1_IdleState : State
{
    private Enemy1 enemy;
    public AnimData idleAnimData;

    public override void BuildState(StateMachine stateMachine)
    {
        base.BuildState(stateMachine);
        enemy = stateMachine as Enemy1;
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
        if (!machine.isUpdatedFirstTime)
        {
            machine.isUpdatedFirstTime = true;
        }

        if (!GameManager.Cur.Player.IsDeath)
            machine.ChangeState(enemy.executeTreeState);
    }
}
