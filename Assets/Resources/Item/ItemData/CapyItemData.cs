using UnityEngine;

public enum CapyItemType { Food, Toy }

[CreateAssetMenu(menuName = "Capybara/Item Data", fileName = "NewCapyItem")]
public class CapyItemData : ScriptableObject
{
    [Header("Identity")]
    public string id = "new_item";
    public string displayName = "New Item";
    public CapyItemType type = CapyItemType.Food;

    [Header("Visuals")]
    public Sprite icon;
    public Sprite previewSprite;

    [Header("Effects (+는 증가/회복)")]
    [Range(-100f, 100f)] public float hungerChange;
    [Range(-100f, 100f)] public float boredomChange;
    [Min(0)] public int expChange = 0;
}