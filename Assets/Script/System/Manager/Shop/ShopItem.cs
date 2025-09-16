using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 화폐 타입 정의
public enum CurrencyType
{
    Coin,
    Gem
}

// 상점 아이템 타입 정의
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
    [Header("아이템 기본 정보")]
    public string itemName;
    public int price; // 가구 가격
    public CurrencyType currencyType = CurrencyType.Coin; // 코인 또는 보석
    public ItemType itemType = ItemType.MainRoomFurniture;

    [Header("UI 요소")]
    public GameObject purchasePanel;
    public Button yesButton;
    public Button noButton;
    public Button itemButton;
    public TextMeshProUGUI priceText;
    public Image currencyIcon;
    public GameObject soldOutMark; // 품절 표시

    [Header("가구 설정")]
    public bool isReplaceable; // 기존 가구 교체 여부 (true: 기존 가구 대체, false: 새 가구 추가)
    public GameObject oldFurniture; // 기존 가구 (교체형 가구일 경우)
    public GameObject newFurniture; // 새 가구
    public Vector3 addPosition; // 새 가구 추가 위치 (isReplaceable이 false일 때)

    [Header("화폐 아이콘")]
    public Sprite coinIcon;
    public Sprite gemIcon;

    private bool isPurchased = false;

    private void Start()
    {
        InitializeUI();
        SetupButtons();

        // ShopManager에 가구 등록 (교체형인 경우)
        if (ShopManager.instance != null && isReplaceable && oldFurniture != null && newFurniture != null)
        {
            ShopManager.instance.RegisterFurnitureReplacement(oldFurniture, newFurniture);
        }
    }

    private void InitializeUI()
    {
        // 가격 텍스트 설정
        if (priceText != null)
        {
            priceText.text = price.ToString();
        }

        // 화폐 아이콘 설정
        if (currencyIcon != null)
        {
            currencyIcon.sprite = currencyType == CurrencyType.Coin ? coinIcon : gemIcon;
        }

        // 구매 패널 초기 상태
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
            Debug.Log("이미 구매한 아이템입니다.");
            return;
        }

        if (purchasePanel != null)
        {
            // 패널을 열 때 현재 선택된 가구 정보를 업데이트
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
            Debug.Log("이미 구매한 아이템입니다.");
            ClosePurchasePanel();
            return;
        }

        // 화폐 차감 시도
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
            Debug.Log($"{itemName} 구매 성공!");
            ProcessPurchase();
            isPurchased = true;
            UpdatePurchaseState();
            ClosePurchasePanel();

            // 자동 저장 호출
            if (ShopManager.instance != null)
            {
                ShopManager.instance.SavePurchaseData();
            }
        }
        else
        {
            string currencyName = currencyType == CurrencyType.Coin ? "코인" : "보석";
            Debug.Log($"{currencyName}이 부족합니다!");
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
            // 기존 가구와 교체
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
            // 새 가구 추가
            if (newFurniture != null)
            {
                Vector3 spawnPos = addPosition != Vector3.zero ? addPosition : newFurniture.transform.position;
                GameObject newObj = Instantiate(newFurniture, spawnPos, newFurniture.transform.rotation);
                newObj.SetActive(true);
                Debug.Log($"새 가구 추가: {itemName}");
            }
        }
    }

    private void ProcessFoodPurchase()
    {
        // 음식 구매 처리 (향후 인벤토리 시스템과 연동)
        Debug.Log($"음식 {itemName}을 구매했습니다. (인벤토리 추가 예정)");

        // 여기에 음식 인벤토리 추가 로직 구현
        // FoodInventoryManager.instance.AddFood(itemName, 1);
    }

    private void ProcessToyPurchase()
    {
        // 장난감 구매 처리 (향후 인벤토리 시스템과 연동)
        Debug.Log($"장난감 {itemName}을 구매했습니다. (인벤토리 추가 예정)");

        // 여기에 장난감 인벤토리 추가 로직 구현
        // ToyInventoryManager.instance.AddToy(itemName, 1);
    }

    private void ProcessClothingPurchase()
    {
        // 옷 구매 처리 (향후 의상 시스템과 연동)
        Debug.Log($"옷 {itemName}을 구매했습니다. (의상 목록 추가 예정)");

        // 여기에 의상 시스템 연동 로직 구현
        // ClothingManager.instance.UnlockClothing(itemName);
    }

    private void UpdatePurchaseState()
    {
        // 구매 버튼 상태 업데이트
        if (itemButton != null)
        {
            itemButton.interactable = !isPurchased;
        }

        // 품절 표시 업데이트
        if (soldOutMark != null)
        {
            soldOutMark.SetActive(isPurchased);
        }
    }

    // 외부에서 구매 상태 설정 (세이브 로드 시 사용)
    public void SetPurchased(bool purchased)
    {
        isPurchased = purchased;
        UpdatePurchaseState();
    }

    // 구매 상태 확인
    public bool IsPurchased()
    {
        return isPurchased;
    }
}