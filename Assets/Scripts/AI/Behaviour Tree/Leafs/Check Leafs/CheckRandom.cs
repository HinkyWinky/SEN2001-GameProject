using System;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class CheckRandom : CheckLeaf
    {
        [SerializeField, Range(0, 100)] private int percentageOfSuccess = 100;

        public override NodeStates Check()
        {
            int randomInt = UnityEngine.Random.Range(0, 101);

            return randomInt <= percentageOfSuccess ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }
    }
}
