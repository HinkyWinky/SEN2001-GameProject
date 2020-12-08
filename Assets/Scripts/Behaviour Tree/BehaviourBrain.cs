using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable] public abstract class BehaviourBrain : MonoBehaviour
    {
        public Player Player => GameManager.Cur.Player;

        [Title("Behaviour Tree")]
        [SerializeField] protected bool isEvaluateAtStart = false;
        [SerializeField, ReadOnly] protected bool isEvaluating;
        [SerializeField, Range(0.0333f, 10f), HideInPlayMode] protected float evaluateDeltaTime = 0.1f;
        public float EvaluateDeltaTime => evaluateDeltaTime;
        [Range(0f, 5f)] public float moveSpeed = 0.5f;
        [SerializeField, PropertyRange(0f, 1f), LabelText("Rotation Duration For 180")]
        public float rotationDuration = 0.1f; // roll rotation duration for 180 degree

        protected Vector3 forward;

        protected Composite rootNode;
        [HideInInspector] public Animator anim;
        [HideInInspector] public AnimatorX animX;
        [HideInInspector] public Rigidbody rig;
        protected WaitForSeconds waitTimeEvaluateDeltaTime;
        protected IEnumerator evaluateBehaviourTree;
        [HideInInspector] public NavMeshPath movePath;
        [HideInInspector] public NavMeshPath checkPath;
        public IEnumerator moveToTargetPos;
        public IEnumerator rotateToTargetPos;

        public virtual void Awake()
        {
            TryGetComponent(out anim);
            TryGetComponent(out animX);
            TryGetComponent(out rig);

            checkPath = new NavMeshPath();
            movePath = new NavMeshPath();
            waitTimeEvaluateDeltaTime = new WaitForSeconds(evaluateDeltaTime);
        }

        /// <summary>
        /// Create tree inside it.
        /// Call leaf`s StartLeaf() inside it.
        /// Call inside the Start() of child class.
        /// </summary>
        protected abstract void BuildBehaviourTree();

        /// <summary>
        /// Call leafs` UpdateLeaf() inside it.
        /// Call inside the loop of the EvaluateBehaviourTree().
        /// </summary>
        protected abstract void UpdateBehaviourTree();

        /// <summary>
        /// Call the StartEvaluateBehaviourTree() instead of this.
        /// </summary>
        protected abstract IEnumerator EvaluateBehaviourTree();

        /// <summary>
        /// Call to start EvaluateBehviourTree() coroutine. 
        /// </summary>
        protected void StartEvaluateBehaviourTree()
        {
            StopEvaluateBehaviourTree();

            isEvaluating = true;
            evaluateBehaviourTree = EvaluateBehaviourTree();
            StartCoroutine(evaluateBehaviourTree);
        }

        /// <summary>
        /// Call to stop EvaluateBehviourTree() coroutine. 
        /// </summary>
        protected void StopEvaluateBehaviourTree()
        {
            if (evaluateBehaviourTree != null)
                StopCoroutine(evaluateBehaviourTree);
            isEvaluating = false;
        }
    }
}
