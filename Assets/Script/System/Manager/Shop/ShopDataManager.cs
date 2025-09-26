// 1. 스타트씬: ShopDataManager (데이터만 관리)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopDataManager : MonoBehaviour
{
    public static ShopDataManager Instance;

    [Header("Data Settings")]
    public bool enableSaveSystem = true;

    private List<int> purchasedItemIDs = new List<int>();
    private const string SHOP_SAVE_KEY = "ShopPurchaseData";
    private bool isPurchasing = false;

    public static System.Action<int> OnItemPurchased;
    public static System.Action OnShopDataLoaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPurchaseData();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 메인씬 로드시 FurnitureApplier에게 알림
        if (scene.name.Contains("Main") || scene.name.Contains("main"))
        {
            StartCoroutine(NotifyMainSceneLoaded());
        }
    }

    private System.Collections.IEnumerator NotifyMainSceneLoaded()
    {
        yield return new WaitForEndOfFrame();

        // FurnitureApplier 찾아서 가구 적용 요청
        FurnitureApplier applier = FindObjectOfType<FurnitureApplier>();
        if (applier != null)
        {
            applier.ApplyAllPurchasedFurniture(purchasedItemIDs);
        }
    }

    public bool PurchaseItem(int itemID, string itemName, int cost, bool useGems = false)
    {
        if (isPurchasing || IsItemPurchased(itemID))
        {
            return false;
        }

        isPurchasing = true;

        try
        {
            bool canAfford = false;
            if (useGems)
            {
                canAfford = MoneyManager.Instance?.SpendGems(cost) ?? false;
            }
            else
            {
                canAfford = MoneyManager.Instance?.SpendCoins(cost) ?? false;
            }

            if (canAfford && !purchasedItemIDs.Contains(itemID))
            {
                purchasedItemIDs.Add(itemID);

                // 메인씬의 FurnitureApplier에게 즉시 적용 요청
                FurnitureApplier applier = FindObjectOfType<FurnitureApplier>();
                if (applier != null)
                {
                    applier.ApplyFurnitureByItemID(itemID);
                }

                SavePurchaseData();
                OnItemPurchased?.Invoke(itemID);
                return true;
            }
            return false;
        }
        finally
        {
            isPurchasing = false;
        }
    }

    public bool IsItemPurchased(int itemID)
    {
        return purchasedItemIDs.Contains(itemID);
    }

    public List<int> GetPurchasedItemIDs()
    {
        return new List<int>(purchasedItemIDs);
    }

    // 세이브/로드 (기존과 동일)
    public void SavePurchaseData()
    {
        if (!enableSaveSystem) return;

        ShopSaveData saveData = new ShopSaveData();
        saveData.purchasedItemIDs = new List<int>(purchasedItemIDs);
        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SHOP_SAVE_KEY, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadPurchaseData()
    {
        if (!enableSaveSystem) return;

        string jsonData = PlayerPrefs.GetString(SHOP_SAVE_KEY, "");
        if (string.IsNullOrEmpty(jsonData))
        {
            OnShopDataLoaded?.Invoke();
            return;
        }

        try
        {
            ShopSaveData saveData = JsonUtility.FromJson<ShopSaveData>(jsonData);
            purchasedItemIDs = saveData.purchasedItemIDs ?? new List<int>();
            OnShopDataLoaded?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"상점 데이터 로드 실패: {e.Message}");
            purchasedItemIDs = new List<int>();
            OnShopDataLoaded?.Invoke();
        }
    }

    [ContextMenu("Reset Purchase Data")]
    public void ResetPurchaseData()
    {
        purchasedItemIDs.Clear();
        PlayerPrefs.DeleteKey(SHOP_SAVE_KEY);
        PlayerPrefs.Save();

        // 메인씬의 FurnitureApplier에게 리셋 요청
        FurnitureApplier applier = FindObjectOfType<FurnitureApplier>();
        if (applier != null)
        {
            applier.ResetAllFurniture();
        }
    }
}

