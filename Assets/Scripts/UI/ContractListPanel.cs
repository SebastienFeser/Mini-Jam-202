using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContractListPanel : MonoBehaviour
{
    [Header("References")]
    public Transform contractListParent;
    public GameObject contractButtonPrefab;

    private void Start()
    {
        RefreshContractList();
    }

    public void RefreshContractList()
    {
        foreach (Transform child in contractListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ContractData contract in ContractManager.Instance.allContracts)
        {
            GameObject buttonObj = Instantiate(contractButtonPrefab, contractListParent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = $"{contract.contractName}\n" +
                              $"SPD: {contract.enemySpeed} | STR: {contract.enemyStrength} | HP: {contract.enemyHealth}\n" +
                              $"Reward: ${contract.baseReward}";

            ContractData contractCopy = contract;
            button.onClick.AddListener(() => OnContractClicked(contractCopy));
        }
    }

    private void OnContractClicked(ContractData contract)
    {
        if (GameManager.Instance.currentState != GameState.PLAYING) return;

        ContractManager.Instance.SelectContract(contract);
        gameObject.SetActive(false);
    }
}