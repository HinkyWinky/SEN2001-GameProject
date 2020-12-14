using System;

namespace Game.AI
{
    [Serializable]
    public class CheckIsMyHealthFull : CheckLeaf
    {
        private StateMachine Machine => btState.machine;

        private int curHealth;
        private int maxHealth;

        protected override NodeStates Check()
        {
            return curHealth == maxHealth ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        public void UpdateLeaf(int currentHealth, int maximumHealth)
        {
            curHealth = currentHealth;
            maxHealth = maximumHealth;
        }
    }
}

