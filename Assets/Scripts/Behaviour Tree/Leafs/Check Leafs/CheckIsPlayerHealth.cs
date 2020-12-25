using System;
using UnityEngine;

namespace Game.AI
{
    [Serializable] public class CheckIsPlayerHealth : CheckLeaf
    {
        [SerializeField] private CheckType checkType = default;
        [SerializeField, Min(0)] private int compareValue = 3;
        private int playerHealth;

        protected override NodeStates Check()
        {
            switch (checkType)
            {
                case CheckType.IS_GREATER:
                    return playerHealth > compareValue ? NodeStates.SUCCESS : NodeStates.FAILURE;
                case CheckType.IS_SMALLER:
                    return playerHealth < compareValue ? NodeStates.SUCCESS : NodeStates.FAILURE;
                case CheckType.IS_EQUAL:
                    return playerHealth == compareValue ? NodeStates.SUCCESS : NodeStates.FAILURE;
                default:
                    return NodeStates.FAILURE;
            }
        }

        public void UpdateLeaf(int playerHealthValue)
        {
            playerHealth = playerHealthValue;
        }
    }
}

