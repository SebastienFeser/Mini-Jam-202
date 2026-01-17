using UnityEngine;

[CreateAssetMenu(fileName = "New Implant", menuName = "Game/Implant")]
public class ImplantData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public int price;
    public Sprite icon;

    [Header("Stat Bonuses")]
    public float speedBonus;
    public int strengthBonus;
    public int healthBonus;
}