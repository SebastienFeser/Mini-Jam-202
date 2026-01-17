using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public int money;

    public void Initialize(int startingMoney)
    {
        money = startingMoney;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
    }
}