using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameExitManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject exitPanel; // 종료 확인 패널
    [SerializeField] private Button exitButton; // 종료 버튼
    [SerializeField] private Button cancelButton; // 돌아가기 버튼

    [Header("ESC 키 설정")]
    [SerializeField] private float holdTime = 3f; // ESC 키를 눌러야 하는 시간 (초)

    private float escHoldTimer = 0f;
    private bool isHoldingEsc = false;

    private void Awake()
    {
        // 시작 시 패널 비활성화
        if (exitPanel != null)
        {
            exitPanel.SetActive(false);
        }

        // 버튼 이벤트 연결
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
    }

    private void Update()
    {
        // 패널이 이미 열려있으면 ESC 입력 무시
        if (exitPanel != null && exitPanel.activeSelf)
        {
            return;
        }

        // ESC 키를 누르고 있는지 확인
        if (Input.GetKey(KeyCode.Escape))
        {
            if (!isHoldingEsc)
            {
                isHoldingEsc = true;
                escHoldTimer = 0f;
            }

            escHoldTimer += Time.deltaTime;

            // 3초 동안 누르면 종료 패널 표시
            if (escHoldTimer >= holdTime)
            {
                ShowExitPanel();
                isHoldingEsc = false;
                escHoldTimer = 0f;
            }
        }
        else
        {
            // ESC 키를 놓으면 타이머 리셋
            if (isHoldingEsc)
            {
                isHoldingEsc = false;
                escHoldTimer = 0f;
            }
        }
    }

    // 종료 패널 표시
    private void ShowExitPanel()
    {
        if (exitPanel != null)
        {
            exitPanel.SetActive(true);
            Debug.Log("종료 확인 패널이 열렸습니다.");
        }
    }

    // 종료 버튼 클릭 시
    private void OnExitButtonClicked()
    {
        Debug.Log("게임을 종료합니다. 모든 데이터를 초기화합니다...");

        // 모든 게임 데이터 초기화
        ResetAllGameData();

        // 게임 종료
        StartCoroutine(QuitGame());
    }

    // 돌아가기 버튼 클릭 시
    private void OnCancelButtonClicked()
    {
        if (exitPanel != null)
        {
            exitPanel.SetActive(false);
            Debug.Log("종료를 취소했습니다.");
        }
    }

    // 모든 게임 데이터 초기화 (GameDataResetter와 동일)
    private void ResetAllGameData()
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
                PlayerPrefs.DeleteKey("ShopPurchaseData");
                Debug.Log("✓ 상점 데이터 PlayerPrefs에서 직접 삭제");
            }

            // 2. 악세사리 데이터 초기화
            if (AccessoryManager.Instance != null)
            {
                AccessoryManager.Instance.ResetAllAccessories();
                Debug.Log("✓ 악세사리 데이터 초기화 완료");
            }
            else
            {
                PlayerPrefs.DeleteKey("CurrentAccessory");
                Debug.Log("✓ 악세사리 데이터 PlayerPrefs에서 직접 삭제");
            }

            // 3. 플레이어 재화/경험치 데이터 초기화
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.ResetData();
                Debug.Log("✓ 플레이어 재화/경험치 데이터 초기화 완료");
            }
            else
            {
                PlayerPrefs.DeleteKey("PlayerData_Coins");
                PlayerPrefs.DeleteKey("PlayerData_Gems");
                PlayerPrefs.DeleteKey("PlayerData_TotalExperience");
                PlayerPrefs.DeleteKey("PlayerData_Level");
                Debug.Log("✓ 플레이어 재화/경험치 데이터 PlayerPrefs에서 직접 삭제");
            }

            // 4. 인벤토리 데이터 초기화
            if (Inventory.Instance != null)
            {
                Inventory.Instance.ResetData();
                Debug.Log("✓ 인벤토리 데이터 초기화 완료");
            }
            else
            {
                PlayerPrefs.DeleteKey("InventoryData");
                Debug.Log("✓ 인벤토리 데이터 PlayerPrefs에서 직접 삭제");
            }

            // 5. 플레이어 이름 초기화
            PlayerPrefs.DeleteKey("CapybaraName");
            Debug.Log("✓ 플레이어 이름 초기화 완료");

            // 6. PlayerPrefs 전체 저장
            PlayerPrefs.Save();
            Debug.Log("✓ 모든 데이터 저장 완료");

            Debug.Log("=== 모든 게임 데이터가 초기화되었습니다 ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 초기화 중 오류 발생: {e.Message}");
        }
    }

    // 게임 종료
    private IEnumerator QuitGame()
    {
        // 잠시 대기 (데이터 저장 완료 보장)
        yield return new WaitForSeconds(0.2f);

        Debug.Log("게임을 종료합니다...");

#if UNITY_EDITOR
        // 에디터에서는 플레이 모드 중지
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // 버튼 이벤트 해제
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }
    }
}