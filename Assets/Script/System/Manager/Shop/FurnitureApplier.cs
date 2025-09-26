// 2. 메인씬: FurnitureApplier (GameObject 참조 관리)
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnitureMapping
{
    public int itemID;
    public string itemName;
    public GameObject oldFurniture;
    public GameObject newFurniture;
    public bool isReplacement = true;
}

public class FurnitureApplier : MonoBehaviour
{
    [Header("가구 매핑 설정")]
    public List<FurnitureMapping> furnitureMappings = new List<FurnitureMapping>();

    private void Start()
    {
        // ShopDataManager가 준비되면 가구 적용
        if (ShopDataManager.Instance != null)
        {
            var purchasedIDs = ShopDataManager.Instance.GetPurchasedItemIDs();
            ApplyAllPurchasedFurniture(purchasedIDs);
        }
    }

    public void ApplyAllPurchasedFurniture(List<int> purchasedItemIDs)
    {
        Debug.Log($"가구 적용 시작: {purchasedItemIDs.Count}개");

        foreach (int itemID in purchasedItemIDs)
        {
            ApplyFurnitureByItemID(itemID);
        }
    }

    public void ApplyFurnitureByItemID(int itemID)
    {
        FurnitureMapping mapping = furnitureMappings.Find(m => m.itemID == itemID);
        if (mapping == null) return;

        if (mapping.isReplacement)
        {
            if (mapping.oldFurniture != null)
                mapping.oldFurniture.SetActive(false);
            if (mapping.newFurniture != null)
                mapping.newFurniture.SetActive(true);
        }
        else
        {
            if (mapping.newFurniture != null)
                mapping.newFurniture.SetActive(true);
        }

        Debug.Log($"가구 적용 완료: {mapping.itemName}");
    }

    public void ResetAllFurniture()
    {
        foreach (FurnitureMapping mapping in furnitureMappings)
        {
            if (mapping.isReplacement)
            {
                if (mapping.oldFurniture != null)
                    mapping.oldFurniture.SetActive(true);
                if (mapping.newFurniture != null)
                    mapping.newFurniture.SetActive(false);
            }
            else
            {
                if (mapping.newFurniture != null)
                    mapping.newFurniture.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public class ShopSaveData
{
    public List<int> purchasedItemIDs = new List<int>();
}