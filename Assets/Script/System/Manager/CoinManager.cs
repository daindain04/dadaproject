using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance; // 싱글톤 패턴

    [Header("코인 UI")]
    public TextMeshProUGUI[] coinTexts; // 여러 개의 코인 UI를 지원하도록 배열로 변경

    private int playerCoins;


    private void Awake()
    {
        PlayerPrefs.DeleteKey("PlayerCoins"); //  기존 코인 데이터 삭제 (테스트용)
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
        LoadCoins(); // 코인 불러오기
    }

    // 코인 저장
    private void SaveCoins()
    {
        PlayerPrefs.SetInt("PlayerCoins", playerCoins);
        PlayerPrefs.Save();
    }

    // 코인 불러오기
    private void LoadCoins()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 1000);
        UpdateCoinUI();
    }

    // 코인 추가
    public void AddCoins(int amount)
    {
        playerCoins += amount;
        SaveCoins();
        UpdateCoinUI();
    }

    // 코인 차감
    public bool SpendCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            SaveCoins();
            UpdateCoinUI();
            return true; // 성공적으로 차감됨
        }
        else
        {
            Debug.Log("코인이 부족합니다!");
            return false; // 코인이 부족함
        }
    }

    // 여러 개의 UI 업데이트
    private void UpdateCoinUI()
    {
        foreach (TextMeshProUGUI coinText in coinTexts)
        {
            if (coinText != null)
            {
                coinText.text = playerCoins.ToString();
            }
        }
    }
}
