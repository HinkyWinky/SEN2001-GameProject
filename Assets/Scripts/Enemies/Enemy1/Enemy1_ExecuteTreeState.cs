using System;
using System.Collections;
using System.Collections.Generic;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Enemy1_ExecuteTreeState : BehaviourTreeState
    {
        private Enemy1 enemy;

        [ShowInInspector, ReadOnly] NodeStates rootNodeState;
        [Title("DashAttack_1")]
        [ShowInInspector, ReadOnly] private Sequence dashAttack_1;
        public CheckIsDistance checkIsDistance_1;
        public CheckIsThereObstacle checkIsThereObstacle_1;
        public CheckPathCornerCount checkPathCornerCount_1;
        public ActionTimer actionWaitBeforeDash_1;
        public ActionDash actionDash_1;
        public ActionPlayAnimation actionPlayAnimation_1;
        public ActionTimer actionWaitAfterDash_1;
        [Title("CloseAttack_2")]
        [ShowInInspector, ReadOnly] private Selector closeAttack_2;
            [Title("Attack_21")]
            [ShowInInspector, ReadOnly] private Sequence attack_21;
            public CheckRandom chechRandom_21;
            public ActionAttack actionAttack_21;
            public ActionTimer actionWaitAfterCloseAttack_21;
            [Title("Attack_22")]
            [ShowInInspector, ReadOnly] private Sequence attack_22;
            public ActionAttack actionAttack_22;
            public ActionTimer actionWaitAfterCloseAttack_22;

        public override void BuildBehaviourTree(StateMachine stateMachine)
        {
            machine = stateMachine;
            enemy = machine as Enemy1;

            waitTimeEvaluateDeltaTime = new WaitForSeconds(evaluateDeltaTime);

                    attack_22 = new Sequence(new List<Node>()
                    {
                        actionAttack_22,
                        actionWaitAfterCloseAttack_22
                    });
                    attack_21 = new Sequence(new List<Node>()
                    {
                        chechRandom_21,
                        actionAttack_21,
                        actionWaitAfterCloseAttack_21
                    });
                closeAttack_2 = new Selector(new List<Node>
                {
                    attack_21,
                    attack_22
                });
                dashAttack_1 = new Sequence(new List<Node>
                {
                    checkIsDistance_1,
                    checkIsThereObstacle_1,
                    checkPathCornerCount_1,
                    actionWaitBeforeDash_1,
                    actionDash_1,
                    actionPlayAnimation_1,
                    actionWaitAfterDash_1
                });
            rootNode = new Selector(new List<Node>
            {
                dashAttack_1,
                closeAttack_2
            });

            checkIsDistance_1.StartLeaf(this);
            checkIsThereObstacle_1.StartLeaf(this);
            checkPathCornerCount_1.StartLeaf(this);
            actionWaitBeforeDash_1.StartLeaf(this);
            actionDash_1.StartLeaf(this);
            actionPlayAnimation_1.StartLeaf(this);
            actionWaitAfterDash_1.StartLeaf(this);

            chechRandom_21.StartLeaf(this);
            actionAttack_21.StartLeaf(this);
            actionWaitAfterCloseAttack_21.StartLeaf(this);

            actionAttack_22.StartLeaf(this);
            actionWaitAfterCloseAttack_22.StartLeaf(this);
        }

        public override void UpdateBehaviourTree()
        {
            Vector3 playerPos = machine.Player.transform.position;

            checkIsDistance_1.UpdateLeaf(playerPos);
            checkIsThereObstacle_1.UpdateLeaf(playerPos);
            checkPathCornerCount_1.UpdateLeaf(playerPos);
            actionDash_1.UpdateLeaf(playerPos);

            actionAttack_21.UpdateLeaf(playerPos);

            actionAttack_22.UpdateLeaf(playerPos);

        }

        public override IEnumerator EvaluateBehaviourTree()
        {
            UpdateBehaviourTree();

            while (GameManager.Cur.StateCtrl.CompareGameState(GameState.PLAYLEVEL))
            {
                rootNodeState = rootNode.Evaluate();
                yield return waitTimeEvaluateDeltaTime;
            }
        }

        public override void StateUpdate()
        {
            if (!machine.isStateUpdatedFirstTime)
            {
                machine.isStateUpdatedFirstTime = true;
            }

            machine.Rotate();

            if (GameManager.Cur.Player.IsDead)
            {
                machine.ChangeState(enemy.idleState);
                return;
            }

            if (rootNode.NodeState == NodeStates.FAILURE)
            {
                machine.ChangeState(enemy.findTreeState);
                return;
            }

            UpdateBehaviourTree();
        }
    }
}
