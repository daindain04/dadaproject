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

    [Header("카피바라 걷기")]
    public RectTransform capyWalk;
    public Animator capyAnimator;

    // 메인방 → 주방
    public Vector2 walkStartToKitchenPos;
    public Vector2 walkEndToKitchenPos;

    // 주방 → 메인방
    public Vector2 walkStartToMainPos;
    public Vector2 walkEndToMainPos;

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

    private IEnumerator TransitionToKitchen()
    {
        loadingToKitchen.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        capyWalk.localScale = new Vector3(-1, 1, 1); // ← 방향

        if (capyWalk != null)
            capyWalk.anchoredPosition = walkStartToKitchenPos;
        if (capyAnimator != null)
            capyAnimator.speed = 0.5f; // 애니메이션 느리게
        if (loadingBarToKitchen != null)
            loadingBarToKitchen.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);

            if (loadingBarToKitchen != null)
                loadingBarToKitchen.fillAmount = t;

            if (capyWalk != null)
                capyWalk.anchoredPosition = Vector2.Lerp(walkStartToKitchenPos, walkEndToKitchenPos, t);

            yield return null;
        }

        mainFurniture.SetActive(false);
        kitchenFurniture.SetActive(true);
        loadingToKitchen.SetActive(false);

        UpdateButtonStates();
    }

    private IEnumerator TransitionToMain()
    {
        loadingToMain.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        //  오른쪽에서 왼쪽으로 걷기
        capyWalk.localScale = new Vector3(1, 1, 1); // 왼쪽 방향

        if (capyWalk != null)
            capyWalk.anchoredPosition = walkStartToMainPos; // 오른쪽에서 출발

        if (capyAnimator != null)
            capyAnimator.speed = 0.5f;

        if (loadingBarToMain != null)
            loadingBarToMain.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);

            if (loadingBarToMain != null)
                loadingBarToMain.fillAmount = t;

            if (capyWalk != null)
                capyWalk.anchoredPosition = Vector2.Lerp(walkStartToMainPos, walkEndToMainPos, t); // 역방향으로 이동

            yield return null;
        }

        kitchenFurniture.SetActive(false);
        mainFurniture.SetActive(true);
        loadingToMain.SetActive(false);

        UpdateButtonStates();
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
