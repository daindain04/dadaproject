using UnityEngine;

public class ItemPanelRefresher : MonoBehaviour
{
    [Header("참조")]
    public ItemTabManager itemTabManager;

    private void OnEnable()
    {
        Debug.Log("=== ItemPanel 활성화됨 ===");

        if (itemTabManager != null)
        {
            itemTabManager.ForceRefresh();
        }
        else
        {
            Debug.LogError("ItemTabManager 참조가 없습니다!");
        }
    }
}