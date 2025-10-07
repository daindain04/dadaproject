using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomChangeManager : MonoBehaviour
{
    [Header("방 오브젝트")]
    public GameObject mainFurniture;
    public GameObject kitchenFurniture;

    [Header("로딩 패널")]
    public GameObject loadingToKitchen;
    public GameObject loadingToMain;

    [Header("로딩 바")]
    public Image loadingBarToKitchen;
    public Image loadingBarToMain;

    [Header("이동 버튼")]
    public Button moveToKitchenButton;
    public Button moveToMainButton;

    [Header("로딩 시간")]
    public float loadingDuration = 5f;

    [Header("숨길 UI")]
    public GameObject buttonGroup;
    public GameObject coinBar;
    public GameObject gemBar;
    public GameObject expBar;
    public GameObject dropFoodToy;  // ⭐ 추가
    public GameObject profile;

    [Header("카피바라 걷기")]
    public RectTransform capyWalkToKitchen;
    public RectTransform capyWalkToMain;

    public Animator capyAnimatorToKitchen;
    public Animator capyAnimatorToMain;

    // 메인방 → 주방
    public Vector2 walkStartToKitchenPos;
    public Vector2 walkEndToKitchenPos;

    // 주방 → 메인방
    public Vector2 walkStartToMainPos;
    public Vector2 walkEndToMainPos;

    public MenuToggle menuToggle;

    [Header("레벨 제한")]
    public int requiredLevelForKitchen = 5;
    public GameObject kitchenLockPanel; // 주방 잠금 패널
    public Button backToMainFromLockButton; // 잠금 패널에서 메인방으로 돌아가는 버튼

    private void Start()
    {
        UpdateButtonStates();

        // 잠금 패널의 돌아가기 버튼 이벤트 연결
        if (backToMainFromLockButton != null)
        {
            backToMainFromLockButton.onClick.AddListener(ReturnToMainFromLock);
        }
    }

    // 메인방 → 주방
    public void MoveToKitchen()
    {
        // 레벨 체크
        if (!CanAccessKitchen())
        {
            StartCoroutine(ShowKitchenLockPanel());
        }
        else
        {
            StartCoroutine(TransitionToKitchen());
        }
    }

    // 주방 → 메인방
    public void MoveToMain()
    {
        StartCoroutine(TransitionToMain());
    }

    // 주방 접근 가능 여부 체크
    private bool CanAccessKitchen()
    {
        // 현재 레벨을 가져오는 로직
        // 경험치 관련 스크립트가 있다면 그것을 사용
        int currentLevel = GetCurrentLevel();
        return currentLevel >= requiredLevelForKitchen;
    }

    // 현재 레벨을 가져오는 함수
    private int GetCurrentLevel()
    {
        // MoneyManager 싱글톤 인스턴스에서 현재 레벨 가져오기
        if (MoneyManager.Instance != null)
        {
            return MoneyManager.Instance.level;
        }

        // MoneyManager가 없을 경우 기본값 1 반환
        Debug.LogWarning("MoneyManager 인스턴스를 찾을 수 없습니다. 기본 레벨 1을 반환합니다.");
        return 1;
    }

    // 주방 잠금 패널 표시
    private IEnumerator ShowKitchenLockPanel()
    {
        // 숨길 UI 끄기
        buttonGroup.SetActive(false);
        coinBar.SetActive(false);
        gemBar.SetActive(false);
        expBar.SetActive(false);
        dropFoodToy.SetActive(false);  // ⭐ 추가
        profile.SetActive(false);  // ⭐ 추가
       

    // 로딩 패널 먼저 보여주기
    loadingToKitchen.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        // Kitchen 쪽 카피바라 사용
        capyWalkToKitchen.localScale = new Vector3(-1, 1, 1);
        capyWalkToKitchen.anchoredPosition = walkStartToKitchenPos;
        capyAnimatorToKitchen.speed = 0.5f;
        loadingBarToKitchen.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);
            loadingBarToKitchen.fillAmount = t;
            capyWalkToKitchen.anchoredPosition = Vector2.Lerp(walkStartToKitchenPos, walkEndToKitchenPos, t);

            yield return null;
        }

        // 로딩 완료 후 주방으로 화면 전환
        mainFurniture.SetActive(false);
        kitchenFurniture.SetActive(true);
        loadingToKitchen.SetActive(false);

        // 주방 화면 위에 잠금 패널 표시
        kitchenLockPanel.SetActive(true);
    }

    // 잠금 패널에서 메인방으로 돌아가기
    public void ReturnToMainFromLock()
    {
        StartCoroutine(ReturnToMainFromLockCoroutine());
    }

    private IEnumerator ReturnToMainFromLockCoroutine()
    {
        // 잠금 패널 숨기기
        kitchenLockPanel.SetActive(false);

        // 메인방으로 돌아가는 로딩 시작
        loadingToMain.SetActive(true);

        // Main 쪽 카피바라 사용
        capyWalkToMain.localScale = new Vector3(1, 1, 1);
        capyWalkToMain.anchoredPosition = walkStartToMainPos;
        capyAnimatorToMain.speed = 0.5f;
        loadingBarToMain.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);
            loadingBarToMain.fillAmount = t;
            capyWalkToMain.anchoredPosition = Vector2.Lerp(walkStartToMainPos, walkEndToMainPos, t);

            yield return null;
        }

        // 로딩 완료 후 메인방으로 화면 전환
        kitchenFurniture.SetActive(false);
        mainFurniture.SetActive(true);
        loadingToMain.SetActive(false);

        // UI 다시 활성화
        buttonGroup.SetActive(true);
        coinBar.SetActive(true);
        gemBar.SetActive(true);
        expBar.SetActive(true);
        dropFoodToy.SetActive(true);  // ⭐ 추가
        profile.SetActive(true);  // ⭐ 추가

        UpdateButtonStates();
        GoToMainRoom();
    }

    // 메인방 → 주방 이동
    public void GoToKitchen()
    {
        // ... 로딩 처리 등
        menuToggle.SetRoom("Kitchen");
    }

    // 주방 → 메인방 이동
    public void GoToMainRoom()
    {
        // ... 로딩 처리 등
        menuToggle.SetRoom("Main");
    }

    private IEnumerator TransitionToKitchen()
    {
        // 숨길 UI 끄기
        buttonGroup.SetActive(false);
        coinBar.SetActive(false);
        gemBar.SetActive(false);
       // expBar.SetActive(false);
        dropFoodToy.SetActive(false);  // ⭐ 추가
        profile.SetActive(false);  // ⭐ 추가

        loadingToKitchen.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        // Kitchen 쪽 카피바라 사용
        capyWalkToKitchen.localScale = new Vector3(-1, 1, 1);
        capyWalkToKitchen.anchoredPosition = walkStartToKitchenPos;
        capyAnimatorToKitchen.speed = 0.5f;
        loadingBarToKitchen.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);
            loadingBarToKitchen.fillAmount = t;
            capyWalkToKitchen.anchoredPosition = Vector2.Lerp(walkStartToKitchenPos, walkEndToKitchenPos, t);

            yield return null;
        }

        mainFurniture.SetActive(false);
        kitchenFurniture.SetActive(true);
        loadingToKitchen.SetActive(false);

        buttonGroup.SetActive(true);
        coinBar.SetActive(true);
        gemBar.SetActive(true);
        expBar.SetActive(true);
        profile.SetActive(true);
        dropFoodToy.SetActive(true);

        UpdateButtonStates();
        GoToKitchen();
    }

    private IEnumerator TransitionToMain()
    {
        buttonGroup.SetActive(false);
        coinBar.SetActive(false);
        gemBar.SetActive(false);
        expBar.SetActive(false);
        dropFoodToy.SetActive(false);  // ⭐ 추가
        profile.SetActive(false);  // ⭐ 추가

        loadingToMain.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        // Main 쪽 카피바라 사용
        capyWalkToMain.localScale = new Vector3(1, 1, 1);
        capyWalkToMain.anchoredPosition = walkStartToMainPos;
        capyAnimatorToMain.speed = 0.5f;
        loadingBarToMain.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);
            loadingBarToMain.fillAmount = t;
            capyWalkToMain.anchoredPosition = Vector2.Lerp(walkStartToMainPos, walkEndToMainPos, t);

            yield return null;
        }

        kitchenFurniture.SetActive(false);
        mainFurniture.SetActive(true);
        loadingToMain.SetActive(false);

        // 메인방에서는 모든 UI 다시 켜기
        buttonGroup.SetActive(true);
        coinBar.SetActive(true);
        gemBar.SetActive(true);
        expBar.SetActive(true);
        dropFoodToy.SetActive(true);  // ⭐ 메인방에서는 다시 켜기
        profile.SetActive(true);  // ⭐ 메인방에서는 다시 켜기

        UpdateButtonStates();
        GoToMainRoom();
    }

    private void UpdateButtonStates()
    {
        if (mainFurniture.activeSelf)
        {
            moveToKitchenButton.gameObject.SetActive(true);
            moveToMainButton.gameObject.SetActive(false);
        }
        else if (kitchenFurniture.activeSelf)
        {
            moveToKitchenButton.gameObject.SetActive(false);
            moveToMainButton.gameObject.SetActive(true);
        }
    }
}