using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private readonly Dictionary<CapyItemData, int> counts = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetCount(CapyItemData item) =>
        counts.TryGetValue(item, out var c) ? c : 0;

    public void Add(CapyItemData item, int amount = 1)
    {
        if (!counts.ContainsKey(item)) counts[item] = 0;
        counts[item] += amount;
        OnInventoryChanged?.Invoke(item, counts[item]);
    }

    public bool TryConsume(CapyItemData item, int amount = 1)
    {
        if (GetCount(item) < amount) return false;
        counts[item] -= amount;
        OnInventoryChanged?.Invoke(item, counts[item]);
        return true;
    }

    public delegate void InventoryChanged(CapyItemData item, int newCount);
    public event InventoryChanged OnInventoryChanged;
}
