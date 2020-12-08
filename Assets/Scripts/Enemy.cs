using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : BehaviourBrain
{
    [Title("Enemy")]
    [SerializeField, Min(0)] private int maxHealth = 3;
    [SerializeField, PropertyRange(0, "maxHealth")] private int health = 3;
    public int Health
    {
        get => health;
        set
        {
            if (value < 0) { health = 0; }
            else if (value > maxHealth) { health = maxHealth; }
            else { health = value; }
        }
    }

    [Title("Root Selector")]
        [ShowInInspector, ReadOnly]      NodeStates             rootNodeState          ;
        [ShowInInspector, ReadOnly]      Sequence               dashAttack_1           ;
        [ShowInInspector, ReadOnly]      Sequence               closeAttack_2          ;
        [ShowInInspector, ReadOnly]      Selector               findProperPos_3        ;
    [Title("DashAttack_1")]                                   
        public                           CheckIsDistance        checkIsDistance_1      ;
        public                           CheckIsWayOpen         checkIsWayOpen_1       ;  
        public                           ActionDash             actionDash_1           ;
    [Title("CloseAttack_2")]                                  
        public                           CheckIsDistance        checkIsDistance_2      ;
        public                           ActionAttack           actionAttack_2         ;
    [Title("FindProperPos_3")]
        [ShowInInspector, ReadOnly]      Sequence               goFarPos_31            ;
        [ShowInInspector, ReadOnly]      Sequence               goClosePos_32          ;
    [Title("GoFarPos_31")]
        public                           CheckIsDistance        checkIsDistance_31     ;
        public                           CheckIsWayOpen         checkIsWayOpen_31  ;
        public                           ActionMove             actionMove_31          ;
    [Title("GoClosePos_32")]
        public                           ActionMove             actionMove_32          ;

    private void Start()
    {
        animX.StartAnimation("Idle", 1f, true, 0.1f);

        BuildBehaviourTree();
        if (isEvaluateAtStart)
            StartEvaluateBehaviourTree();
    }

    protected override void BuildBehaviourTree()
    {
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
            findProperPos_3 = new Selector(new List<Node>
            {
                goFarPos_31,
                goClosePos_32,
            });
            closeAttack_2 = new Sequence(new List<Node>
            {
                checkIsDistance_2,
                actionAttack_2
            });
            dashAttack_1 = new Sequence(new List<Node>
            {
                checkIsDistance_1,
                checkIsWayOpen_1,
                actionDash_1
            });
        rootNode = new Selector(new List<Node>
        {
            dashAttack_1,
            closeAttack_2,
            findProperPos_3
        });

        actionMove_32.StartLeaf(this);
        checkIsDistance_31.StartLeaf(this);
        checkIsWayOpen_31.StartLeaf(this);
        actionMove_31.StartLeaf(this);
        checkIsDistance_1.StartLeaf(this);
        checkIsWayOpen_1.StartLeaf(this);
        actionDash_1.StartLeaf(this);
        checkIsDistance_2.StartLeaf(this);
        actionAttack_2.StartLeaf(this);
    }
    protected override void UpdateBehaviourTree()
    {
        Vector3 playerPos = Player.transform.position;

        actionMove_32.UpdateLeaf(playerPos);
        checkIsDistance_31.UpdateLeaf(playerPos);
        checkIsWayOpen_31.UpdateLeaf(playerPos);
        actionMove_31.UpdateLeaf(playerPos);
        checkIsDistance_1.UpdateLeaf(playerPos);
        checkIsWayOpen_1.UpdateLeaf(playerPos);
        actionDash_1.UpdateLeaf(playerPos);
        checkIsDistance_2.UpdateLeaf(playerPos);
        actionAttack_2.UpdateLeaf(playerPos);
    }
    protected override IEnumerator EvaluateBehaviourTree()
    {
        while (GameManager.Cur.StateCtrl.CompareGameState(GameState.PLAY))
        {
            UpdateBehaviourTree();
            rootNodeState = rootNode.Evaluate();

            yield return waitTimeEvaluateDeltaTime;
        }
    }
}
