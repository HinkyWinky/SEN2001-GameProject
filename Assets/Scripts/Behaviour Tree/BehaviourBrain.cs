using System.Collections;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BehaviourBrain : MonoBehaviour
    {
        protected Composite rootNode;

        [SerializeField, Range(0.0333f, 10f)] protected float evaluateDeltaTime = 0.1f;
        protected WaitForSeconds waitTimeEvaluateDeltaTime;

        public virtual void Awake()
        {
            waitTimeEvaluateDeltaTime = new WaitForSeconds(evaluateDeltaTime);
        }

        public abstract void StartBehaviourTree();
        public abstract void UpdateBehaviourTree();
        public abstract IEnumerator EvaluateBehaviourTree();
    }
}
