using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PawClickEffect : MonoBehaviour
{
    [Header("발바닥 설정")]
    public GameObject pawPrefab;
    public float effectDuration = 0.6f;
    public float scaleChange = 0.8f;
    public float moveUpDistance = 50f;
    public int maxRotation = 30;

    [Header("크기 조정")]
    [Tooltip("모든 씬에서 동일하게 보일 발바닥의 기본 크기")]
    public float basePawSize = 80f; // 픽셀 단위로 설정
    [Tooltip("최소 발바닥 크기")]
    public float minPawSize = 40f;
    [Tooltip("최대 발바닥 크기")]
    public float maxPawSize = 200f;

    [Header("클릭 방해 방지")]
    public bool allowPawOnUI = false;

    private static PawClickEffect instance;
    private Canvas uiCanvas;
    private List<GameObject> activeEffects = new List<GameObject>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        FindCanvas();
    }

    void Update()
    {
        if (uiCanvas == null)
        {
            FindCanvas();
        }

        if (Input.GetMouseButtonDown(0))
        {
            CreatePawEffect(Input.mousePosition);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                CreatePawEffect(touch.position);
            }
        }
    }

    void FindCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        if (canvases.Length == 0)
        {
            Debug.LogWarning("PawClickEffect: 씬에 Canvas가 없습니다!");
            return;
        }

        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                uiCanvas = canvas;
                break;
            }
        }

        if (uiCanvas == null && canvases.Length > 0)
        {
            uiCanvas = canvases[0];
        }
    }

    // 캔버스의 실제 스케일 팩터를 계산
    float GetCanvasScaleFactor()
    {
        if (uiCanvas == null) return 1f;

        CanvasScaler scaler = uiCanvas.GetComponent<CanvasScaler>();
        if (scaler == null) return 1f;

        if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            Vector2 referenceResolution = scaler.referenceResolution;
            float referenceWidth = referenceResolution.x;
            float referenceHeight = referenceResolution.y;

            // 0으로 나누기 방지
            if (referenceWidth <= 0 || referenceHeight <= 0)
                return 1f;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float widthRatio = screenWidth / referenceWidth;
            float heightRatio = screenHeight / referenceHeight;

            // matchWidthOrHeight 값에 따라 스케일 계산
            float match = scaler.matchWidthOrHeight;

            // 로그 스케일로 계산 (Unity의 실제 계산 방식)
            float logWidth = Mathf.Log(widthRatio, 2f);
            float logHeight = Mathf.Log(heightRatio, 2f);
            float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, match);

            return Mathf.Pow(2f, logWeightedAverage);
        }
        else if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
        {
            return scaler.scaleFactor;
        }

        return 1f;
    }

    void CreatePawEffect(Vector3 screenPosition)
    {
        if (pawPrefab == null || uiCanvas == null) return;

        if (!allowPawOnUI && IsPointerOverUI()) return;

        GameObject pawEffect = Instantiate(pawPrefab, uiCanvas.transform);

        RectTransform canvasRect = uiCanvas.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPosition, uiCanvas.worldCamera, out localPoint);

        RectTransform pawRect = pawEffect.GetComponent<RectTransform>();
        pawRect.localPosition = localPoint;
        pawRect.localRotation = Quaternion.Euler(0, 0, Random.Range(-maxRotation, maxRotation));

        // 캔버스 스케일 팩터를 고려한 크기 계산
        float scaleFactor = GetCanvasScaleFactor();

        // basePawSize를 캔버스 스케일로 나누어 모든 씬에서 동일한 실제 크기로 보이게 함
        float adjustedSize = basePawSize / scaleFactor;
        adjustedSize = Mathf.Clamp(adjustedSize, minPawSize, maxPawSize);

        pawRect.localScale = Vector3.one * adjustedSize;

        SetupPawForNonInterference(pawEffect);
        pawRect.SetAsLastSibling();

        activeEffects.Add(pawEffect);
        StartCoroutine(AnimatePaw(pawEffect, scaleFactor));
    }

    IEnumerator AnimatePaw(GameObject pawEffect, float scaleFactor)
    {
        if (pawEffect == null) yield break;

        RectTransform rect = pawEffect.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = pawEffect.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = pawEffect.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
        }

        Vector3 startPos = rect.localPosition;
        // 이동 거리도 캔버스 스케일에 맞춰 조정
        float adjustedMoveDistance = moveUpDistance / scaleFactor;
        Vector3 endPos = startPos + Vector3.up * adjustedMoveDistance;
        Vector3 startScale = rect.localScale;
        Vector3 endScale = startScale * scaleChange;
        float startRotation = rect.localEulerAngles.z;
        float endRotation = startRotation + Random.Range(-40f, 40f);

        float elapsedTime = 0f;

        while (elapsedTime < effectDuration && pawEffect != null)
        {
            float progress = elapsedTime / effectDuration;
            float easedProgress = 1f - (1f - progress) * (1f - progress);

            rect.localPosition = Vector3.Lerp(startPos, endPos, easedProgress);
            rect.localScale = Vector3.Lerp(startScale, endScale, progress);

            Vector3 euler = rect.localEulerAngles;
            euler.z = Mathf.Lerp(startRotation, endRotation, progress);
            rect.localEulerAngles = euler;

            canvasGroup.alpha = 1f - progress;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        RemoveEffect(pawEffect);
    }

    void RemoveEffect(GameObject effect)
    {
        if (effect != null)
        {
            activeEffects.Remove(effect);
            Destroy(effect);
        }
    }

    bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current != null &&
               UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    void SetupPawForNonInterference(GameObject pawEffect)
    {
        UnityEngine.UI.Image image = pawEffect.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.raycastTarget = false;
        }

        CanvasGroup canvasGroup = pawEffect.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup = pawEffect.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
        }
    }

    void OnLevelWasLoaded(int level)
    {
        FindCanvas();
    }

    void OnDestroy()
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeEffects.Clear();
    }
}