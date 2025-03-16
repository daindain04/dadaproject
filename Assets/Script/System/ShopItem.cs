using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int price; // ���� ����
    public GameObject purchasePanel; // ���� â �г�
   
    public Button yesButton;
    public Button noButton;
    public Button itemButton; //  ���� ��ư (�߰���)

    private void Start()
    {
        // ���� ���� �� ���� â�� ��Ȱ��ȭ
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }

   

        // ��ư Ŭ�� �̺�Ʈ ���
        yesButton.onClick.AddListener(PurchaseItem);
        noButton.onClick.AddListener(ClosePurchasePanel);

        //  ���� ��ư�� Ŭ���ϸ� ���� â ����
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OpenPurchasePanel);
        }
    }

    //  ���� â ����
    public void OpenPurchasePanel()
    {
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
        }
    }

    //  ���� â �ݱ�
    public void ClosePurchasePanel()
    {
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }
    }

    //  ���� ó��
    public void PurchaseItem()
    {
        if (CoinManager.instance.SpendCoins(price)) // ���� ����
        {
            Debug.Log("���� ����!");
            ClosePurchasePanel(); // ���� â �ݱ�
        }
        else
        {
            Debug.Log("������ �����մϴ�!");
        }
    }
}
