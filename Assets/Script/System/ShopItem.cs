using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int price; // ���� ����
    public GameObject purchasePanel;
    public Button yesButton;
    public Button noButton;
    public Button itemButton;

    [Header("���� ����")]
    public bool isReplaceable; // ���� ���� ��ü ���� (true: ���� ���� ��ü, false: �� ���� �߰�)
    public GameObject oldFurniture; // ���� ���� (��ü�� ������ ���)
    public GameObject newFurniture; // �� ����

    private void Start()
    {
        if (purchasePanel != null)
        {
            ShopManager.instance.SetCurrentFurniture(this);
            purchasePanel.SetActive(true);
        }

        yesButton.onClick.AddListener(PurchaseItem);
        noButton.onClick.AddListener(ClosePurchasePanel);

        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OpenPurchasePanel);
        }

        if (ShopManager.instance != null && isReplaceable && oldFurniture != null && newFurniture != null)
        {
            ShopManager.instance.RegisterFurnitureReplacement(oldFurniture, newFurniture);
        }
    }

    public void OpenPurchasePanel()
    {
        if (purchasePanel != null)
        {
            //  �г��� �� �� ���� ���õ� ���� ������ ������Ʈ
            ShopManager.instance.SetCurrentFurniture(this);
            purchasePanel.SetActive(true);
        }
    }

    public void ClosePurchasePanel()
    {
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }
    }

    public void PurchaseItem()
    {
        if (CoinManager.instance.SpendCoins(price)) // ���� ����
        {
            Debug.Log("���� ����!");

            if (isReplaceable)
            {
                //  ������ ���� Ư�� ������ ��� & ��ü ����
                if (ShopManager.instance != null)
                {
                    if (!ShopManager.instance.IsFurnitureRegistered(oldFurniture))
                    {
                        ShopManager.instance.RegisterFurnitureReplacement(oldFurniture, newFurniture);
                    }
                    ShopManager.instance.ReplaceFurniture(oldFurniture);
                }
            }
            else
            {
                // �� ������ �߰��ϴ� ��� (���� ������ �߰�)
                if (ShopManager.instance != null)
                {
                    ShopManager.instance.AddNewFurniture(newFurniture);
                }
            }

            ClosePurchasePanel();
        }
        else
        {
            Debug.Log("������ �����մϴ�!");
        }
    }
}
