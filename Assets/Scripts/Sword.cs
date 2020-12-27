using UnityEngine;

namespace Game
{
    public class Sword : MonoBehaviour
    {
        private bool isHitAble = false;
        public int damageValue = 10;
        public bool isPlayerSword = false;

        public void IsHitEnable()
        {
            isHitAble = true;
        }

        public void IsHitDisable()
        {
            isHitAble = false;
        }

        public void OnHit()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isHitAble) return;

            if (isPlayerSword)
            {
                if (other.CompareTag("Enemy"))
                {
                    if (other.TryGetComponent(out IHitable takeDamageObject))
                    {
                        takeDamageObject.TakeDamage(damageValue);
                        IsHitDisable();
                    }
                }
            }
            else
            {
                if (other.CompareTag("Player"))
                {
                    if (other.TryGetComponent(out IHitable takeDamageObject))
                    {
                        takeDamageObject.TakeDamage(damageValue);
                        IsHitDisable();
                    }
                }
            }
        }
    }
}
