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

    private void Start()
    {
        UpdateButtonStates();
    }

    // 메인방 → 주방
    public void MoveToKitchen()
    {
        StartCoroutine(TransitionToKitchen());
    }

    // 주방 → 메인방
    public void MoveToMain()
    {
        StartCoroutine(TransitionToMain());
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
        expBar.SetActive(false);

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

        UpdateButtonStates();
        GoToKitchen();
    }

    private IEnumerator TransitionToMain()
    {
        buttonGroup.SetActive(false);
        coinBar.SetActive(false);
        gemBar.SetActive(false);
        expBar.SetActive(false);

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

        buttonGroup.SetActive(true);
        coinBar.SetActive(true);
        gemBar.SetActive(true);
        expBar.SetActive(true);

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
