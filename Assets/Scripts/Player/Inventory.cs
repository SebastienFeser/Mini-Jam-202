using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public List<ImplantData> implants = new List<ImplantData>();
    public List<PotionData> potions = new List<PotionData>();

    public float GetSpeedBonus()
    {
        float bonus = 0;
        foreach (var implant in implants)
        {
            bonus += implant.speedBonus;
        }
        return bonus;
    }

    public int GetStrengthBonus()
    {
        int bonus = 0;
        foreach (var implant in implants)
        {
            bonus += implant.strengthBonus;
        }
        return bonus;
    }

    public int GetHealthBonus()
    {
        int bonus = 0;
        foreach (var implant in implants)
        {
            bonus += implant.healthBonus;
        }
        return bonus;
    }

    public void AddImplant(ImplantData implant)
    {
        implants.Add(implant);
    }

    public void AddPotion(PotionData potion)
    {
        potions.Add(potion);
    }

    public PotionData UsePotion()
    {
        if (potions.Count > 0)
        {
            PotionData potion = potions[0];
            potions.RemoveAt(0);
            return potion;
        }
        return null;
    }

    public bool HasPotions()
    {
        return potions.Count > 0;
    }

    public void ClearImplants()
    {
        implants.Clear();
    }

    public void ClearPotions()
    {
        potions.Clear();
    }

    public void ClearAll()
    {
        implants.Clear();
        potions.Clear();
    }
}