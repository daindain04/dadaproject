using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int price; // 가구 가격
    public GameObject purchasePanel; // 구매 창 패널
   
    public Button yesButton;
    public Button noButton;
    public Button itemButton; //  가구 버튼 (추가됨)

    private void Start()
    {
        // 게임 시작 시 구매 창을 비활성화
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }

   

        // 버튼 클릭 이벤트 등록
        yesButton.onClick.AddListener(PurchaseItem);
        noButton.onClick.AddListener(ClosePurchasePanel);

        //  가구 버튼을 클릭하면 구매 창 열기
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OpenPurchasePanel);
        }
    }

    //  구매 창 열기
    public void OpenPurchasePanel()
    {
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
        }
    }

    //  구매 창 닫기
    public void ClosePurchasePanel()
    {
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }
    }

    //  구매 처리
    public void PurchaseItem()
    {
        if (CoinManager.instance.SpendCoins(price)) // 코인 차감
        {
            Debug.Log("구매 성공!");
            ClosePurchasePanel(); // 구매 창 닫기
        }
        else
        {
            Debug.Log("코인이 부족합니다!");
        }
    }
}
