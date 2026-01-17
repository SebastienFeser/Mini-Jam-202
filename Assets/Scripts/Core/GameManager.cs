using System;
using UnityEngine;
using static Enums;

public enum GameState
{
    PLAYING,
    IN_COMBAT,
    GAME_OVER
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState currentState = GameState.PLAYING;

    public event Action OnGameOver;
    public event Action OnGameRestart;

    [Header("Config")]
    public int resurrectionCost = 300;

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
        PlayerController.Instance.OnDeath += HandlePlayerDeath;
        TimeManager.Instance.OnRentDue += HandleRentDue;
        CombatManager.Instance.OnCombatStarted += (_) => currentState = GameState.IN_COMBAT;
        CombatManager.Instance.OnCombatEnded += (state, _) =>
        {
            if (state != CombatState.DEFEAT)
                currentState = GameState.PLAYING;
        };
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnDeath -= HandlePlayerDeath;
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnRentDue -= HandleRentDue;
    }

    //TODO: Resurect screen
    private void HandlePlayerDeath()
    {
        if (PlayerController.Instance.state.money >= resurrectionCost)
        {
            Resurrect();
        }
        else
        {
            TriggerGameOver();
        }
    }

    private void Resurrect()
    {
        PlayerController player = PlayerController.Instance;

        player.SpendMoney(resurrectionCost);

        player.baseStats.currentHealth = player.GetTotalMaxHealth();

        player.ClearImplants();

        currentState = GameState.PLAYING;
        Debug.Log("Résurrection! Implants perdus.");
    }

    private void HandleRentDue(int amount)
    {
        PlayerController player = PlayerController.Instance;

        player.SpendMoney(amount);

        if (player.state.money < 0)
        {
            Debug.Log("Loyer non payé! Vous êtes SDF.");
        }
        else
        {
            Debug.Log($"Loyer payé: {amount}€");
        }
    }

    public void TriggerGameOver()
    {
        currentState = GameState.GAME_OVER;
        OnGameOver?.Invoke();
        Debug.Log("GAME OVER");
    }

    public void RestartGame()
    {
        currentState = GameState.PLAYING;
        PlayerController.Instance.InitializePlayer();
        TimeManager.Instance.Reset();
        OnGameRestart?.Invoke();
    }
}