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

    // 세이브 시스템 관련
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
        Debug.Log(furniture.name + "이(가) 구매됨!");
    }

    public List<GameObject> GetPurchasedFurniture()
    {
        return purchasedFurniture;
    }

    public void OpenShop()
    {
        // 메인방 UI 비활성화
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

        // 메인방 UI 활성화
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
                Debug.Log($"가구 교체 완료: {oldFurniture.name} -> {newFurniture.name}");
            }
            else
            {
                Debug.LogWarning($"이미 비활성화된 가구입니다: {oldFurniture.name}");
            }
        }
        else
        {
            Debug.LogWarning($"교체할 가구가 등록되지 않았거나 존재하지 않습니다: {oldFurniture}");
        }
    }

    public void AddNewFurniture(GameObject newFurniture)
    {
        if (newFurniture != null)
        {
            GameObject newObject = Instantiate(newFurniture, newFurniture.transform.position, Quaternion.identity);
            newObject.SetActive(true);
            Debug.Log($"{newObject.name} 가구가 추가되었습니다!");
        }
        else
        {
            Debug.LogWarning("추가할 가구가 설정되지 않았습니다.");
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

    // === 세이브/로드 시스템 ===

    /// <summary>
    /// 구매 데이터를 저장합니다
    /// </summary>
    public void SavePurchaseData()
    {
        if (!enableSaveSystem) return;

        ShopSaveData saveData = new ShopSaveData();

        // 씬에 있는 모든 ShopItem에서 구매된 아이템들 수집
        ShopItem[] allShopItems = FindObjectsOfType<ShopItem>();
        foreach (ShopItem item in allShopItems)
        {
            if (item.IsPurchased())
            {
                saveData.purchasedItems.Add(item.itemName);
            }
        }

        // JSON으로 변환하여 저장
        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SHOP_SAVE_KEY, jsonData);
        PlayerPrefs.Save();

        Debug.Log($"상점 구매 데이터 저장 완료: {saveData.purchasedItems.Count}개 아이템");
    }

    /// <summary>
    /// 구매 데이터를 불러옵니다
    /// </summary>
    public void LoadPurchaseData()
    {
        if (!enableSaveSystem) return;

        string jsonData = PlayerPrefs.GetString(SHOP_SAVE_KEY, "");
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.Log("저장된 상점 데이터가 없습니다.");
            return;
        }

        try
        {
            ShopSaveData saveData = JsonUtility.FromJson<ShopSaveData>(jsonData);

            // 저장된 구매 정보를 ShopItem들에 적용
            ShopItem[] allShopItems = FindObjectsOfType<ShopItem>();
            foreach (ShopItem item in allShopItems)
            {
                if (saveData.purchasedItems.Contains(item.itemName))
                {
                    item.SetPurchased(true);

                    // 이미 구매된 가구라면 즉시 적용
                    if (item.itemType == ItemType.MainRoomFurniture ||
                        item.itemType == ItemType.KitchenFurniture)
                    {
                        ApplyPurchasedFurniture(item);
                    }
                }
            }

            Debug.Log($"상점 구매 데이터 로드 완료: {saveData.purchasedItems.Count}개 아이템");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"상점 데이터 로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 구매된 가구를 씬에 적용합니다
    /// </summary>
    private void ApplyPurchasedFurniture(ShopItem item)
    {
        if (item.isReplaceable)
        {
            // 교체형 가구 처리
            if (item.oldFurniture != null && item.newFurniture != null)
            {
                item.oldFurniture.SetActive(false);

                // 새 가구가 이미 씬에 있다면 활성화, 없다면 생성
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
            // 추가형 가구 처리
            if (item.newFurniture != null)
            {
                Vector3 spawnPos = item.addPosition != Vector3.zero ?
                    item.addPosition : item.newFurniture.transform.position;

                // 이미 같은 가구가 있는지 확인 후 생성
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
    /// 저장된 구매 데이터 초기화 (테스트/디버그용)
    /// </summary>
    [ContextMenu("Reset Purchase Data")]
    public void ResetPurchaseData()
    {
        PlayerPrefs.DeleteKey(SHOP_SAVE_KEY);
        PlayerPrefs.Save();

        // 현재 씬의 모든 ShopItem 초기화
        ShopItem[] allShopItems = FindObjectsOfType<ShopItem>();
        foreach (ShopItem item in allShopItems)
        {
            item.SetPurchased(false);
        }

        Debug.Log("상점 구매 데이터가 초기화되었습니다.");
    }

    /// <summary>
    /// 특정 아이템이 구매되었는지 확인
    /// </summary>
    /// <param name="itemName">아이템 이름</param>
    /// <returns>구매 여부</returns>
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

// 상점 구매 데이터 저장을 위한 클래스
[System.Serializable]
public class ShopSaveData
{
    public List<string> purchasedItems = new List<string>();
}