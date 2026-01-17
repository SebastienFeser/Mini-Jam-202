using UnityEngine;
using System.Collections.Generic;

public class ContractManager : MonoBehaviour
{
    public static ContractManager Instance { get; private set; }

    [Header("Available Contracts")]
    public List<ContractData> allContracts = new List<ContractData>();

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

    public void SelectContract(ContractData contract)
    {
        CombatManager.Instance.StartCombat(contract);
    }
}