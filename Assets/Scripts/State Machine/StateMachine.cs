using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    public abstract class StateMachine : MonoBehaviour
    {
        public Player Player => GameManager.Cur.Player;

        [HideInInspector] public Animator anim;
        [HideInInspector] public AnimatorX animX;
        [HideInInspector] public Rigidbody rig;

        protected IState currentState;
        [Range(0f, 5f)] public float moveSpeed = 0.5f;

        [SerializeField, PropertyRange(0f, 1f), LabelText("Rotation Duration For 180")]
        public float rotationDuration = 0.1f; // roll rotation duration for 180 degree

        [SerializeField, Range(500f, 3000f), LabelText("Max Rotation Acceleration For 180")]
        private float maxRotationAcceleration = 1000f; // max acceleration for 180 degree while rotating

        [HideInInspector] public NavMeshPath movePath;
        [HideInInspector] public NavMeshPath checkPath;

        public IEnumerator evaluateBehaviourTree;
        public IEnumerator action;
        public IEnumerator rotateToTargetPos;

        [HideInInspector] public bool isUpdatedFirstTime = false;

        public void StartStateMachine(IState startState)
        {
            currentState = startState;
            currentState.StateEnter();
        }

        public void ChangeState(IState newState)
        {
            currentState.StateExit();
            currentState = newState;
            currentState.StateEnter();
        }

        public void Rotate(Vector3 targetPos)
        {
            Vector3 fixedTargetPos = new Vector3(targetPos.x, rig.position.y, targetPos.z);
            Vector3 direction = (fixedTargetPos - rig.position).normalized;
            if (direction == Vector3.zero) return;
            var targetRot = Quaternion.LookRotation(direction, Vector3.up); // Calculate target rotation value.
            float fixedMaxRotationAcceleration = maxRotationAcceleration * Quaternion.Angle(rig.rotation, targetRot) / 180f; // Calculate acceleration value for the angle.
            float maxRotationChange = fixedMaxRotationAcceleration * Time.fixedDeltaTime; // Calculate acceleration needed for one frame.
            rig.rotation = Quaternion.RotateTowards(rig.rotation, targetRot, maxRotationChange); // Rotate the player to the target rotation value smoothly.
        }
    }
}
