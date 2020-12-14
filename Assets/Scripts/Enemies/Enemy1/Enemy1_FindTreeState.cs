using System;
using System.Collections;
using System.Collections.Generic;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable] public class Enemy1_FindTreeState : BehaviourTreeState
{
    private Enemy1 enemy;

    [Title("Root Selector")] [ShowInInspector, ReadOnly]
    NodeStates rootNodeState;

    [ShowInInspector, ReadOnly] Sequence goFarPos_31;
    [ShowInInspector, ReadOnly] Sequence goClosePos_32;
    [Title("GoFarPos_31")] public CheckIsDistance checkIsDistance_31;
    public CheckIsWayOpen checkIsWayOpen_31;
    public ActionMove actionMove_31;
    [Title("GoClosePos_32")] public ActionMove actionMove_32;

    public override void BuildBehaviourTree(StateMachine stateMachine)
    {
        machine = stateMachine;
        enemy = machine as Enemy1;

        waitTimeEvaluateDeltaTime = new WaitForSeconds(evaluateDeltaTime);

        goClosePos_32 = new Sequence(new List<Node>
        {
            actionMove_32
        });
        goFarPos_31 = new Sequence(new List<Node>
        {
            checkIsDistance_31,
            checkIsWayOpen_31,
            actionMove_31
        });
        rootNode = new Selector(new List<Node>
        {
            goFarPos_31,
            goClosePos_32
        });

        actionMove_32.StartLeaf(this);
        checkIsDistance_31.StartLeaf(this);
        checkIsWayOpen_31.StartLeaf(this);
        actionMove_31.StartLeaf(this);
    }

    public override void UpdateBehaviourTree()
    {
        Vector3 playerPos = machine.Player.transform.position;

        actionMove_32.UpdateLeaf(playerPos);
        checkIsDistance_31.UpdateLeaf(playerPos);
        checkIsWayOpen_31.UpdateLeaf(playerPos);
        actionMove_31.UpdateLeaf(playerPos);
    }

    public override IEnumerator EvaluateBehaviourTree()
    {
        UpdateBehaviourTree();

        while (GameManager.Cur.StateCtrl.CompareGameState(GameState.PLAY))
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

        if (rootNode.NodeState != NodeStates.RUNNING)
        {
            machine.ChangeState(enemy.executeTreeState);
            return;
        }

        UpdateBehaviourTree();
    }
}
