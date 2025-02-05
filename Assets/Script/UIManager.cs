using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text goldText;
    public TMP_Text gemsText;
    public TMP_Text hungerText;
    public TMP_Text boredomText;
    public TMP_Text happinessText;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        GameData data = GameDataManager.Instance.data;

        nameText.text = $"카피바라: {data.capybaraName}";
        goldText.text = $"골드: {data.gold}";
        gemsText.text = $"보석: {data.gems}";
        hungerText.text = $"배고픔: {data.hunger:F1}";
        boredomText.text = $"지루함: {data.boredom:F1}";
        happinessText.text = $"행복도: {data.happiness:F1}";
    }
}
