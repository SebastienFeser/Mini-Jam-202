using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI slotText;
    public TextMeshProUGUI statsText;

    private void Start()
    {
        PlayerController.Instance.OnStatsChanged += UpdateHUD;
        TimeManager.Instance.OnSlotChanged += UpdateHUD;
        UpdateHUD();
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnStatsChanged -= UpdateHUD;
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnSlotChanged -= UpdateHUD;
    }

    private void UpdateHUD()
    {
        PlayerController player = PlayerController.Instance;
        TimeManager time = TimeManager.Instance;

        moneyText.text = $"${player.state.money}";
        hpText.text = $"HP: {player.GetCurrentHealth()}/{player.GetTotalMaxHealth()}";
        dayText.text = $"Day {time.currentDay}";
        slotText.text = time.GetSlotName();

        statsText.text = $"SPD: {player.GetTotalSpeed():F1}\n" +
                         $"STR: {player.GetTotalStrength()}\n" +
                         $"HP: {player.GetTotalMaxHealth()}";
    }
}