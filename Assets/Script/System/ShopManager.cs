using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Shop UI")]
    public GameObject shopPanel;
    public Button shopButton;

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


    public static ShopManager instance;
 

    private List<GameObject> purchasedFurniture = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;

        shopPages = new GameObject[] { mainRoomPage, FoodPage, toyPage, closetPage };


        scrollViews = new GameObject[] { mainScrollView, kitchenScrollView };


        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void Start()
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


        ShowScrollView(mainScrollView);
    }

    // 구매한 가구 추가하기
    public void AddPurchasedFurniture(GameObject furniture)
    {
        purchasedFurniture.Add(furniture);
        Debug.Log(furniture.name + "이(가) 구매됨!");
    }

    // 배치된 가구 리스트 반환
    public List<GameObject> GetPurchasedFurniture()
    {
        return purchasedFurniture;
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            ShowPage(mainRoomPage);
            ShowScrollView(mainScrollView);
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
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
}
