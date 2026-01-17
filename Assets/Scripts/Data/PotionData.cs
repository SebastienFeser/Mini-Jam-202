using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Game/Potion")]
public class PotionData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public int price;
    public Sprite icon;

    [Header("Effect")]
    public int healAmount;
    public float autoUseThreshold; // Ex: 0.3 = utilise quand HP < 30%
}