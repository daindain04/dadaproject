using UnityEngine;

// 이 스크립트는 새로운 시스템에서는 FurnitureApplier로 대체됩니다.
// 만약 특별한 가구 배치 로직이 필요하다면 아래와 같이 수정할 수 있습니다.

public class FurnitureManager : MonoBehaviour
{
    [Header("Debug Info")]
    public bool showDebugInfo = true;

    private void Start()
    {
        if (showDebugInfo)
        {
            Debug.Log("FurnitureManager: 새로운 시스템에서는 FurnitureApplier가 가구 관리를 담당합니다.");

            // 현재 구매된 아이템 목록 출력 (디버그용)
            if (ShopDataManager.Instance != null)
            {
                var purchasedIDs = ShopDataManager.Instance.GetPurchasedItemIDs();
                Debug.Log($"현재 구매된 아이템: [{string.Join(", ", purchasedIDs)}]");
            }
        }
    }

    // 이 메서드는 더 이상 사용되지 않습니다.
    [System.Obsolete("PlacePurchasedFurniture는 더 이상 사용되지 않습니다. FurnitureApplier를 사용하세요.")]
    public void PlacePurchasedFurniture()
    {
        Debug.LogWarning("PlacePurchasedFurniture는 더 이상 사용되지 않습니다. FurnitureApplier가 자동으로 가구를 관리합니다.");
    }

    // 필요하다면 특별한 가구 관련 로직을 여기에 추가할 수 있습니다.
    // 예: 가구 애니메이션, 특수 효과 등
}