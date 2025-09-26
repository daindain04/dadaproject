using System.Collections;
using UnityEngine;

public class MainSceneInitializer : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("MainSceneInitializer 실행됨");
        StartCoroutine(InitializeMainScene());
    }

    private IEnumerator InitializeMainScene()
    {
        // ShopDataManager가 준비될 때까지 대기
        while (ShopDataManager.Instance == null)
        {
            Debug.Log("ShopDataManager 대기 중...");
            yield return new WaitForSeconds(0.1f);
        }

        // FurnitureApplier가 준비될 때까지 대기
        FurnitureApplier furnitureApplier = null;
        while (furnitureApplier == null)
        {
            furnitureApplier = FindObjectOfType<FurnitureApplier>();
            if (furnitureApplier == null)
            {
                Debug.Log("FurnitureApplier 대기 중...");
                yield return new WaitForSeconds(0.1f);
            }
        }

        // 한 프레임 대기 (모든 오브젝트가 완전히 로드되도록)
        yield return new WaitForEndOfFrame();

        Debug.Log("메인씬 가구 적용 시작");

        // FurnitureApplier를 통해 가구 적용
        var purchasedIDs = ShopDataManager.Instance.GetPurchasedItemIDs();
        furnitureApplier.ApplyAllPurchasedFurniture(purchasedIDs);

        // ShopObjectManager UI 업데이트
        if (ShopObjectManager.Instance != null)
        {
            yield return new WaitForEndOfFrame();
            ShopObjectManager.Instance.UpdateAllShopItemsUI();
        }

        Debug.Log("메인씬 초기화 완료");
    }
}