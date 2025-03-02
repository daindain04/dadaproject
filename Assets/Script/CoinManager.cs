using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance; // �̱��� ����

    [Header("���� UI")]
    public TextMeshProUGUI coinText;

    private int playerCoins;

    private void Awake()
    {
        // �̱��� ����
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
        LoadCoins(); // ���� �ҷ�����
    }

    // ���� ����
    private void SaveCoins()
    {
        PlayerPrefs.SetInt("PlayerCoins", playerCoins);
        PlayerPrefs.Save();
    }

    // ���� �ҷ�����
    private void LoadCoins()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        UpdateCoinUI();
    }

    // ���� �߰�
    public void AddCoins(int amount)
    {
        playerCoins += amount;
        SaveCoins();
        UpdateCoinUI();
    }

    // ���� ����
    public bool SpendCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            SaveCoins();
            UpdateCoinUI();
            return true; // ���������� ������
        }
        else
        {
            Debug.Log("������ �����մϴ�!");
            return false; // ������ ������
        }
    }

    // UI ������Ʈ
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = playerCoins.ToString();
        }
    }
}

