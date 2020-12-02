using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : BehaviourBrain
{
    private Player Player => GameManager.Cur.Player;

    private Animator anim;
    private AnimatorX animX;
    private NavMeshAgent agent;

    [SerializeField, Min(0)] private int maxHealth = 3;
    private int health = 3;
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
    private NavMeshPath path;

    [Title("Root Node")]
    [FoldoutGroup("Behaviour Tree"), ShowInInspector, ReadOnly] private NodeStates rootState;
    [FoldoutGroup("Behaviour Tree"), ShowInInspector, ReadOnly, HideInEditorMode] private Sequence sequence1;
    [FoldoutGroup("Behaviour Tree"), ShowInInspector, ReadOnly, HideInEditorMode] private Sequence sequence2;
    [Title("Sequence1")]
    [FoldoutGroup("Behaviour Tree")] public CheckIsMyHealthFull checkIsMyHealthFull1;
    //[FoldoutGroup("Behaviour Tree")] public CheckIsPlayerHealthSmaller checkIsPlayerHealthSmaller1;
    [FoldoutGroup("Behaviour Tree")] public ActionMove actionMove1;
    [FoldoutGroup("Behaviour Tree")] public ActionAttack actionAttack1;
    [Title("Sequence2")]
    [FoldoutGroup("Behaviour Tree")] public CheckIsMyHealthFull checkIsMyHealthFull2;
    [FoldoutGroup("Behaviour Tree")] public ActionMove actionMove2;

    public override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        animX = GetComponent<AnimatorX>();
        agent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();
    }

    private void Start()
    {
        animX.StartAnimation("Idle", 1f, true, 0.1f);
        StartBehaviourTree();
        StartCoroutine(EvaluateBehaviourTree());
    }

    public override void StartBehaviourTree()
    {
        sequence1 = new Sequence(new List<Node>
        {
            checkIsMyHealthFull1,
            //checkIsPlayerHealthSmaller1,
            actionMove1,
            actionAttack1
        });
        sequence2 = new Sequence(new List<Node>
        {
            checkIsMyHealthFull2,
            actionMove2
        });
        rootNode = new Selector(new List<Node>
        {
            sequence1,
            sequence2
        });

        actionMove1.StartLeaf(agent, ref path);
        actionAttack1.StartLeaf(agent, ref path, anim, animX);

        actionMove2.StartLeaf(agent, ref path);
    }

    public override void UpdateBehaviourTree()
    {
        checkIsMyHealthFull1.UpdateLeaf(Health, maxHealth);
        //checkIsPlayerHealthSmaller1.UpdateLeaf(Player.Health);
        Vector3 playerPos = Player.transform.position;
        actionMove1.UpdateLeaf(playerPos);
        actionAttack1.UpdateLeaf(playerPos);

        checkIsMyHealthFull2.UpdateLeaf(Health, maxHealth);
        actionMove2.UpdateLeaf(playerPos);
    }

    public override IEnumerator EvaluateBehaviourTree()
    {
        while (GameManager.Cur.StateCtrl.CompareGameState(GameState.PLAY))
        {
            UpdateBehaviourTree();
            rootState = rootNode.Evaluate();
            yield return waitTimeEvaluateDeltaTime;
        }
    }
}
