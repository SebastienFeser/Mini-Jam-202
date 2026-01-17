using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class CombatPanel : MonoBehaviour
{
    [Header("Panel")]
    public GameObject combatPanel;

    [Header("Player Side")]
    public Image playerHealthBar;
    public TextMeshProUGUI playerHealthText;

    [Header("Enemy Side")]
    public TextMeshProUGUI enemyNameText;
    public Image enemyHealthBar;
    public TextMeshProUGUI enemyHealthText;
    public Image enemySprite;

    [Header("Actions")]
    public Button fleeButton;

    [Header("Log")]
    public TextMeshProUGUI combatLogText;

    [Header("Result")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button closeResultButton;

    private void Start()
    {
        //combatPanel.SetActive(false);
        resultPanel.SetActive(false);

        CombatManager combat = CombatManager.Instance;
        combat.OnCombatStarted += ShowCombat;
        combat.OnPlayerAttack += OnPlayerAttack;
        combat.OnEnemyAttack += OnEnemyAttack;
        combat.OnPotionUsed += OnPotionUsed;
        combat.OnPlayerHealthChanged += UpdateHealthBars;
        combat.OnEnemyHealthChanged += UpdateHealthBars;
        combat.OnCombatEnded += OnCombatEnded;

        fleeButton.onClick.AddListener(OnFleeClicked);
        closeResultButton.onClick.AddListener(CloseResult);
    }

    private void ShowCombat(ContractData contract)
    {
        combatPanel.SetActive(true);
        resultPanel.SetActive(false);

        enemyNameText.text = contract.contractName;
        if (contract.enemySprite != null)
            enemySprite.sprite = contract.enemySprite;

        combatLogText.text = "Combat started!\n";

        UpdateHealthBars();
    }

    private void UpdateHealthBars()
    {
        CombatManager combat = CombatManager.Instance;

        float playerPercent = (float)combat.GetPlayerCurrentHP() / combat.GetPlayerMaxHP();
        playerHealthBar.fillAmount = playerPercent;
        playerHealthText.text = $"{combat.GetPlayerCurrentHP()}/{combat.GetPlayerMaxHP()}";

        float enemyPercent = (float)combat.GetEnemyCurrentHP() / combat.GetEnemyMaxHP();
        enemyHealthBar.fillAmount = enemyPercent;
        enemyHealthText.text = $"{combat.GetEnemyCurrentHP()}/{combat.GetEnemyMaxHP()}";
    }

    private void OnPlayerAttack(int damage)
    {
        AddLog($"You deal {damage} damage!");
    }

    private void OnEnemyAttack(int damage)
    {
        AddLog($"Enemy deals {damage} damage!");
    }

    private void OnPotionUsed(PotionData potion)
    {
        AddLog($"<color=green>Auto-used {potion.itemName}! +{potion.healAmount} HP</color>");
    }

    private void AddLog(string message)
    {
        combatLogText.text += message + "\n";

        string[] lines = combatLogText.text.Split('\n');
        if (lines.Length > 10)
        {
            combatLogText.text = string.Join("\n", lines, lines.Length - 10, 10);
        }
    }

    private void OnFleeClicked()
    {
        CombatManager.Instance.Flee();
    }

    private void OnCombatEnded(CombatState state, int reward)
    {
        resultPanel.SetActive(true);

        switch (state)
        {
            case CombatState.VICTORY:
                resultText.text = $"<color=green>VICTORY!</color>\n\n+${reward}";
                break;
            case CombatState.DEFEAT:
                resultText.text = $"<color=red>DEFEAT!</color>\n\nYou died...";
                break;
            case CombatState.FLED:
                resultText.text = $"<color=yellow>FLED!</color>\n\nLost ${-reward}";
                break;
        }
    }

    private void CloseResult()
    {
        combatPanel.SetActive(false);
        resultPanel.SetActive(false);

        if (GameManager.Instance.currentState == GameState.GAME_OVER)
        {
            // Show game over screen
        }
    }
}