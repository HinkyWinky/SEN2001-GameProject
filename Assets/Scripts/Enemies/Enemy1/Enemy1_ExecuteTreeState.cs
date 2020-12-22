using System;
using System.Collections;
using System.Collections.Generic;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Enemy1_ExecuteTreeState : BehaviourTreeState
{
    private Enemy1 enemy;

    [Title("Root Selector")]
        [ShowInInspector, ReadOnly] NodeStates rootNodeState;
        [ShowInInspector, ReadOnly] Sequence dashAttack_1;
        [ShowInInspector, ReadOnly] Sequence closeAttack_2;
    [Title("DashAttack_1")]
        public CheckIsDistance checkIsDistance_1;
        public CheckIsWayOpen checkIsWayOpen_1;
        public ActionTimer actionWaitBeforeDash_1;
        public ActionDash actionDash_1;
        public ActionPlayAnimation actionPlayAnimation1;
        public ActionTimer actionWaitAfterDash_1;
    [Title("CloseAttack_2")]
        public ActionAttack actionAttack_2;
        public ActionTimer actionWaitAfterCloseAttack_2;

    public override void BuildBehaviourTree(StateMachine stateMachine)
    {
        machine = stateMachine;
        enemy = machine as Enemy1;

        waitTimeEvaluateDeltaTime = new WaitForSeconds(evaluateDeltaTime);

            closeAttack_2 = new Sequence(new List<Node>
            {
                actionAttack_2,
                actionWaitAfterCloseAttack_2
            });
            dashAttack_1 = new Sequence(new List<Node>
            {
                checkIsDistance_1,
                checkIsWayOpen_1,
                actionWaitBeforeDash_1,
                actionDash_1,
                actionPlayAnimation1,
                actionWaitAfterDash_1
            });
        rootNode = new Selector(new List<Node>
        {
            dashAttack_1,
            closeAttack_2
        });

        checkIsDistance_1.StartLeaf(this);
        checkIsWayOpen_1.StartLeaf(this);
        actionWaitBeforeDash_1.StartLeaf(this);
        actionDash_1.StartLeaf(this);
        actionPlayAnimation1.StartLeaf(this);
        actionWaitAfterDash_1.StartLeaf(this);
        actionAttack_2.StartLeaf(this);
        actionWaitAfterCloseAttack_2.StartLeaf(this);
    }

    public override void UpdateBehaviourTree()
    {
        Vector3 playerPos = machine.Player.transform.position;

        checkIsDistance_1.UpdateLeaf(playerPos);
        checkIsWayOpen_1.UpdateLeaf(playerPos);
        actionDash_1.UpdateLeaf(playerPos);
        actionAttack_2.UpdateLeaf(playerPos);
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
        if (!machine.isUpdatedFirstTime)
        {
            machine.isUpdatedFirstTime = true;
        }

        if (GameManager.Cur.Player.IsDeath)
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
