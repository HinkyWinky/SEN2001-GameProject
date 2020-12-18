using System.Collections;
using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : StateMachine, IHitable
{
    [Title("Enemy")]
    public Sword sword;

    [SerializeField, Min(0)] private int maxHealth = 100;
    [SerializeField, PropertyRange(0, "maxHealth")] private int health = 100;
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

    public Enemy1_TakeDamageState takeDamageState;

    public Enemy1_ExecuteTreeState executeTreeState;
    public Enemy1_FindTreeState findTreeState;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out animX);
        TryGetComponent(out rig);

        checkPath = new NavMeshPath();
        movePath = new NavMeshPath();
    }
    private void Start()
    {
        animX.StartAnimation(animX.ReturnAnimData("Idle"));

        executeTreeState.BuildBehaviourTree(this);
        findTreeState.BuildBehaviourTree(this);
        takeDamageState.BuildState(this);

        StartStateMachine(executeTreeState);
    }
    private void Update()
    {
        currentState.StateUpdate();
    }
    private void FixedUpdate()
    {
        currentState.StateFixedUpdate();
    }

    public void TakeDamage(int damageValue)
    {
        if (!takeDamageState.isHitAble) return;
        //ChangeState(takeDamageState);
        Health -= damageValue;
    }

    public void IsSwordHitEnable()
    {
        sword.IsHitEnable();
    }
    public void IsSwordHitDisable()
    {
        sword.IsHitDisable();
    }
}
