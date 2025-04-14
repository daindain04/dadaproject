using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int price; // 가구 가격
    public GameObject purchasePanel;
    public Button yesButton;
    public Button noButton;
    public Button itemButton;

    [Header("가구 설정")]
    public bool isReplaceable; // 기존 가구 교체 여부 (true: 기존 가구 대체, false: 새 가구 추가)
    public GameObject oldFurniture; // 기존 가구 (교체형 가구일 경우)
    public GameObject newFurniture; // 새 가구

    private void Start()
    {
        if (purchasePanel != null)
        {
            ShopManager.instance.SetCurrentFurniture(this);
            purchasePanel.SetActive(true);
        }

        yesButton.onClick.AddListener(PurchaseItem);
        noButton.onClick.AddListener(ClosePurchasePanel);

        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OpenPurchasePanel);
        }

        if (ShopManager.instance != null && isReplaceable && oldFurniture != null && newFurniture != null)
        {
            ShopManager.instance.RegisterFurnitureReplacement(oldFurniture, newFurniture);
        }
    }

    public void OpenPurchasePanel()
    {
        if (purchasePanel != null)
        {
            //  패널을 열 때 현재 선택된 가구 정보를 업데이트
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
        if (CoinManager.instance.SpendCoins(price)) // 코인 차감
        {
            Debug.Log("구매 성공!");

            if (isReplaceable)
            {
                //  구매할 때만 특정 가구를 등록 & 교체 실행
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
                // 새 가구를 추가하는 경우 (개별 가구만 추가)
                if (ShopManager.instance != null)
                {
                    ShopManager.instance.AddNewFurniture(newFurniture);
                }
            }

            ClosePurchasePanel();
        }
        else
        {
            Debug.Log("코인이 부족합니다!");
        }
    }
}
