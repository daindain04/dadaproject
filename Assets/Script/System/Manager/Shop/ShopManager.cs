using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private Dictionary<GameObject, GameObject> furnitureReplacementMap = new Dictionary<GameObject, GameObject>();

    [Header("Shop UI")]
    public GameObject shopPanel;
    public Button shopButton;
    public GameObject PurchaseAsk;

    [Header("Shop Pages")]
    public GameObject mainRoomPage;
    public GameObject FoodPage;
    public GameObject toyPage;
    public GameObject closetPage;

    [Header("Scroll Views in MainRoomPage")]
    public GameObject mainScrollView;
    public GameObject kitchenScrollView;

    [Header("Page Buttons")]
    public Button mainRoomButton;
    public Button FoodButton;
    public Button toyButton;
    public Button closetButton;
    public Button closeButton;

    [Header("Scroll View Buttons")]
    public Button mainButton;
    public Button kitchenButton;

    [Header("Save/Load")]
    public bool enableSaveSystem = true;

    private GameObject[] shopPages;
    private GameObject[] scrollViews;

    public static ShopManager instance;
    private ShopItem currentShopItem;
    private List<GameObject> purchasedFurniture = new List<GameObject>();

    // ���̺� �ý��� ����
    private const string SHOP_SAVE_KEY = "ShopPurchaseData";

    private void Awake()
    {
        if (instance == null) { instance = this; }

        shopPages = new GameObject[] { mainRoomPage, FoodPage, toyPage, closetPage };
        scrollViews = new GameObject[] { mainScrollView, kitchenScrollView };

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        if (PurchaseAsk != null)
        {
            PurchaseAsk.SetActive(false);
        }
    }

    private void Start()
    {
        SetupButtons();

        if (enableSaveSystem)
        {
            LoadPurchaseData();
        }

        ShowScrollView(mainScrollView);
    }

    private void SetupButtons()
    {
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OpenShop);
        }

        mainRoomButton.onClick.AddListener(() => ShowPage(mainRoomPage));
        FoodButton.onClick.AddListener(() => ShowPage(FoodPage));
        toyButton.onClick.AddListener(() => ShowPage(toyPage));
        closetButton.onClick.AddListener(() => ShowPage(closetPage));

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        if (mainButton != null)
            mainButton.onClick.AddListener(() => ShowScrollView(mainScrollView));

        if (kitchenButton != null)
            kitchenButton.onClick.AddListener(() => ShowScrollView(kitchenScrollView));
    }

    public void AddPurchasedFurniture(GameObject furniture)
    {
        purchasedFurniture.Add(furniture);
        Debug.Log(furniture.name + "��(��) ���ŵ�!");
    }

    public List<GameObject> GetPurchasedFurniture()
    {
        return purchasedFurniture;
    }

    public void OpenShop()
    {
        // ���ι� UI ��Ȱ��ȭ
        if (MainRoomUIManager.Instance != null)
        {
            MainRoomUIManager.Instance.HideMainRoomUI();
        }

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            ShowPage(mainRoomPage);
            ShowScrollView(mainScrollView);
        }

        if (PurchaseAsk != null)
        {
            PurchaseAsk.SetActive(false);
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // ���ι� UI Ȱ��ȭ
        if (MainRoomUIManager.Instance != null)
        {
            MainRoomUIManager.Instance.ShowMainRoomUI();
        }
    }

    public void ShowPage(GameObject pageToShow)
    {
        foreach (GameObject page in shopPages)
        {
            page.SetActive(page == pageToShow);
        }
    }

    public void ShowScrollView(GameObject scrollViewToShow)
    {
        foreach (GameObject scroll in scrollViews)
        {
            scroll.SetActive(scroll == scrollViewToShow);
        }
    }

    public void RegisterFurnitureReplacement(GameObject oldFurniture, GameObject newFurniture)
    {
        if (!furnitureReplacementMap.ContainsKey(oldFurniture))
        {
            furnitureReplacementMap.Add(oldFurniture, newFurniture);
            newFurniture.SetActive(false);
        }
    }

    public void ReplaceFurniture(GameObject oldFurniture)
    {
        if (oldFurniture != null && furnitureReplacementMap.ContainsKey(oldFurniture))
        {
            GameObject newFurniture = furnitureReplacementMap[oldFurniture];
            if (oldFurniture.activeSelf)
            {
                oldFurniture.SetActive(false);
                newFurniture.SetActive(true);
                Debug.Log($"���� ��ü �Ϸ�: {oldFurniture.name} -> {newFurniture.name}");
            }
            else
            {
                Debug.LogWarning($"�̹� ��Ȱ��ȭ�� �����Դϴ�: {oldFurniture.name}");
            }
        }
        else
        {
            Debug.LogWarning($"��ü�� ������ ��ϵ��� �ʾҰų� �������� �ʽ��ϴ�: {oldFurniture}");
        }
    }

    public void AddNewFurniture(GameObject newFurniture)
    {
        if (newFurniture != null)
        {
            GameObject newObject = Instantiate(newFurniture, newFurniture.transform.position, Quaternion.identity);
            newObject.SetActive(true);
            Debug.Log($"{newObject.name} ������ �߰��Ǿ����ϴ�!");
        }
        else
        {
            Debug.LogWarning("�߰��� ������ �������� �ʾҽ��ϴ�.");
        }
    }

    public bool IsFurnitureRegistered(GameObject oldFurniture)
    {
        return furnitureReplacementMap.ContainsKey(oldFurniture);
    }

    public void SetCurrentFurniture(ShopItem shopItem)
    {
        currentShopItem = shopItem;
    }

    public void ConfirmPurchase()
    {
        if (currentShopItem != null)
        {
            currentShopItem.PurchaseItem();
            currentShopItem = null;
        }
    }

    // === ���̺�/�ε� �ý��� ===

    /// <summary>
    /// ���� �����͸� �����մϴ�
    /// </summary>
    public void SavePurchaseData()
    {
        if (!enableSaveSystem) return;

        ShopSaveData saveData = new ShopSaveData();

        // ���� �ִ� ��� ShopItem���� ���ŵ� �����۵� ����
        ShopItem[] allShopItems = FindObjectsOfType<ShopItem>();
        foreach (ShopItem item in allShopItems)
        {
            if (item.IsPurchased())
            {
                saveData.purchasedItems.Add(item.itemName);
            }
        }

        // JSON���� ��ȯ�Ͽ� ����
        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SHOP_SAVE_KEY, jsonData);
        PlayerPrefs.Save();

        Debug.Log($"���� ���� ������ ���� �Ϸ�: {saveData.purchasedItems.Count}�� ������");
    }

    /// <summary>
    /// ���� �����͸� �ҷ��ɴϴ�
    /// </summary>
    public void LoadPurchaseData()
    {
        if (!enableSaveSystem) return;

        string jsonData = PlayerPrefs.GetString(SHOP_SAVE_KEY, "");
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.Log("����� ���� �����Ͱ� �����ϴ�.");
            return;
        }

        try
        {
            ShopSaveData saveData = JsonUtility.FromJson<ShopSaveData>(jsonData);

            // ����� ���� ������ ShopItem�鿡 ����
            ShopItem[] allShopItems = FindObjectsOfType<ShopItem>();
            foreach (ShopItem item in allShopItems)
            {
                if (saveData.purchasedItems.Contains(item.itemName))
                {
                    item.SetPurchased(true);

                    // �̹� ���ŵ� ������� ��� ����
                    if (item.itemType == ItemType.MainRoomFurniture ||
                        item.itemType == ItemType.KitchenFurniture)
                    {
                        ApplyPurchasedFurniture(item);
                    }
                }
            }

            Debug.Log($"���� ���� ������ �ε� �Ϸ�: {saveData.purchasedItems.Count}�� ������");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���� ������ �ε� ����: {e.Message}");
        }
    }

    /// <summary>
    /// ���ŵ� ������ ���� �����մϴ�
    /// </summary>
    private void ApplyPurchasedFurniture(ShopItem item)
    {
        if (item.isReplaceable)
        {
            // ��ü�� ���� ó��
            if (item.oldFurniture != null && item.newFurniture != null)
            {
                item.oldFurniture.SetActive(false);

                // �� ������ �̹� ���� �ִٸ� Ȱ��ȭ, ���ٸ� ����
                GameObject existingNew = GameObject.Find(item.newFurniture.name);
                if (existingNew != null)
                {
                    existingNew.SetActive(true);
                }
                else
                {
                    GameObject newFurniture = Instantiate(item.newFurniture,
                        item.oldFurniture.transform.position,
                        item.oldFurniture.transform.rotation);
                    newFurniture.SetActive(true);
                }
            }
        }
        else
        {
            // �߰��� ���� ó��
            if (item.newFurniture != null)
            {
                Vector3 spawnPos = item.addPosition != Vector3.zero ?
                    item.addPosition : item.newFurniture.transform.position;

                // �̹� ���� ������ �ִ��� Ȯ�� �� ����
                string furnitureName = item.newFurniture.name + "(Clone)";
                if (GameObject.Find(furnitureName) == null)
                {
                    GameObject newFurniture = Instantiate(item.newFurniture, spawnPos,
                        item.newFurniture.transform.rotation);
                    newFurniture.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// ����� ���� ������ �ʱ�ȭ (�׽�Ʈ/����׿�)
    /// </summary>
    [ContextMenu("Reset Purchase Data")]
    public void ResetPurchaseData()
    {
        PlayerPrefs.DeleteKey(SHOP_SAVE_KEY);
        PlayerPrefs.Save();

        // ���� ���� ��� ShopItem �ʱ�ȭ
        ShopItem[] allShopItems = FindObjectsOfType<ShopItem>();
        foreach (ShopItem item in allShopItems)
        {
            item.SetPurchased(false);
        }

        Debug.Log("���� ���� �����Ͱ� �ʱ�ȭ�Ǿ����ϴ�.");
    }

    /// <summary>
    /// Ư�� �������� ���ŵǾ����� Ȯ��
    /// </summary>
    /// <param name="itemName">������ �̸�</param>
    /// <returns>���� ����</returns>
    public bool IsItemPurchased(string itemName)
    {
        string jsonData = PlayerPrefs.GetString(SHOP_SAVE_KEY, "");
        if (string.IsNullOrEmpty(jsonData)) return false;

        try
        {
            ShopSaveData saveData = JsonUtility.FromJson<ShopSaveData>(jsonData);
            return saveData.purchasedItems.Contains(itemName);
        }
        catch
        {
            return false;
        }
    }
}

// ���� ���� ������ ������ ���� Ŭ����
[System.Serializable]
public class ShopSaveData
{
    public List<string> purchasedItems = new List<string>();
}