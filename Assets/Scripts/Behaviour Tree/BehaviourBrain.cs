using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable] public abstract class BehaviourBrain : MonoBehaviour
    {
        public Player Player => GameManager.Cur.Player;

        [Title("Behaviour Tree")]
        [SerializeField] protected bool isEvaluateAtStart = false;
        [SerializeField, Range(0.0333f, 10f), HideInPlayMode] protected float evaluateDeltaTime = 0.1f;
        [SerializeField, ReadOnly] protected bool isEvaluating;
        [Range(0f, 5f)] public float moveSpeed = 0.5f;
        [SerializeField, PropertyRange(0f, 1f), LabelText("Rotation Duration For 180")]
        private float rotationDuration = 0.1f; // roll rotation duration for 180 degree

        protected Vector3 forward;

        [HideInInspector] public Animator anim;
        [HideInInspector] public AnimatorX animX;
        [HideInInspector] public Rigidbody rig;
        protected WaitForSeconds waitTimeEvaluateDeltaTime;
        protected Composite rootNode;
        protected IEnumerator evaluateBehaviourTree;
        protected IEnumerator rotate;

        public virtual void Awake()
        {
            TryGetComponent(out anim);
            TryGetComponent(out animX);
            TryGetComponent(out rig);

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

        public void StartRotate(Vector3 targetPos)
        {
            if (rotate != null)
                StopCoroutine(rotate);
            rotate = RotateCor(targetPos);
            StartCoroutine(rotate);
        }
        private IEnumerator RotateCor(Vector3 targetPos)
        {
            Vector3 fixedTargetPos = new Vector3(targetPos.x, rig.position.y, targetPos.z); // Target`s Position on x and z axis.
            forward = Vector3.Normalize(fixedTargetPos - rig.position); // Direction from the player`s position to the target`s position.

            var startRot = rig.rotation;
            var targetRot = Quaternion.LookRotation(forward, Vector3.up); // Calculate target rotation value.
            float fixedDuration = rotationDuration * Quaternion.Angle(startRot, targetRot) / 180f; // Calculate the duration value needed to rotate to the target angle.
            float percent = 0f;
            while (percent < fixedDuration)
            {
                rig.rotation = Quaternion.Slerp(startRot, targetRot, percent / fixedDuration); // Rotate the player to the target rotation value.
                percent += Time.deltaTime;
                yield return CoroutineUtils.waitForFixedUpdate;
            }
            rig.rotation = targetRot;
            yield return CoroutineUtils.waitForFixedUpdate;
        }
    }
}
