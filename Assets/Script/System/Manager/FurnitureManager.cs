using System.Collections.Generic;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public Transform room; // 가구를 배치할 공간
    public List<GameObject> furniturePrefabs; // 가구 프리팹 리스트

    private void Start()
    {
        PlacePurchasedFurniture();
    }

    // 구매한 가구 배치하기
    public void PlacePurchasedFurniture()
    {
        List<GameObject> purchasedFurniture = ShopManager.instance.GetPurchasedFurniture();

        foreach (GameObject furniture in purchasedFurniture)
        {
            GameObject newFurniture = Instantiate(furniture, room);
            newFurniture.SetActive(true);
        }
    }
}
