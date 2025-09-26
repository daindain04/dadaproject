using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject startPanel;
    public Image noticeImage;
    public GameObject synopsis1;
    public GameObject synopsis2;
    public GameObject namePanel;

    [Header("Button References")]
    public Button nextButton;
    public Button nameSettingButton;
    public Button confirmButton;

    [Header("Name Input")]
    public TMP_InputField nameInputField;

    [Header("Fade Effect")]
    public Image fadeImage; // 암전용 이미지 (검은색)
    [Range(0.1f, 2.0f)]
    public float fadeToBlackDuration = 0.5f; // 암전 속도
    [Range(0.1f, 2.0f)]
    public float fadeFromBlackDuration = 0.5f; // 밝아지는 속도

    [Header("Synopsis Text")]
    public TextMeshProUGUI[] synopsis1Texts; // 시놉시스1의 텍스트들
    public TextMeshProUGUI[] synopsis2Texts; // 시놉시스2의 텍스트들

    [Header("Typing Effect Settings")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f; // 타이핑 속도 (한 글자당 시간)
    [Range(0.0f, 2.0f)]
    public float lineDelay = 0.5f; // 다음 줄까지의 대기 시간

    private const int MAX_NAME_LENGTH = 10;
    private const string PLAYER_PREFS_KEY = "CapybaraName";
    private bool hasPressedKey = false;
    private bool isTextAnimating = false;
    private bool canSkipTextAnimation = false;

    // 노티스 깜빡임 코루틴 참조
    private Coroutine blinkRoutine;

    void Start()
    {
        // 초기 세팅
        startPanel.SetActive(true);
        synopsis1.SetActive(false);
        synopsis2.SetActive(false);
        namePanel.SetActive(false);

        // 페이드 이미지 초기화
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(true); // 처음엔 켜둔 상태
        }

        // (디버그용) 텍스트 로깅
        InitializeTexts();

        // 버튼 이벤트 연결
        nextButton.onClick.AddListener(OnNextClicked);
        nameSettingButton.onClick.AddListener(OnNameSettingClicked);
        confirmButton.onClick.AddListener(SetCapybaraName);

        // 기존 이름 불러오기
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
            nameInputField.text = PlayerPrefs.GetString(PLAYER_PREFS_KEY);

        // 노티스 이미지 깜빡임 시작
        blinkRoutine = StartCoroutine(BlinkNoticeImage());
    }

    void Update()
    {
        if (!hasPressedKey && Input.anyKeyDown)
        {
            hasPressedKey = true;
            StartCoroutine(TransitionToSynopsis1());
        }

        // 텍스트 애니메이션 중 스페이스바로 스킵
        if (isTextAnimating && Input.GetKeyDown(KeyCode.Space))
            canSkipTextAnimation = true;
    }

    void InitializeTexts()
    {
        
        if (synopsis1Texts != null)
        {
            
            for (int i = 0; i < synopsis1Texts.Length; i++)
                if (synopsis1Texts[i] != null)
                    Debug.Log($"Synopsis1 Text {i}: '{synopsis1Texts[i].text}'");
        }

        if (synopsis2Texts != null)
        {
           
            for (int i = 0; i < synopsis2Texts.Length; i++)
                if (synopsis2Texts[i] != null)
                    Debug.Log($"Synopsis2 Text {i}: '{synopsis2Texts[i].text}'");
        }
    }

    IEnumerator BlinkNoticeImage()
    {
        while (true)
        {
            // 페이드 아웃
            for (float alpha = 1; alpha >= 0; alpha -= 0.05f)
            {
                if (noticeImage != null)
                {
                    Color imageColor = noticeImage.color;
                    imageColor.a = alpha;
                    noticeImage.color = imageColor;
                }
                yield return new WaitForSeconds(0.03f);
            }

            // 페이드 인
            for (float alpha = 0; alpha <= 1; alpha += 0.05f)
            {
                if (noticeImage != null)
                {
                    Color imageColor = noticeImage.color;
                    imageColor.a = alpha;
                    noticeImage.color = imageColor;
                }
                yield return new WaitForSeconds(0.03f);
            }
        }
    }

    IEnumerator TransitionToSynopsis1()
    {
        // 깜빡임 코루틴 중지
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        // 노티스 이미지 알파 복구
        if (noticeImage != null)
        {
            var c = noticeImage.color;
            c.a = 1f;
            noticeImage.color = c;
        }

        // 암전
        yield return StartCoroutine(FadeToBlack());

        // 패널 전환
        startPanel.SetActive(false);
        synopsis1.SetActive(true);

        // **원본 캐싱 + 화면 텍스트 비우기**
        string[] originals = CacheAndClear(synopsis1Texts);

        // 밝아짐
        yield return StartCoroutine(FadeFromBlack());

        // 타이핑 시작 (원본을 넘겨줌)
        yield return StartCoroutine(AnimateTexts(synopsis1Texts, originals));
    }

    IEnumerator TransitionToSynopsis2()
    {
        // 암전
        yield return StartCoroutine(FadeToBlack());

        // 패널 전환
        synopsis1.SetActive(false);
        synopsis2.SetActive(true);

        // **원본 캐싱 + 화면 텍스트 비우기**
        string[] originals = CacheAndClear(synopsis2Texts);

        // 밝아짐
        yield return StartCoroutine(FadeFromBlack());

        // 타이핑 시작
        yield return StartCoroutine(AnimateTexts(synopsis2Texts, originals));
    }

    void OnNextClicked()
    {
        if (isTextAnimating) return;
        StartCoroutine(TransitionToSynopsis2());
    }

    void OnNameSettingClicked()
    {
        if (isTextAnimating) return;
        StartCoroutine(TransitionToNamePanel());
    }

    IEnumerator TransitionToNamePanel()
    {
        // 암전
        yield return StartCoroutine(FadeToBlack());

        // 패널 전환
        synopsis2.SetActive(false);
        namePanel.SetActive(true);

        // (이름 패널은 타이핑 대상이 없으면 캐싱 불필요)
        // 만약 이름 패널에도 설명 텍스트 타이핑이 필요하면
        // TextMeshProUGUI[] namePanelTexts 를 만들어 같은 패턴 적용

        // 밝아짐
        yield return StartCoroutine(FadeFromBlack());
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        if (!fadeImage.gameObject.activeSelf)
            fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeToBlackDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeToBlackDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1f);
    }

    IEnumerator FadeFromBlack()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeFromBlackDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeFromBlackDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0f);

        // “필요할 때 활성화” 전략 유지
        fadeImage.gameObject.SetActive(false);
    }

    // --- 여기부터 타이핑 유틸 ---

    /// <summary>
    /// 각 TextMeshProUGUI의 현재 텍스트를 배열로 캐싱하고,
    /// 화면에 보이는 텍스트는 즉시 ""로 비웁니다.
    /// </summary>
    private string[] CacheAndClear(TextMeshProUGUI[] texts)
    {
        if (texts == null || texts.Length == 0) return new string[0];

        string[] originals = new string[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null)
            {
                originals[i] = string.Empty;
                continue;
            }

            originals[i] = texts[i].text; // 원본 보관
            texts[i].text = "";           // 화면 비우기
        }
        return originals;
    }

    /// <summary>
    /// 캐싱된 원본 문자열을 이용해 타자 효과를 재생합니다.
    /// </summary>
    IEnumerator AnimateTexts(TextMeshProUGUI[] texts, string[] originals)
    {
        if (texts == null || originals == null || texts.Length == 0 || originals.Length == 0)
            yield break;

        isTextAnimating = true;
        canSkipTextAnimation = false;

        for (int i = 0; i < texts.Length; i++)
        {
            var label = texts[i];
            string full = (i < originals.Length) ? originals[i] : string.Empty;

            if (label == null || string.IsNullOrEmpty(full))
                continue;

            // 스킵 플래그가 이미 켜졌다면 남은 줄 즉시 표시
            if (canSkipTextAnimation)
            {
                for (int j = i; j < texts.Length; j++)
                {
                    if (texts[j] != null && j < originals.Length)
                        texts[j].text = originals[j];
                }
                break;
            }

            // 타이핑
            string current = "";
            for (int charIndex = 0; charIndex < full.Length; charIndex++)
            {
                if (canSkipTextAnimation)
                {
                    label.text = full;
                    break;
                }

                current += full[charIndex];
                label.text = current;

                yield return new WaitForSeconds(typingSpeed);
            }

            // 다음 줄 대기
            if (!canSkipTextAnimation && i < texts.Length - 1)
                yield return new WaitForSeconds(lineDelay);
        }

        isTextAnimating = false;
    }

    // --- 이름 저장 ---

    void SetCapybaraName()
    {
        string inputName = nameInputField.text.Trim();
        if (!IsValidName(inputName))
        {
            Debug.LogWarning("이름은 한글 또는 영어만 가능합니다.");
            return;
        }
        if (inputName.Length > MAX_NAME_LENGTH)
        {
            Debug.LogWarning("이름은 최대 10자까지 입력할 수 있습니다.");
            return;
        }

        PlayerPrefs.SetString(PLAYER_PREFS_KEY, inputName);
        PlayerPrefs.Save();
        Debug.Log("카피바라 이름 저장 완료: " + inputName);

        SceneManager.LoadScene("Main");
    }

    bool IsValidName(string name)
    {
        return Regex.IsMatch(name, "^[가-힣a-zA-Z]+$");
    }
}
