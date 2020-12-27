namespace Game
{
    public interface IHitable
    {
        void TakeDamage(int damageValue);
        void Die();
    }
}
