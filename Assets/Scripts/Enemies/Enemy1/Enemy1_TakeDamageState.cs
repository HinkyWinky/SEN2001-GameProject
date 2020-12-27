using System;
using System.Collections;
using Game.AI;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Enemy1_TakeDamageState : State
    {
        private Enemy1 enemy;
        public AnimData takeDamageAnimData;

        public override void BuildState(StateMachine stateMachine)
        {
            base.BuildState(stateMachine);
            enemy = stateMachine as Enemy1;
        }

        public override void StateEnter()
        {
            base.StateEnter();
            enemy.isHitAble = false;
            machine.animX.StartAnimation(takeDamageAnimData);
            if (machine.action != null)
                machine.StopCoroutine(machine.action);
            machine.action = OnTakeDamage();
            machine.StartCoroutine(machine.action);
        }

        public override void StateExit()
        {
            enemy.isHitAble = true;
        }

        public override void StateUpdate()
        {
            if (!machine.isUpdatedFirstTime)
            {
                machine.isUpdatedFirstTime = true;
            }

            if (GameManager.Cur.Player.IsDeath)
            {
                machine.ChangeState(enemy.idleState);
                return;
            }
        }

        private IEnumerator OnTakeDamage()
        {
            Vector3 direction = machine.Player.Forward;
            float percent = 0f;
            while (percent < takeDamageAnimData.duration)
            {
                percent += Time.fixedDeltaTime;
                machine.rig.velocity = Vector3.zero;
                yield return StateMachineUtils.waitForFixedUpdate;
            }
            machine.rig.velocity = Vector3.zero;
            yield return StateMachineUtils.waitForFixedUpdate;

            machine.ChangeState(enemy.executeTreeState);
        }
    }
}
