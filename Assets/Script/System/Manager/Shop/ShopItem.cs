using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ȭ�� Ÿ�� ����
public enum CurrencyType
{
    Coin,
    Gem
}

// ���� ������ Ÿ�� ����
public enum ItemType
{
    MainRoomFurniture,
    KitchenFurniture,
    Food,
    Toy,
    Clothing
}

public class ShopItem : MonoBehaviour
{
    [Header("������ �⺻ ����")]
    public string itemName;
    public int price; // ���� ����
    public CurrencyType currencyType = CurrencyType.Coin; // ���� �Ǵ� ����
    public ItemType itemType = ItemType.MainRoomFurniture;

    [Header("UI ���")]
    public GameObject purchasePanel;
    public Button yesButton;
    public Button noButton;
    public Button itemButton;
    public TextMeshProUGUI priceText;
    public Image currencyIcon;
    public GameObject soldOutMark; // ǰ�� ǥ��

    [Header("���� ����")]
    public bool isReplaceable; // ���� ���� ��ü ���� (true: ���� ���� ��ü, false: �� ���� �߰�)
    public GameObject oldFurniture; // ���� ���� (��ü�� ������ ���)
    public GameObject newFurniture; // �� ����
    public Vector3 addPosition; // �� ���� �߰� ��ġ (isReplaceable�� false�� ��)

    [Header("ȭ�� ������")]
    public Sprite coinIcon;
    public Sprite gemIcon;

    private bool isPurchased = false;

    private void Start()
    {
        InitializeUI();
        SetupButtons();

        // ShopManager�� ���� ��� (��ü���� ���)
        if (ShopManager.instance != null && isReplaceable && oldFurniture != null && newFurniture != null)
        {
            ShopManager.instance.RegisterFurnitureReplacement(oldFurniture, newFurniture);
        }
    }

    private void InitializeUI()
    {
        // ���� �ؽ�Ʈ ����
        if (priceText != null)
        {
            priceText.text = price.ToString();
        }

        // ȭ�� ������ ����
        if (currencyIcon != null)
        {
            currencyIcon.sprite = currencyType == CurrencyType.Coin ? coinIcon : gemIcon;
        }

        // ���� �г� �ʱ� ����
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }

        UpdatePurchaseState();
    }

    private void SetupButtons()
    {
        if (yesButton != null)
            yesButton.onClick.AddListener(PurchaseItem);

        if (noButton != null)
            noButton.onClick.AddListener(ClosePurchasePanel);

        if (itemButton != null)
            itemButton.onClick.AddListener(OpenPurchasePanel);
    }

    public void OpenPurchasePanel()
    {
        if (isPurchased)
        {
            Debug.Log("�̹� ������ �������Դϴ�.");
            return;
        }

        if (purchasePanel != null)
        {
            // �г��� �� �� ���� ���õ� ���� ������ ������Ʈ
            if (ShopManager.instance != null)
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
        if (isPurchased)
        {
            Debug.Log("�̹� ������ �������Դϴ�.");
            ClosePurchasePanel();
            return;
        }

        // ȭ�� ���� �õ�
        bool purchaseSuccess = false;
        if (currencyType == CurrencyType.Coin)
        {
            if (MoneyManager.Instance != null)
                purchaseSuccess = MoneyManager.Instance.SpendCoins(price);
        }
        else if (currencyType == CurrencyType.Gem)
        {
            if (MoneyManager.Instance != null)
                purchaseSuccess = MoneyManager.Instance.SpendGems(price);
        }

        if (purchaseSuccess)
        {
            Debug.Log($"{itemName} ���� ����!");
            ProcessPurchase();
            isPurchased = true;
            UpdatePurchaseState();
            ClosePurchasePanel();

            // �ڵ� ���� ȣ��
            if (ShopManager.instance != null)
            {
                ShopManager.instance.SavePurchaseData();
            }
        }
        else
        {
            string currencyName = currencyType == CurrencyType.Coin ? "����" : "����";
            Debug.Log($"{currencyName}�� �����մϴ�!");
        }
    }

    private void ProcessPurchase()
    {
        switch (itemType)
        {
            case ItemType.MainRoomFurniture:
            case ItemType.KitchenFurniture:
                ProcessFurniturePurchase();
                break;

            case ItemType.Food:
                ProcessFoodPurchase();
                break;

            case ItemType.Toy:
                ProcessToyPurchase();
                break;

            case ItemType.Clothing:
                ProcessClothingPurchase();
                break;
        }
    }

    private void ProcessFurniturePurchase()
    {
        if (isReplaceable)
        {
            // ���� ������ ��ü
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
            // �� ���� �߰�
            if (newFurniture != null)
            {
                Vector3 spawnPos = addPosition != Vector3.zero ? addPosition : newFurniture.transform.position;
                GameObject newObj = Instantiate(newFurniture, spawnPos, newFurniture.transform.rotation);
                newObj.SetActive(true);
                Debug.Log($"�� ���� �߰�: {itemName}");
            }
        }
    }

    private void ProcessFoodPurchase()
    {
        // ���� ���� ó�� (���� �κ��丮 �ý��۰� ����)
        Debug.Log($"���� {itemName}�� �����߽��ϴ�. (�κ��丮 �߰� ����)");

        // ���⿡ ���� �κ��丮 �߰� ���� ����
        // FoodInventoryManager.instance.AddFood(itemName, 1);
    }

    private void ProcessToyPurchase()
    {
        // �峭�� ���� ó�� (���� �κ��丮 �ý��۰� ����)
        Debug.Log($"�峭�� {itemName}�� �����߽��ϴ�. (�κ��丮 �߰� ����)");

        // ���⿡ �峭�� �κ��丮 �߰� ���� ����
        // ToyInventoryManager.instance.AddToy(itemName, 1);
    }

    private void ProcessClothingPurchase()
    {
        // �� ���� ó�� (���� �ǻ� �ý��۰� ����)
        Debug.Log($"�� {itemName}�� �����߽��ϴ�. (�ǻ� ��� �߰� ����)");

        // ���⿡ �ǻ� �ý��� ���� ���� ����
        // ClothingManager.instance.UnlockClothing(itemName);
    }

    private void UpdatePurchaseState()
    {
        // ���� ��ư ���� ������Ʈ
        if (itemButton != null)
        {
            itemButton.interactable = !isPurchased;
        }

        // ǰ�� ǥ�� ������Ʈ
        if (soldOutMark != null)
        {
            soldOutMark.SetActive(isPurchased);
        }
    }

    // �ܺο��� ���� ���� ���� (���̺� �ε� �� ���)
    public void SetPurchased(bool purchased)
    {
        isPurchased = purchased;
        UpdatePurchaseState();
    }

    // ���� ���� Ȯ��
    public bool IsPurchased()
    {
        return isPurchased;
    }
}