using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public abstract class StateMachine : MonoBehaviour
{
    public Player Player => GameManager.Cur.Player;

    [Title("State Machine")]
    [ShowInInspector, ReadOnly] protected State currentState;
    [Range(0f, 5f)] public float moveSpeed = 0.5f;
    [SerializeField, PropertyRange(0f, 1f), LabelText("Rotation Duration For 180")]
    public float rotationDuration = 0.1f; // roll rotation duration for 180 degree

    [HideInInspector] public Animator anim;
    [HideInInspector] public AnimatorX animX;
    [HideInInspector] public Rigidbody rig;

    [HideInInspector] public NavMeshPath movePath;
    [HideInInspector] public NavMeshPath checkPath;

    public IEnumerator evaluateBehaviourTree;
    public IEnumerator moveToTargetPos;
    public IEnumerator rotateToTargetPos;

    public StateExecute stateExecute;
    public StateFind stateFind;

    public virtual void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out animX);
        TryGetComponent(out rig);

        checkPath = new NavMeshPath();
        movePath = new NavMeshPath();
    }
       
    protected void StartStateMachine(State startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
