using System;
using Game.AI;

[Serializable]
public class Enemy1_DeathState : State
{
    private Enemy1 enemy;
    public AnimData dieAnimData;

    public override void BuildState(StateMachine stateMachine)
    {
        base.BuildState(stateMachine);
        enemy = stateMachine as Enemy1;
    }

    public override void StateEnter()
    {
        base.StateEnter();
        enemy.isHitAble = false;
        enemy.col.isTrigger = true;
        machine.animX.StartAnimation(dieAnimData);

        GameManager.Cur.EventCtrl.onEnemyDie?.Invoke();
    }

    public override void StateExit()
    {
        enemy.isHitAble = true;
        enemy.col.isTrigger = false;
    }

    public override void StateUpdate()
    {
        if (!machine.isUpdatedFirstTime)
        {
            machine.isUpdatedFirstTime = true;
        }

        if (!enemy.IsDeath)
            machine.ChangeState(enemy.idleState);
    }
}
