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

    private GameObject[] shopPages;
    private GameObject[] scrollViews;

    public static ShopManager instance;
    private ShopItem currentShopItem;
    private List<GameObject> purchasedFurniture = new List<GameObject>();

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

        ShowScrollView(mainScrollView);
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
}
