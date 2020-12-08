using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    public abstract class Composite : Node
    {
        protected int curChildIndex = 0;

        protected List<Node> childNodes;

        protected override void OnReset()
        {
            curChildIndex = 0;

            for (int i = 0; i < childNodes.Count; i++)
            {
                childNodes[i].Reset();
            }
        }
    }
}
