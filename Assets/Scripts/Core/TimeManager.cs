using System;
using UnityEngine;
using static Enums;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public int currentDay = 1;
    public SlotType currentSlot = SlotType.MORNING;

    public event Action OnSlotChanged;
    public event Action OnDayEnded;
    public event Action<int> OnRentDue;

    [Header("Config")]
    public int rentInterval = 7;
    public int rentAmount = 200;

    private int daysSinceRent = 0;

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

    public void AdvanceSlot()
    {
        switch (currentSlot)
        {
            case SlotType.MORNING:
                currentSlot = SlotType.AFTERNOON1;
                break;
            case SlotType.AFTERNOON1:
                currentSlot = SlotType.AFTERNOON2;
                break;
            case SlotType.AFTERNOON2:
                EndDay();
                return;
        }

        OnSlotChanged?.Invoke();
    }

    private void EndDay()
    {
        currentDay++;
        currentSlot = SlotType.MORNING;
        daysSinceRent++;

        if (daysSinceRent >= rentInterval)
        {
            daysSinceRent = 0;
            OnRentDue?.Invoke(rentAmount);
        }

        OnDayEnded?.Invoke();
        OnSlotChanged?.Invoke();
    }

    public string GetSlotName()
    {
        switch (currentSlot)
        {
            case SlotType.MORNING: return "Morning";
            case SlotType.AFTERNOON1: return "Afternoon";
            case SlotType.AFTERNOON2: return "Evening";
            default: return "";
        }
    }

    public void Reset()
    {
        currentDay = 1;
        currentSlot = SlotType.MORNING;
        daysSinceRent = 0;
    }
}