using System;
using UnityEngine;
using static Enums;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public CombatState currentState = CombatState.NOT_IN_COMBAT;

    private ContractData currentContract;
    private CombatantStats playerCombatStats;
    private CombatantStats enemyCombatStats;

    private float playerAttackTimer;
    private float enemyAttackTimer;

    public event Action<ContractData> OnCombatStarted;
    public event Action<int> OnPlayerAttack;      
    public event Action<int> OnEnemyAttack;              
    public event Action<int> OnPlayerHealed;             
    public event Action<PotionData> OnPotionUsed;
    public event Action OnPlayerHealthChanged;
    public event Action OnEnemyHealthChanged;
    public event Action<CombatState, int> OnCombatEnded;

    [SerializeField] GameObject combatPanel;

    

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

    private void Update()
    {
        if (currentState != CombatState.IN_PROGRESS) return;

        float deltaTime = Time.deltaTime;

        playerAttackTimer += deltaTime;
        enemyAttackTimer += deltaTime;

        float playerInterval = playerCombatStats.GetAttackInterval();
        if (playerAttackTimer >= playerInterval)
        {
            playerAttackTimer -= playerInterval;
            PlayerAttacks();
        }

        float enemyInterval = enemyCombatStats.GetAttackInterval();
        if (enemyAttackTimer >= enemyInterval)
        {
            enemyAttackTimer -= enemyInterval;
            EnemyAttacks();
        }

        CheckAutoPotion();

        if (enemyCombatStats.IsDead())
        {
            EndCombat(CombatState.VICTORY);
            return;
        }

        if (playerCombatStats.IsDead())
        {
            EndCombat(CombatState.DEFEAT);
            return;
        }
    }

    public void StartCombat(ContractData contract)
    {
        currentContract = contract;
        combatPanel.SetActive(true);
        currentState = CombatState.IN_PROGRESS;

        playerCombatStats = PlayerController.Instance.GetCombatStats();

        enemyCombatStats = new CombatantStats(
            contract.enemySpeed,
            contract.enemyStrength,
            contract.enemyHealth
        );

        playerAttackTimer = 0f;
        enemyAttackTimer = 0f;

        OnCombatStarted?.Invoke(contract);
    }

    private void PlayerAttacks()
    {
        int damage = playerCombatStats.strength;
        enemyCombatStats.TakeDamage(damage);
        OnPlayerAttack?.Invoke(damage);
        OnEnemyHealthChanged?.Invoke();
    }

    private void EnemyAttacks()
    {
        int damage = enemyCombatStats.strength;
        playerCombatStats.TakeDamage(damage);

        PlayerController.Instance.baseStats.currentHealth = playerCombatStats.currentHealth;

        OnEnemyAttack?.Invoke(damage);
        OnPlayerHealthChanged?.Invoke();
    }

    private void CheckAutoPotion()
    {
        PlayerController player = PlayerController.Instance;

        if (!player.inventory.HasPotions()) return;

        for (int i = player.inventory.potions.Count - 1; i >= 0; i--)
        {
            PotionData potion = player.inventory.potions[i];
            float healthPercent = (float)playerCombatStats.currentHealth / playerCombatStats.maxHealth;

            if (healthPercent <= potion.autoUseThreshold)
            {
                player.inventory.potions.RemoveAt(i);
                playerCombatStats.Heal(potion.healAmount);

                player.baseStats.currentHealth = playerCombatStats.currentHealth;

                OnPotionUsed?.Invoke(potion);
                OnPlayerHealed?.Invoke(potion.healAmount);
                OnPlayerHealthChanged?.Invoke();
                break;
            }
        }
    }

    public void Flee()
    {
        if (currentState != CombatState.IN_PROGRESS) return;

        EndCombat(CombatState.FLED);
    }

    private void EndCombat(CombatState endState)
    {
        currentState = endState;

        int reward = 0;

        switch (endState)
        {
            case CombatState.VICTORY:
                reward = currentContract.baseReward;
                PlayerController.Instance.AddMoney(reward);
                break;

            case CombatState.DEFEAT:
                break;

            case CombatState.FLED:
                int penalty = Mathf.RoundToInt(PlayerController.Instance.state.money * 0.2f);
                PlayerController.Instance.SpendMoney(penalty);
                reward = -penalty;
                break;
        }

        TimeManager.Instance.AdvanceSlot();

        OnCombatEnded?.Invoke(endState, reward);
        combatPanel.SetActive(false);
        Debug.Log("CloseCombat");
        currentContract = null;
    }

    public int GetPlayerCurrentHP() => playerCombatStats?.currentHealth ?? 0;
    public int GetPlayerMaxHP() => playerCombatStats?.maxHealth ?? 0;
    public int GetEnemyCurrentHP() => enemyCombatStats?.currentHealth ?? 0;
    public int GetEnemyMaxHP() => enemyCombatStats?.maxHealth ?? 0;
    public string GetEnemyName() => currentContract?.contractName ?? "";
}