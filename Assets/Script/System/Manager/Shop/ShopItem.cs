using UnityEngine;
using UnityEngine.UI;

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
    public int itemID;
    public string itemName;
    public int price;
    public CurrencyType currencyType = CurrencyType.Coin;
    public ItemType itemType = ItemType.MainRoomFurniture;

    [Header("아이템 데이터 (음식/장난감용)")]
    public CapyItemData itemData;

    [Header("악세사리 정보 (Clothing용)")]
    public int accessoryIndex = -1; // 악세사리 배열에서의 인덱스 (0~8)

    [Header("UI 요소")]
    public Button itemButton;

    private bool isPurchased = false;

    // useGems 속성 (기존 호환성)
    public bool useGems => currencyType == CurrencyType.Gem;

    private void Start()
    {
        SetupButtons();
        // ShopDataManager 이벤트 구독
        ShopDataManager.OnShopDataLoaded += CheckPurchaseStatus;
        ShopDataManager.OnItemPurchased += OnItemPurchased;
        // 초기 구매 상태 확인
        CheckPurchaseStatus();
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        ShopDataManager.OnShopDataLoaded -= CheckPurchaseStatus;
        ShopDataManager.OnItemPurchased -= OnItemPurchased;
    }

    private void SetupButtons()
    {
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(OnItemButtonClicked);
        }
    }

    private void CheckPurchaseStatus()
    {
        if (ShopDataManager.Instance != null)
        {
            // 가구류와 악세사리는 구매 상태 확인 (음식/장난감은 반복 구매 가능)
            if (itemType == ItemType.MainRoomFurniture ||
                itemType == ItemType.KitchenFurniture ||
                itemType == ItemType.Clothing)
            {
                bool shouldBePurchased = ShopDataManager.Instance.IsItemPurchased(itemID);
                SetPurchased(shouldBePurchased);
            }
        }
    }

    private void OnItemPurchased(int purchasedItemID)
    {
        // 가구류와 악세사리는 구매 상태 업데이트 (음식/장난감은 반복 구매 가능)
        if (purchasedItemID == itemID &&
            (itemType == ItemType.MainRoomFurniture ||
             itemType == ItemType.KitchenFurniture ||
             itemType == ItemType.Clothing))
        {
            SetPurchased(true);
        }
    }

    public void OnItemButtonClicked()
    {
        // 가구류와 악세사리는 구매 상태 확인
        if (isPurchased &&
            (itemType == ItemType.MainRoomFurniture ||
             itemType == ItemType.KitchenFurniture ||
             itemType == ItemType.Clothing))
        {
            Debug.Log($"이미 구매한 아이템입니다: {itemName}");
            return;
        }

        // ShopObjectManager의 구매 확인 창 사용
        if (ShopObjectManager.Instance != null)
        {
            ShopObjectManager.Instance.ShowPurchaseConfirmation(this);
        }
        else
        {
            // ShopObjectManager가 없으면 직접 구매 처리
            PurchaseItem();
        }
    }

    public void PurchaseItem()
    {
        // 가구류와 악세사리는 구매 상태 확인
        if (isPurchased &&
            (itemType == ItemType.MainRoomFurniture ||
             itemType == ItemType.KitchenFurniture ||
             itemType == ItemType.Clothing))
        {
            Debug.Log($"이미 구매한 아이템입니다: {itemName}");
            return;
        }

        // 돈 있는지 확인
        bool hasEnoughMoney = false;
        if (currencyType == CurrencyType.Coin)
        {
            hasEnoughMoney = MoneyManager.Instance != null && MoneyManager.Instance.coins >= price;
        }
        else
        {
            hasEnoughMoney = MoneyManager.Instance != null && MoneyManager.Instance.gems >= price;
        }

        if (!hasEnoughMoney)
        {
            string currencyName = currencyType == CurrencyType.Coin ? "코인" : "보석";
            Debug.Log($"{currencyName}이 부족합니다!");
            return;
        }

        // 구매 처리
        switch (itemType)
        {
            case ItemType.Food:
            case ItemType.Toy:
                PurchaseFoodOrToy();
                break;
            case ItemType.Clothing:
                PurchaseClothing();
                break;
            default:
                PurchaseFurniture();
                break;
        }
    }

    private void PurchaseFoodOrToy()
    {
        if (itemData == null)
        {
            Debug.LogError($"아이템 데이터가 설정되지 않았습니다: {itemName}");
            return;
        }

        if (MoneyManager.Instance == null || Inventory.Instance == null)
        {
            Debug.LogError("MoneyManager 또는 Inventory가 없습니다!");
            return;
        }

        // 돈 차감
        if (currencyType == CurrencyType.Coin)
        {
            MoneyManager.Instance.SpendCoins(price);
        }
        else
        {
            MoneyManager.Instance.SpendGems(price);
        }

        // 인벤토리에 아이템 추가
        Inventory.Instance.Add(itemData, 1);

        Debug.Log($"{itemName}을(를) 구매했습니다! 인벤토리에 추가됨");
    }

    private void PurchaseClothing()
    {
        if (accessoryIndex < 0)
        {
            Debug.LogError($"악세사리 인덱스가 설정되지 않았습니다: {itemName}");
            return;
        }

        if (AccessoryManager.Instance == null)
        {
            Debug.LogError("AccessoryManager가 없습니다!");
            return;
        }

        if (ShopDataManager.Instance != null)
        {
            bool purchaseSuccess = ShopDataManager.Instance.PurchaseItem(
                itemID,
                itemName,
                price,
                currencyType == CurrencyType.Gem
            );

            if (purchaseSuccess)
            {
                // 악세사리 착용
                AccessoryManager.Instance.EquipAccessory(accessoryIndex);
                Debug.Log($"{itemName}을(를) 구매하고 착용했습니다!");
            }
            else
            {
                string currencyName = currencyType == CurrencyType.Coin ? "코인" : "보석";
                Debug.Log($"{currencyName}이 부족합니다!");
            }
        }
        else
        {
            Debug.LogError("ShopDataManager.Instance가 null입니다!");
        }
    }

    private void PurchaseFurniture()
    {
        if (ShopDataManager.Instance != null)
        {
            bool purchaseSuccess = ShopDataManager.Instance.PurchaseItem(
                itemID,
                itemName,
                price,
                currencyType == CurrencyType.Gem
            );

            if (!purchaseSuccess)
            {
                string currencyName = currencyType == CurrencyType.Coin ? "코인" : "보석";
                Debug.Log($"{currencyName}이 부족합니다!");
            }
        }
        else
        {
            Debug.LogError("ShopDataManager.Instance가 null입니다!");
        }
    }

    private void UpdatePurchaseState()
    {
        // 가구류와 악세사리는 구매 버튼 비활성화
        if (itemButton != null &&
            (itemType == ItemType.MainRoomFurniture ||
             itemType == ItemType.KitchenFurniture ||
             itemType == ItemType.Clothing))
        {
            itemButton.interactable = !isPurchased;
            // 버튼 색상 변경 (선택사항)
            var colors = itemButton.colors;
            colors.normalColor = isPurchased ? Color.gray : Color.white;
            itemButton.colors = colors;
        }
    }

    // 외부에서 구매 상태 설정
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