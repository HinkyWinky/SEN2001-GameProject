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
        [HideInInspector] public Collider col;

        public bool isStateChangeAble = true;
        protected IState currentState;
        [ShowInInspector, ReadOnly, PropertyOrder(-2), HideInEditorMode]
        public string CurrentStateName => currentState == null ? "Empty" : currentState.ToString();

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

        [HideInInspector] public bool isStateUpdatedFirstTime = false;

        [ReadOnly] public bool hasDoor = false;
        [HideInInspector] public Door door;

        private Vector3 rotationPos;

        public void StartStateMachine(IState startState)
        {
            currentState = startState;
            currentState.StateEnter();
        }

        public void ChangeState(IState newState)
        {
            if (!isStateChangeAble) return;
            currentState.StateExit();
            currentState = newState;
            currentState.StateEnter();
        }

        public void SetRotationTargetPos(Vector3 pos)
        {
            rotationPos = pos;
        }
        public void Rotate()
        {
            Vector3 fixedTargetPos = new Vector3(rotationPos.x, rig.position.y, rotationPos.z);
            Vector3 direction = (fixedTargetPos - rig.position).normalized;
            if (direction == Vector3.zero) return;
            var targetRot = Quaternion.LookRotation(direction, Vector3.up); // Calculate target rotation value.
            float fixedMaxRotationAcceleration = maxRotationAcceleration * Quaternion.Angle(rig.rotation, targetRot) / 180f; // Calculate acceleration value for the angle.
            float maxRotationChange = fixedMaxRotationAcceleration * Time.fixedDeltaTime; // Calculate acceleration needed for one frame.
            rig.rotation = Quaternion.RotateTowards(rig.rotation, targetRot, maxRotationChange); // Rotate the player to the target rotation value smoothly.
        }

        protected void ForwardRaycast()
        {
            Vector3 startPos = transform.position;
            Vector3 direction = transform.forward;
            const float distance = 1.5f;
            bool forwardRay = Physics.Raycast(startPos, direction, out RaycastHit hit, distance);
            if (forwardRay)
            {
                if (hit.collider.TryGetComponent(out door))
                {
                    hasDoor = true;
                    Debug.DrawLine(startPos, hit.point, Color.yellow);
                    return;
                }
                door = null;
                hasDoor = false;
                Debug.DrawLine(startPos, hit.point, Color.red);
                return;
            }
            door = null;
            hasDoor = false;
            Debug.DrawLine(startPos, startPos + direction * distance, Color.white);
        }
    }
}
