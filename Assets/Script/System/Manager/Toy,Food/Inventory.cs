using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    private readonly Dictionary<CapyItemData, int> counts = new();

    [Header("저장 설정")]
    public string saveFileName = "InventoryData";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 게임 시작 시 데이터 로드
        LoadData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveData(); // 앱이 일시정지될 때 저장
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveData(); // 앱이 포커스를 잃을 때 저장
    }

    private void OnDestroy()
    {
        SaveData(); // 오브젝트가 파괴될 때 저장
    }

    public int GetCount(CapyItemData item) =>
        counts.TryGetValue(item, out var c) ? c : 0;

    public void Add(CapyItemData item, int amount = 1)
    {
        if (!counts.ContainsKey(item)) counts[item] = 0;
        counts[item] += amount;
        OnInventoryChanged?.Invoke(item, counts[item]);
        SaveData(); // 아이템 추가 시 저장
        Debug.Log($"인벤토리 추가: {item.displayName} x{amount}, 총 {counts[item]}개");
    }

    public bool TryConsume(CapyItemData item, int amount = 1)
    {
        if (GetCount(item) < amount) return false;
        counts[item] -= amount;
        OnInventoryChanged?.Invoke(item, counts[item]);
        SaveData(); // 아이템 소모 시 저장
        Debug.Log($"인벤토리 소모: {item.displayName} x{amount}, 남은 {counts[item]}개");
        return true;
    }

    public delegate void InventoryChanged(CapyItemData item, int newCount);
    public event InventoryChanged OnInventoryChanged;

    #region 저장/로드 시스템

    public void SaveData()
    {
        // 아이템별 수량을 ID:수량 형태로 저장
        List<string> itemDataList = new List<string>();

        foreach (var kvp in counts)
        {
            if (kvp.Key != null && kvp.Value > 0)
            {
                string itemEntry = $"{kvp.Key.id}:{kvp.Value}";
                itemDataList.Add(itemEntry);
            }
        }

        // List를 하나의 문자열로 합치기 (구분자: |)
        string saveString = string.Join("|", itemDataList);
        PlayerPrefs.SetString(saveFileName, saveString);
        PlayerPrefs.Save();

        Debug.Log($"인벤토리 저장 완료: {itemDataList.Count}개 아이템");
    }

    public void LoadData()
    {
        counts.Clear();

        string loadString = PlayerPrefs.GetString(saveFileName, "");
        if (string.IsNullOrEmpty(loadString))
        {
            Debug.Log("저장된 인벤토리 데이터 없음");
            return;
        }

        // 저장된 문자열을 파싱
        string[] itemEntries = loadString.Split('|');

        foreach (string entry in itemEntries)
        {
            if (string.IsNullOrEmpty(entry)) continue;

            string[] parts = entry.Split(':');
            if (parts.Length == 2)
            {
                string itemId = parts[0];
                if (int.TryParse(parts[1], out int count))
                {
                    // ID로 아이템 찾기
                    CapyItemData item = FindItemById(itemId);
                    if (item != null)
                    {
                        counts[item] = count;
                        Debug.Log($"인벤토리 로드: {item.displayName} x{count}");
                    }
                    else
                    {
                        Debug.LogWarning($"ID를 찾을 수 없음: {itemId}");
                    }
                }
            }
        }

        Debug.Log($"인벤토리 로드 완료: {counts.Count}개 아이템");
    }

    private CapyItemData FindItemById(string id)
    {
        // 모든 CapyItemData 에셋을 찾아서 ID 매칭
        CapyItemData[] allItems = Resources.FindObjectsOfTypeAll<CapyItemData>();

        foreach (CapyItemData item in allItems)
        {
            if (item.id == id)
            {
                return item;
            }
        }

        return null;
    }

    public void ResetData()
    {
        counts.Clear();
        PlayerPrefs.DeleteKey(saveFileName);
        PlayerPrefs.Save();
        Debug.Log("인벤토리 데이터 리셋 완료");

        // 모든 아이템의 변경 이벤트 발생
        OnInventoryChanged?.Invoke(null, 0);
    }

    #endregion

    #region 디버그 기능

    [ContextMenu("디버그: 인벤토리 저장")]
    public void DebugSaveData()
    {
        SaveData();
    }

    [ContextMenu("디버그: 인벤토리 로드")]
    public void DebugLoadData()
    {
        LoadData();
    }

    [ContextMenu("디버그: 인벤토리 리셋")]
    public void DebugResetData()
    {
        ResetData();
    }

    #endregion
}
