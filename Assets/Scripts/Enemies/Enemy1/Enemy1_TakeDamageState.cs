using System;
using System.Collections;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Enemy1_TakeDamageState : State
{
    private Enemy1 enemy;

    [HideInInspector] public bool isHitAble = true;

    [Title("TakeDamage", Bold = true)]
    [SerializeField, Range(0f, 100f)] private float takeDamagePushSpeed = 10f;

    private AnimData takeDamageAnimData;

    public override void BuildState(StateMachine stateMachine)
    {
        base.BuildState(stateMachine);
        enemy = stateMachine as Enemy1;
        if (enemy != null)
            takeDamageAnimData = enemy.animX.ReturnAnimData("Take Damage");
    }

    public override void StateEnter()
    {
        base.StateEnter();
        isHitAble = false;
        machine.rig.isKinematic = false;

        OnTakeDamage();
    }

    public override void StateExit()
    {
        isHitAble = true;
        machine.rig.isKinematic = true;
    }

    public override void StateUpdate()
    {
        if (!machine.isUpdatedFirstTime)
        {
            machine.isUpdatedFirstTime = true;
        }
    }

    public void OnTakeDamage()
    {
        machine.animX.StartAnimation(takeDamageAnimData);

        if (machine.action != null)
            machine.StopCoroutine(machine.action);
        machine.action = OnTakeDamageCur();
        machine.StartCoroutine(machine.action);
    }
    private IEnumerator OnTakeDamageCur()
    {
        Vector3 direction = machine.Player.Forward;
        float percent = 0f;
        while (percent < takeDamageAnimData.duration)
        {
            percent += Time.fixedDeltaTime;
            machine.rig.velocity = direction * takeDamagePushSpeed;
            yield return CoroutineUtils.waitForFixedUpdate;
        }
        machine.rig.velocity = Vector3.zero;
        yield return CoroutineUtils.waitForFixedUpdate;

        machine.ChangeState(enemy.executeTreeState);
    }
}
