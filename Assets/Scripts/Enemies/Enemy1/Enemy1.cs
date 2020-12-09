using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy1 : StateMachine
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

    [FoldoutGroup("Execute Tree")]
    public Enemy1_ExecuteTree executeTree;
    [FoldoutGroup("Find Tree")]
    public Enemy1_FindTree findTree;

    private void Start()
    {
        animX.StartAnimation("Idle", 1f, true, 0.1f);

        executeTree.BuildBehaviourTree(this);
        findTree.BuildBehaviourTree(this);

        stateExecute = new StateExecute(this, executeTree);
        stateFind = new StateFind(this, findTree);

        StartStateMachine(stateExecute);
    }

    private void Update()
    {
        currentState.Update();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();
    }
}
