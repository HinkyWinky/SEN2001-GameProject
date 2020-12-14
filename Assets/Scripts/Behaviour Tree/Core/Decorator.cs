using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.AI
{
    public abstract class Decorator : Node
    {
        [ShowInInspector, ReadOnly] protected Node childNode;

        protected override void OnReset() { }
    }
}
