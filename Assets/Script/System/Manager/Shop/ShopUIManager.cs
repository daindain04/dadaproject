using UnityEngine;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    [Header("상점이 열릴 때 숨길 UI들")]
    [SerializeField] private List<GameObject> uisToHideInShop = new List<GameObject>();

    [Header("상점 패널")]
    [SerializeField] private GameObject shopPanel;

    // UI들의 원래 활성화 상태를 저장
    private Dictionary<GameObject, bool> originalUIStates = new Dictionary<GameObject, bool>();

    private void Start()
    {
        // 시작할 때 각 UI의 원래 상태를 저장
        SaveOriginalUIStates();
    }

    /// <summary>
    /// 상점 열기 - 지정된 UI들을 숨김
    /// </summary>
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        // 지정된 UI들을 숨김
        foreach (GameObject ui in uisToHideInShop)
        {
            if (ui != null)
            {
                ui.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 상점 닫기 - 숨겼던 UI들을 다시 보이게 함
    /// </summary>
    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // 원래 상태로 UI들을 복원
        RestoreOriginalUIStates();
    }

    /// <summary>
    /// UI들의 원래 활성화 상태를 저장
    /// </summary>
    private void SaveOriginalUIStates()
    {
        originalUIStates.Clear();

        foreach (GameObject ui in uisToHideInShop)
        {
            if (ui != null)
            {
                originalUIStates[ui] = ui.activeInHierarchy;
            }
        }
    }

    /// <summary>
    /// UI들을 원래 상태로 복원
    /// </summary>
    private void RestoreOriginalUIStates()
    {
        foreach (GameObject ui in uisToHideInShop)
        {
            if (ui != null && originalUIStates.ContainsKey(ui))
            {
                ui.SetActive(originalUIStates[ui]);
            }
        }
    }

    /// <summary>
    /// 런타임에서 숨길 UI 추가
    /// </summary>
    /// <param name="uiToAdd">추가할 UI GameObject</param>
    public void AddUIToHideList(GameObject uiToAdd)
    {
        if (uiToAdd != null && !uisToHideInShop.Contains(uiToAdd))
        {
            uisToHideInShop.Add(uiToAdd);
            // 새로 추가된 UI의 현재 상태도 저장
            originalUIStates[uiToAdd] = uiToAdd.activeInHierarchy;
        }
    }

    /// <summary>
    /// 숨길 UI 목록에서 제거
    /// </summary>
    /// <param name="uiToRemove">제거할 UI GameObject</param>
    public void RemoveUIFromHideList(GameObject uiToRemove)
    {
        if (uiToRemove != null)
        {
            uisToHideInShop.Remove(uiToRemove);
            originalUIStates.Remove(uiToRemove);
        }
    }

    /// <summary>
    /// 상점이 현재 열려있는지 확인
    /// </summary>
    /// <returns>상점이 열려있으면 true</returns>
    public bool IsShopOpen()
    {
        return shopPanel != null && shopPanel.activeInHierarchy;
    }
}
