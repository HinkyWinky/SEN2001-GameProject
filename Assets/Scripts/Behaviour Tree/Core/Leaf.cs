using UnityEngine;

namespace BehaviourTree
{
    public abstract class Leaf : Node
    {
        [HideInInspector] protected BehaviourBrain brain;

        public virtual void StartLeaf(BehaviourBrain behaviourBrain)
        {
            brain = behaviourBrain;
        }
        protected override void OnReset() { }
    }
}
