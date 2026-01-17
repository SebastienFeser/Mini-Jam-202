using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] Button contractsButton;
    [SerializeField] Button shopButton;
    [SerializeField] GameObject contractsPannel;
    [SerializeField] GameObject shopPannel;

    private void Start()
    {
        contractsButton.onClick.AddListener(OpenContractPannel);
        shopButton.onClick.AddListener(OpenShopPannel);
    }

    public void OpenContractPannel()
    {
        contractsPannel.SetActive(true);
    }

    public void OpenShopPannel()
    {
        shopPannel.SetActive(true);
    }
}
