using Game.AI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : StateMachine, IHitable
{
    [Title("Enemy")]
    public Sword sword;

    [SerializeField, Min(0)] private int maxHealth = 100;
    [ShowInInspector, ReadOnly] private int health;
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
    public bool IsDeath => Health == 0;
    [HideInInspector] public bool isHitAble = true;

    public Enemy1_IdleState idleState;
    public Enemy1_TakeDamageState takeDamageState;
    public Enemy1_DeathState deathState;
    public Enemy1_ExecuteTreeState executeTreeState;
    public Enemy1_FindTreeState findTreeState;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out animX);
        TryGetComponent(out rig);
        TryGetComponent(out col);

        checkPath = new NavMeshPath();
        movePath = new NavMeshPath();

        Health = maxHealth;
    }
    private void Start()
    {
        idleState.BuildState(this);
        takeDamageState.BuildState(this);
        deathState.BuildState(this);
        executeTreeState.BuildBehaviourTree(this);
        findTreeState.BuildBehaviourTree(this);
        
        GameManager.Cur.EventCtrl.onEnemyHealthChange?.Invoke(Health, maxHealth);

        StartStateMachine(idleState);
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
        if (!isHitAble) return;

        Health -= damageValue;
        GameManager.Cur.EventCtrl.onEnemyHealthChange?.Invoke(Health, maxHealth);

        if (IsDeath)
        {
            Die();
            return;
        }

        ChangeState(takeDamageState);
    }

    public void Die()
    {
        ChangeState(deathState);
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
