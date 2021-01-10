using System;
using System.Collections;
using System.Collections.Generic;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Enemy2_ExecuteTreeState : BehaviourTreeState
    {
        private Enemy2 enemy;

        [ShowInInspector, ReadOnly] NodeStates rootNodeState;
        [Title("ApproachAttack_1")]
        [ShowInInspector, ReadOnly] private Selector approachAttack_1;
            [Title("DashAttack_11")]
            [ShowInInspector, ReadOnly] private Sequence dashAttack_11;
            public CheckIsDistance checkIsDistance1_11;
            public CheckIsDistance checkIsDistance2_11;
            public ActionDash actionDashAttack_11;
            public ActionPlayAnimation actionPlayAttackEnd_11;
            public ActionPlayAnimation actionPlayIdleAnimation_11;
            public ActionTimer actionWaitAfterAttack_11;
            [Title("CloseAttack_12")]
            [ShowInInspector, ReadOnly] private Sequence closeAttack_12;
            public CheckIsDistance checkIsDistance_12;
            public ActionAttack actionCloseAttack_12;
            public ActionPlayAnimation actionPlayIdleAnimation_12;
            public ActionTimer actionWaitAfterAttack_12;
        [Title("goToAttackPosition_2")]
        [ShowInInspector, ReadOnly] private Sequence goToAttackPosition_2;
        public CheckIsDistance checkIsDistance_2;
        public ActionMove actionGoToAttackPosition_2;
        [Title("WaitForApproach_3")]
        [ShowInInspector, ReadOnly] private Sequence waitForApproach_3;
        public CheckIsDistance checkIsDistance1_3;
        public CheckIsDistance checkIsDistance2_3;
        public ActionMirrorMovement actionMirrorTargetMoves_3;

        public override void BuildBehaviourTree(StateMachine stateMachine)
        {
            machine = stateMachine;
            enemy = machine as Enemy2;
                
                waitForApproach_3 = new Sequence(new List<Node>
                {
                    checkIsDistance1_3,
                    checkIsDistance2_3,
                    actionMirrorTargetMoves_3
                });
                goToAttackPosition_2 = new Sequence(new List<Node>
                {
                    checkIsDistance_2,
                    actionGoToAttackPosition_2
                });
                    closeAttack_12 = new Sequence(new List<Node>
                    {
                        checkIsDistance_12,
                        actionCloseAttack_12,
                        actionPlayIdleAnimation_12,
                        actionWaitAfterAttack_12
                    });
                    dashAttack_11 = new Sequence(new List<Node>
                    {
                        checkIsDistance1_11,
                        checkIsDistance2_11,
                        actionDashAttack_11,
                        actionPlayAttackEnd_11,
                        actionPlayIdleAnimation_11,
                        actionWaitAfterAttack_11
                    });
                approachAttack_1 = new Selector(new List<Node>
                {
                    dashAttack_11,
                    closeAttack_12
                });
            rootNode = new Selector(new List<Node>
            {
                approachAttack_1,
                goToAttackPosition_2,
                waitForApproach_3
            });

            checkIsDistance1_11.StartLeaf(this);
            checkIsDistance2_11.StartLeaf(this);
            actionDashAttack_11.StartLeaf(this);
            actionPlayAttackEnd_11.StartLeaf(this);
            actionPlayIdleAnimation_11.StartLeaf(this);
            actionWaitAfterAttack_11.StartLeaf(this);

            checkIsDistance_12.StartLeaf(this);
            actionCloseAttack_12.StartLeaf(this);
            actionPlayIdleAnimation_12.StartLeaf(this);
            actionWaitAfterAttack_12.StartLeaf(this);

            checkIsDistance_2.StartLeaf(this);
            actionGoToAttackPosition_2.StartLeaf(this);

            checkIsDistance1_3.StartLeaf(this);
            checkIsDistance2_3.StartLeaf(this);
            actionMirrorTargetMoves_3.StartLeaf(this);
        }

        public override void UpdateBehaviourTree()
        {
            Vector3 playerPos = machine.Player.rig.position;

            checkIsDistance1_11.UpdateLeaf(playerPos);
            checkIsDistance2_11.UpdateLeaf(playerPos);
            actionDashAttack_11.UpdateLeaf(playerPos);

            checkIsDistance_12.UpdateLeaf(playerPos);
            actionCloseAttack_12.UpdateLeaf(playerPos);
            

            checkIsDistance_2.UpdateLeaf(playerPos);
            actionGoToAttackPosition_2.UpdateLeaf(playerPos);

            checkIsDistance1_3.UpdateLeaf(playerPos);
            checkIsDistance2_3.UpdateLeaf(playerPos);
            actionMirrorTargetMoves_3.UpdateLeaf(machine.Player.rig, machine.Player.AxisInputs);
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
                machine.ChangeState(enemy.idleState);
                return;
            }

            UpdateBehaviourTree();
        }
    }
}
