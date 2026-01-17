using UnityEngine;
using System.Collections.Generic;
using System;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Shop Inventory")]
    public List<ImplantData> availableImplants = new List<ImplantData>();
    public List<PotionData> availablePotions = new List<PotionData>();

    public event Action OnPurchaseComplete;

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

    public bool BuyImplant(ImplantData implant)
    {
        PlayerController player = PlayerController.Instance;

        if (player.state.money - implant.price > 0)
        {
            player.SpendMoney(implant.price);
            player.AddImplant(implant);
            OnPurchaseComplete?.Invoke();
            return true;
        }
        return false;
    }

    public bool BuyPotion(PotionData potion)
    {
        PlayerController player = PlayerController.Instance;

        if(player.state.money - potion.price > 0)
        {
            player.SpendMoney(potion.price);
            player.AddPotion(potion);
            OnPurchaseComplete?.Invoke();
            return true;
        }
        return false;
    }
}