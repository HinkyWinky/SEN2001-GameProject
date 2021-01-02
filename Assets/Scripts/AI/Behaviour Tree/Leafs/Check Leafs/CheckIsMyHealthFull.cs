using System;

namespace Game.AI
{
    [Serializable]
    public class CheckIsMyHealthFull : CheckLeaf
    {
        private int curHealth;
        private int maxHealth;

        public override NodeStates Check()
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

