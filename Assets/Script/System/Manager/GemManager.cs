using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemManager : MonoBehaviour
{
    public static GemManager instance;

    [Header("���� UI")]
    public TextMeshProUGUI[] gemTexts; // ���� ���� ���� UI�� ����

    private int playerGems;

    private void Awake()
    {
        // �׽�Ʈ������ ���� ������ �����ϰ� ������ �� ���� (���� �� ����)
        // PlayerPrefs.DeleteKey("PlayerGems");

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadGems(); // ���� �ҷ�����
    }

    // ���� ����
    private void SaveGems()
    {
        PlayerPrefs.SetInt("PlayerGems", playerGems);
        PlayerPrefs.Save();
    }

    // ���� �ҷ�����
    private void LoadGems()
    {
        playerGems = PlayerPrefs.GetInt("PlayerGems", 50); // �⺻�� 50
        UpdateGemUI();
    }

    // ���� �߰�
    public void AddGems(int amount)
    {
        playerGems += amount;
        SaveGems();
        UpdateGemUI();
    }

    // ���� ����
    public bool SpendGems(int amount)
    {
        if (playerGems >= amount)
        {
            playerGems -= amount;
            SaveGems();
            UpdateGemUI();
            return true;
        }
        else
        {
            Debug.Log("������ �����մϴ�!");
            return false;
        }
    }

    // UI ������Ʈ
    private void UpdateGemUI()
    {
        foreach (TextMeshProUGUI gemText in gemTexts)
        {
            if (gemText != null)
            {
                gemText.text = playerGems.ToString();
            }
        }
    }
}

