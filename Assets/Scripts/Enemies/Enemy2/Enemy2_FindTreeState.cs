using System;
using System.Collections;
using System.Collections.Generic;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Enemy2_FindTreeState : BehaviourTreeState
    {
        private Enemy2 enemy;

        [ShowInInspector, ReadOnly] NodeStates rootNodeState;
        [Title("GoFarPos_1")]
        [ShowInInspector, ReadOnly] Sequence goFarPos_1;
        public CheckIsDistance checkIsDistance_1;
        public CheckIsThereObstacle checkIsThereObstacle_1;
        public CheckPathCornerCount checkPathCornerCount_1;
        public ActionMove actionMove_1;
        [Title("GoClosePos_2")]
        [ShowInInspector, ReadOnly] Sequence goClosePos_2;
        public ActionMove actionMove_2;

        public override void BuildBehaviourTree(StateMachine stateMachine)
        {
            machine = stateMachine;
            enemy = machine as Enemy2;

            waitTimeEvaluateDeltaTime = new WaitForSeconds(evaluateDeltaTime);

            goClosePos_2 = new Sequence(new List<Node>
            {
                actionMove_2
            });
            goFarPos_1 = new Sequence(new List<Node>
            {
                checkIsDistance_1,
                checkIsThereObstacle_1,
                checkPathCornerCount_1,
                actionMove_1
            });
            rootNode = new Selector(new List<Node>
        {
            goFarPos_1,
            goClosePos_2
        });

            checkIsDistance_1.StartLeaf(this);
            checkIsThereObstacle_1.StartLeaf(this);
            checkPathCornerCount_1.StartLeaf(this);
            actionMove_1.StartLeaf(this);

            actionMove_2.StartLeaf(this);
        }

        public override void UpdateBehaviourTree()
        {
            Vector3 playerPos = machine.Player.transform.position;

            checkIsDistance_1.UpdateLeaf(playerPos);
            checkIsThereObstacle_1.UpdateLeaf(playerPos);
            checkPathCornerCount_1.UpdateLeaf(playerPos);
            actionMove_1.UpdateLeaf(playerPos);

            actionMove_2.UpdateLeaf(playerPos);

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

            if (GameManager.Cur.Player.IsDeath)
            {
                machine.ChangeState(enemy.idleState);
                return;
            }

            if (rootNode.NodeState != NodeStates.RUNNING)
            {
                machine.ChangeState(enemy.executeTreeState);
                return;
            }

            UpdateBehaviourTree();
        }
    }
}
