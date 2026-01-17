[System.Serializable]
public class CombatantStats
{
    public float speed;
    public int strength;
    public int maxHealth;
    public int currentHealth;

    public CombatantStats(float speed, int strength, int health)
    {
        this.speed = speed;
        this.strength = strength;
        this.maxHealth = health;
        this.currentHealth = health;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public float GetAttackInterval()
    {
        return 1f / speed;
    }
}