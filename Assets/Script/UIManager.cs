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

        nameText.text = $"ī�ǹٶ�: {data.capybaraName}";
        goldText.text = $"���: {data.gold}";
        gemsText.text = $"����: {data.gems}";
        hungerText.text = $"�����: {data.hunger:F1}";
        boredomText.text = $"������: {data.boredom:F1}";
        happinessText.text = $"�ູ��: {data.happiness:F1}";
    }
}
