using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemManager : MonoBehaviour
{
    public static GemManager instance;

    [Header("보석 UI")]
    public TextMeshProUGUI[] gemTexts; // 여러 개의 보석 UI를 지원

    private int playerGems;

    private void Awake()
    {
        // 테스트용으로 기존 데이터 삭제하고 시작할 수 있음 (배포 시 제거)
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
        LoadGems(); // 보석 불러오기
    }

    // 보석 저장
    private void SaveGems()
    {
        PlayerPrefs.SetInt("PlayerGems", playerGems);
        PlayerPrefs.Save();
    }

    // 보석 불러오기
    private void LoadGems()
    {
        playerGems = PlayerPrefs.GetInt("PlayerGems", 50); // 기본값 50
        UpdateGemUI();
    }

    // 보석 추가
    public void AddGems(int amount)
    {
        playerGems += amount;
        SaveGems();
        UpdateGemUI();
    }

    // 보석 차감
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
            Debug.Log("보석이 부족합니다!");
            return false;
        }
    }

    // UI 업데이트
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

