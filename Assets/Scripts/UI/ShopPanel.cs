using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    [Header("Tabs")]
    public Button implantTabButton;
    public Button potionTabButton;

    [Header("Lists")]
    public Transform implantListParent;
    public Transform potionListParent;
    public GameObject itemButtonPrefab;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Other")]
    public Button closeButton;

    private void Start()
    {
        implantTabButton.onClick.AddListener(() => ShowTab("implants"));
        potionTabButton.onClick.AddListener(() => ShowTab("potions"));
        closeButton.onClick.AddListener(CloseWindow);

        RefreshShop();
        ShowTab("implants");
    }

    public void ShowTab(string tab)
    {
        implantListParent.gameObject.SetActive(tab == "implants");
        potionListParent.gameObject.SetActive(tab == "potions");
    }

    public void RefreshShop()
    {
        RefreshImplants();
        RefreshPotions();
    }

    private void RefreshImplants()
    {
        foreach (Transform child in implantListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ImplantData implant in ShopManager.Instance.availableImplants)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, implantListParent);
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Button button = buttonObj.GetComponent<Button>();

            string bonuses = "";
            if (implant.speedBonus != 0) bonuses += $"SPD +{implant.speedBonus} ";
            if (implant.strengthBonus != 0) bonuses += $"STR +{implant.strengthBonus} ";
            if (implant.healthBonus != 0) bonuses += $"HP +{implant.healthBonus}";

            text.text = $"{implant.itemName}\n{bonuses}\n${implant.price}";

            ImplantData implantCopy = implant;
            button.onClick.AddListener(() => BuyImplant(implantCopy));
        }
    }

    private void RefreshPotions()
    {
        foreach (Transform child in potionListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (PotionData potion in ShopManager.Instance.availablePotions)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, potionListParent);
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Button button = buttonObj.GetComponent<Button>();

            text.text = $"{potion.itemName}\nHeal {potion.healAmount} (auto at {potion.autoUseThreshold * 100}%)\n${potion.price}";

            PotionData potionCopy = potion;
            button.onClick.AddListener(() => BuyPotion(potionCopy));
        }
    }

    private void BuyImplant(ImplantData implant)
    {
        if (ShopManager.Instance.BuyImplant(implant))
        {
            feedbackText.text = $"Bought {implant.itemName}!";
        }
        else
        {
            feedbackText.text = "Not enough money!";
        }
    }

    private void BuyPotion(PotionData potion)
    {
        if (ShopManager.Instance.BuyPotion(potion))
        {
            feedbackText.text = $"Bought {potion.itemName}!";
        }
        else
        {
            feedbackText.text = "Not enough money!";
        }
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}