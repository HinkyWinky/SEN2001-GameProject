using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public abstract class BehaviourTreeState : IState
    {
        [Title("Behaviour Tree")]
        [SerializeField, Range(0.0333f, 10f), HideInPlayMode] protected float evaluateDeltaTime = 0.1f;
        [SerializeField, ReadOnly] protected bool isEvaluating;
        public float EvaluateDeltaTime => evaluateDeltaTime;

        [HideInInspector] public StateMachine machine;
        public Composite rootNode;
        protected WaitForSeconds waitTimeEvaluateDeltaTime;

        /// <summary>
        /// Create tree inside it.
        /// Call leaf`s StartLeaf() inside it.
        /// Call inside the Start() of child class.
        /// </summary>
        public abstract void BuildBehaviourTree(StateMachine stateMachine);

        /// <summary>
        /// Call leafs` UpdateLeaf() inside it.
        /// Call inside the loop of the EvaluateBehaviourTree().
        /// </summary>
        public abstract void UpdateBehaviourTree();

        /// <summary>
        /// Call the StartEvaluateBehaviourTree() instead of this.
        /// </summary>
        public abstract IEnumerator EvaluateBehaviourTree();

        /// <summary>
        /// Call to start EvaluateBehviourTree() coroutine. 
        /// </summary>
        public void StartEvaluateBehaviourTree()
        {
            isEvaluating = true;
            machine.evaluateBehaviourTree = EvaluateBehaviourTree();
            machine.StartCoroutine(machine.evaluateBehaviourTree);
        }

        /// <summary>
        /// Call to stop EvaluateBehviourTree() coroutine. 
        /// </summary>
        public void StopEvaluateBehaviourTree()
        {
            isEvaluating = false;
            if (machine.evaluateBehaviourTree != null)
                machine.StopCoroutine(machine.evaluateBehaviourTree);
            rootNode.Reset();
        }

        public virtual void StateEnter()
        {
            machine.isStateUpdatedFirstTime = false;
            StartEvaluateBehaviourTree();
        }

        public virtual void StateExit()
        {
            StopEvaluateBehaviourTree();
        }

        public virtual void StateUpdate()
        {
        }

        public virtual void StateFixedUpdate()
        {
        }
    }
}
