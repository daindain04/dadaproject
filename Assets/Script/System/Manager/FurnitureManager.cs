using System.Collections.Generic;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public Transform room; // ������ ��ġ�� ����
    public List<GameObject> furniturePrefabs; // ���� ������ ����Ʈ

    private void Start()
    {
        PlacePurchasedFurniture();
    }

    // ������ ���� ��ġ�ϱ�
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
