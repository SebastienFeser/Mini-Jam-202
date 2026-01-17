using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public CombatantStats baseStats;
    public PlayerState state = new PlayerState();
    public Inventory inventory = new Inventory();

    public event Action OnStatsChanged;
    public event Action OnDeath;

    [Header("Base Config")]
    public float baseSpeed = 1.0f;
    public int baseStrength = 10;
    public int baseHealth = 100;
    public int startingMoney = 100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePlayer();
    }

    public void InitializePlayer()
    {
        baseStats = new CombatantStats(baseSpeed, baseStrength, baseHealth);
        state.Initialize(startingMoney);
        inventory.ClearAll();
        UpdateMaxHealth();
        OnStatsChanged?.Invoke();
    }

    public float GetTotalSpeed()
    {
        return baseStats.speed + inventory.GetSpeedBonus();
    }

    public int GetTotalStrength()
    {
        return baseStats.strength + inventory.GetStrengthBonus();
    }

    public int GetTotalMaxHealth()
    {
        return baseStats.maxHealth + inventory.GetHealthBonus();
    }

    public int GetCurrentHealth()
    {
        return baseStats.currentHealth;
    }

    public void TakeDamage(int amount)
    {
        baseStats.TakeDamage(amount);
        OnStatsChanged?.Invoke();

        if (baseStats.IsDead())
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        baseStats.Heal(amount);
        if (baseStats.currentHealth > GetTotalMaxHealth())
        {
            baseStats.currentHealth = GetTotalMaxHealth();
        }
        OnStatsChanged?.Invoke();
    }

    public void UpdateMaxHealth()
    {
        int totalMax = GetTotalMaxHealth();
        if (baseStats.currentHealth > totalMax)
        {
            baseStats.currentHealth = totalMax;
        }
        baseStats.maxHealth = baseHealth;
    }

    public void AddMoney(int amount)
    {
        state.AddMoney(amount);
        OnStatsChanged?.Invoke();
    }

    public void SpendMoney(int amount)
    {
        if(state.money - amount > 0)
        {
            state.RemoveMoney(amount);
        }
        OnStatsChanged?.Invoke();
    }

    public void AddImplant(ImplantData implant)
    {
        inventory.AddImplant(implant);
        UpdateMaxHealth();
        OnStatsChanged?.Invoke();
    }

    public void AddPotion(PotionData potion)
    {
        inventory.AddPotion(potion);
        OnStatsChanged?.Invoke();
    }

    public void ClearImplants()
    {
        inventory.ClearImplants();
        UpdateMaxHealth();
        OnStatsChanged?.Invoke();
    }

    public void ClearAll()
    {
        inventory.ClearAll();
        UpdateMaxHealth();
        OnStatsChanged?.Invoke();
    }

    public CombatantStats GetCombatStats()
    {
        return new CombatantStats(GetTotalSpeed(), GetTotalStrength(), GetTotalMaxHealth())
        {
            currentHealth = baseStats.currentHealth
        };
    }
}