using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameDataResetter : MonoBehaviour
{
    [Header("키 조합 설정")]
    [SerializeField] private KeyCode resetKey1 = KeyCode.LeftControl;
    [SerializeField] private KeyCode resetKey2 = KeyCode.LeftShift;
    [SerializeField] private KeyCode resetKey3 = KeyCode.Z;

    [Header("초기화 설정")]
    [SerializeField] private string startSceneName = "Start"; // 스타트 씬 이름
    [SerializeField] private bool showResetMessage = true;
    [SerializeField] private float messageDisplayTime = 2f;

    private bool isResetting = false;

    void Awake()
    {
        // 씬 전환 시에도 유지되도록 설정
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // 초기화 중이면 입력 무시
        if (isResetting) return;

        // Ctrl + Shift + Z 키 조합 체크
        if (Input.GetKey(resetKey1) && Input.GetKey(resetKey2) && Input.GetKeyDown(resetKey3))
        {
            StartCoroutine(ResetGameDataAndGoToStart());
        }
    }

    private IEnumerator ResetGameDataAndGoToStart()
    {
        isResetting = true;

        if (showResetMessage)
        {
            Debug.Log("=== 게임 데이터 초기화 시작 ===");
        }

        // 모든 데이터 초기화
        ResetAllGameData();

        // 잠시 대기 (저장 완료 보장)
        yield return new WaitForSeconds(0.1f);

        if (showResetMessage)
        {
            Debug.Log("=== 게임 데이터 초기화 완료 - 스타트 씬으로 이동 ===");
        }

        // 스타트 씬으로 이동
        SceneManager.LoadScene(startSceneName);

        isResetting = false;
    }

    [ContextMenu("모든 데이터 초기화")]
    public void ResetAllGameData()
    {
        try
        {
            // 1. 상점 구매 데이터 초기화
            if (ShopDataManager.Instance != null)
            {
                ShopDataManager.Instance.ResetPurchaseData();
                Debug.Log("✓ 상점 데이터 초기화 완료");
            }
            else
            {
                // ShopDataManager가 없어도 PlayerPrefs에서 직접 삭제
                PlayerPrefs.DeleteKey("ShopPurchaseData");
                Debug.Log("✓ 상점 데이터 PlayerPrefs에서 직접 삭제");
            }

            // 2. 플레이어 재화/경험치 데이터 초기화
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.ResetData();
                Debug.Log("✓ 플레이어 재화/경험치 데이터 초기화 완료");
            }
            else
            {
                // MoneyManager가 없어도 PlayerPrefs에서 직접 삭제
                PlayerPrefs.DeleteKey("PlayerData_Coins");
                PlayerPrefs.DeleteKey("PlayerData_Gems");
                PlayerPrefs.DeleteKey("PlayerData_TotalExperience");
                PlayerPrefs.DeleteKey("PlayerData_Level");
                Debug.Log("✓ 플레이어 재화/경험치 데이터 PlayerPrefs에서 직접 삭제");
            }

            // 3. 인벤토리 데이터 초기화
            if (Inventory.Instance != null)
            {
                Inventory.Instance.ResetData();
                Debug.Log("✓ 인벤토리 데이터 초기화 완료");
            }
            else
            {
                // Inventory가 없어도 PlayerPrefs에서 직접 삭제
                PlayerPrefs.DeleteKey("InventoryData");
                Debug.Log("✓ 인벤토리 데이터 PlayerPrefs에서 직접 삭제");
            }

            // 4. 플레이어 이름 초기화
            PlayerPrefs.DeleteKey("CapybaraName");
            Debug.Log("✓ 플레이어 이름 초기화 완료");

            // 5. PlayerPrefs 전체 저장
            PlayerPrefs.Save();
            Debug.Log("✓ 모든 데이터 저장 완료");

            Debug.Log("🎉 모든 게임 데이터가 성공적으로 초기화되었습니다!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 데이터 초기화 중 오류 발생: {e.Message}");
        }
    }

    [ContextMenu("PlayerPrefs 전체 삭제")]
    public void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🗑️ 모든 PlayerPrefs 데이터가 삭제되었습니다!");
    }

    [ContextMenu("스타트 씬으로 이동")]
    public void GoToStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }

    [ContextMenu("테스트: 초기화 + 스타트씬 이동")]
    public void TestResetAndGoToStart()
    {
        StartCoroutine(ResetGameDataAndGoToStart());
    }

}