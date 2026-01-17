using UnityEngine;

[CreateAssetMenu(fileName = "New Contract", menuName = "Game/Contract")]
public class ContractData : ScriptableObject
{
    [Header("Info")]
    public string contractName;
    [TextArea] public string description;
    public Sprite enemySprite;

    [Header("Enemy Stats")]
    public float enemySpeed;      
    public int enemyStrength;     
    public int enemyHealth;       

    [Header("Rewards")]
    public int baseReward;

    [Header("Texts")]
    [TextArea] public string victoryText;
    [TextArea] public string defeatText;
    [TextArea] public string fleeText;
}