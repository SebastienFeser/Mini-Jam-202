using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum StatType
    {
        SPEED,
        STRENGTH,
        HEALTH
    }

    public enum SlotType
    {
        MORNING,
        AFTERNOON1,
        AFTERNOON2
    }

    public enum CombatState
    {
        NOT_IN_COMBAT,
        IN_PROGRESS,
        VICTORY,
        DEFEAT,
        FLED
    }
}
