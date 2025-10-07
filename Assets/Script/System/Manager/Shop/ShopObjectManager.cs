using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopObjectManager : MonoBehaviour
{
    [Header("Shop UI")]
    public GameObject shopPanel;
    public Button shopButton;
    public GameObject PurchaseAsk;

    [Header("Purchase Confirmation UI")]
    public Text itemNameText;
    public Text itemPriceText;
    public Button confirmButton;
    public Button cancelButton;

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
    private ShopItem currentShopItem;

    public static ShopObjectManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeUI();
    }

    private void Start()
    {
        SetupButtons();
        ShowScrollView(mainScrollView);

        // 이벤트 구독
        ShopDataManager.OnShopDataLoaded += UpdateAllShopItemsUI;
        ShopDataManager.OnItemPurchased += OnItemPurchased;

        // 초기 UI 업데이트
        UpdateAllShopItemsUI();
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        ShopDataManager.OnShopDataLoaded -= UpdateAllShopItemsUI;
        ShopDataManager.OnItemPurchased -= OnItemPurchased;
    }

    private void OnItemPurchased(int itemID)
    {
        // 구매 완료 후 UI 업데이트
        UpdateAllShopItemsUI();
    
    }

    private void InitializeUI()
    {
        // 상점 페이지 배열 초기화
        if (mainRoomPage != null && FoodPage != null && toyPage != null && closetPage != null)
        {
            shopPages = new GameObject[] { mainRoomPage, FoodPage, toyPage, closetPage };
        }

        // 스크롤뷰 배열 초기화
        if (mainScrollView != null && kitchenScrollView != null)
        {
            scrollViews = new GameObject[] { mainScrollView, kitchenScrollView };
        }

        // 초기 상태 설정
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        if (PurchaseAsk != null)
        {
            PurchaseAsk.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        // 상점 열기 버튼
        if (shopButton != null)
        {
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShop);
        }

        // 페이지 전환 버튼들
        if (mainRoomButton != null)
        {
            mainRoomButton.onClick.RemoveAllListeners();
            mainRoomButton.onClick.AddListener(() => ShowPage(mainRoomPage));
        }

        if (FoodButton != null)
        {
            FoodButton.onClick.RemoveAllListeners();
            FoodButton.onClick.AddListener(() => ShowPage(FoodPage));
        }

        if (toyButton != null)
        {
            toyButton.onClick.RemoveAllListeners();
            toyButton.onClick.AddListener(() => ShowPage(toyPage));
        }

        if (closetButton != null)
        {
            closetButton.onClick.RemoveAllListeners();
            closetButton.onClick.AddListener(() => ShowPage(closetPage));
        }

        // 상점 닫기 버튼
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseShop);
        }

        // 스크롤뷰 전환 버튼들
        if (mainButton != null)
        {
            mainButton.onClick.RemoveAllListeners();
            mainButton.onClick.AddListener(() => ShowScrollView(mainScrollView));
        }

        if (kitchenButton != null)
        {
            kitchenButton.onClick.RemoveAllListeners();
            kitchenButton.onClick.AddListener(() => ShowScrollView(kitchenScrollView));
        }

        // 구매 확인 버튼들
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmPurchase);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelPurchase);
        }
    }

    public void OpenShop()
    {
        // 메인룸 UI 숨기기
        if (MainRoomUIManager.Instance != null)
        {
            MainRoomUIManager.Instance.HideMainRoomUI();
        }

        // 상점 패널 열기
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            ShowPage(mainRoomPage);
            ShowScrollView(mainScrollView);
        }

        // 구매 확인창은 닫기
        if (PurchaseAsk != null)
        {
            PurchaseAsk.SetActive(false);
        }

        // 상점을 열 때마다 UI 갱신
        UpdateAllShopItemsUI();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // 메인룸 UI 다시 보이기
        if (MainRoomUIManager.Instance != null)
        {
            MainRoomUIManager.Instance.ShowMainRoomUI();
        }
    }

    public void ShowPage(GameObject pageToShow)
    {
        if (shopPages == null) return;

        foreach (GameObject page in shopPages)
        {
            if (page != null)
                page.SetActive(page == pageToShow);
        }
    }

    public void ShowScrollView(GameObject scrollViewToShow)
    {
        if (scrollViews == null) return;

        foreach (GameObject scroll in scrollViews)
        {
            if (scroll != null)
                scroll.SetActive(scroll == scrollViewToShow);
        }
    }

    public void ShowPurchaseConfirmation(ShopItem shopItem)
    {
        currentShopItem = shopItem;

        if (PurchaseAsk != null)
        {
            PurchaseAsk.SetActive(true);

            // 구매 확인창에 아이템 정보 표시
            UpdatePurchaseConfirmationUI(shopItem);
        }
    }

    private void UpdatePurchaseConfirmationUI(ShopItem shopItem)
    {
        if (itemNameText != null)
        {
            itemNameText.text = shopItem.itemName;
        }

        if (itemPriceText != null)
        {
            string currencySymbol = shopItem.currencyType == CurrencyType.Coin ? "코인" : "보석";
            itemPriceText.text = $"{shopItem.price} {currencySymbol}";
        }
    }

    public void ConfirmPurchase()
    {
        if (currentShopItem != null)
        {
            // ShopItem의 구매 메서드 호출
            currentShopItem.PurchaseItem();
        }

        HidePurchaseConfirmation();
    }

    public void CancelPurchase()
    {
        HidePurchaseConfirmation();
    }

    public void HidePurchaseConfirmation()
    {
        currentShopItem = null;
        if (PurchaseAsk != null)
        {
            PurchaseAsk.SetActive(false);
        }
    }

    public void UpdateAllShopItemsUI()
    {
        if (ShopDataManager.Instance == null) return;

        ShopItem[] allShopItems = FindObjectsOfType<ShopItem>();
        foreach (ShopItem item in allShopItems)
        {
            if (ShopDataManager.Instance.IsItemPurchased(item.itemID))
            {
                item.SetPurchased(true);
            }
        }

        Debug.Log($"모든 상점 아이템 UI 업데이트 완료 (총 {allShopItems.Length}개)");
    }

    public bool IsItemPurchased(int itemID)
    {
        if (ShopDataManager.Instance != null)
        {
            return ShopDataManager.Instance.IsItemPurchased(itemID);
        }
        return false;
    }

    // 디버그 기능
    [ContextMenu("Reset Purchase Data")]
    public void ResetPurchaseData()
    {
        if (ShopDataManager.Instance != null)
        {
            ShopDataManager.Instance.ResetPurchaseData();
            UpdateAllShopItemsUI();
        }
    }

    [ContextMenu("Refresh All UI")]
    public void RefreshAllUI()
    {
        UpdateAllShopItemsUI();
        Debug.Log("모든 상점 UI 새로고침 완료");
    }
}