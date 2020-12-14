using System;
using System.Collections;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Enemy1_OnHittedState : State
{
    private Enemy1 enemy;

    [HideInInspector] public bool isHitAble = true;

    [Title("TakeDamage", Bold = true)]
    [SerializeField, Range(0f, 1f)] private float hittedDuration = 0.5f;
    [SerializeField, Range(0f, 100f)] private float hittedPushSpeed = 20f;

    public override void BuildState(StateMachine stateMachine)
    {
        base.BuildState(stateMachine);
        enemy = stateMachine as Enemy1;
    }

    public override void StateEnter()
    {
        base.StateEnter();
        isHitAble = false;
        machine.rig.isKinematic = false;

        OnHitted();
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

    public void OnHitted()
    {
        machine.animX.StartAnimation("Hitted", hittedDuration, false, 0.1f);

        if (machine.action != null)
            machine.StopCoroutine(machine.action);
        machine.action = OnHittedCur();
        machine.StartCoroutine(machine.action);
    }
    private IEnumerator OnHittedCur()
    {
        Vector3 direction = machine.Player.Forward;
        float percent = 0f;
        while (percent < hittedDuration)
        {
            percent += Time.fixedDeltaTime;
            machine.rig.velocity = direction * hittedPushSpeed;
            yield return CoroutineUtils.waitForFixedUpdate;
        }
        machine.rig.velocity = Vector3.zero;
        yield return CoroutineUtils.waitForFixedUpdate;

        machine.ChangeState(enemy.executeTreeState);
    }
}
