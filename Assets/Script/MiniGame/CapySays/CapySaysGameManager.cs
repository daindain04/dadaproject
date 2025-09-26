using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState
{
    Idle,
    ShowingSequence,
    WaitingInput,
    ShowingResult,
    GameOver
}

public enum CapyGameDifficulty { Easy, Normal, Hard }

public class CapySaysGameManager : MonoBehaviour
{
    [Header("Game Settings (Adjust in Inspector)")]
    [SerializeField] private int startSequenceLength = 3;
    [SerializeField] private int targetRounds = 5;
    [SerializeField] private float inputTimeLimit = 4f;
    [SerializeField] private int maxLives = 1;
    [SerializeField] private float sequenceImageScale = 0.5f;
    [SerializeField] private CapyGameDifficulty difficulty = CapyGameDifficulty.Easy; // 난이도 설정

    [Header("UI References")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private Image timerFill;
    [SerializeField] private Image[] livesImages;

    [SerializeField] private RectTransform sequenceArea;
    [SerializeField] private GameObject sequenceItemPrefab;
    [SerializeField] private GameObject curtainImage;  // 커튼 이미지 추가

    [SerializeField] private Button[] buttons;            // 9 buttons in the grid
    [SerializeField] private Image[] inputSlots;          // input display slots

    [Header("Overlay Message")]
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private GameObject showSequenceImage;
    [SerializeField] private GameObject playerTurnImage;
    [SerializeField] private GameObject successImage;
    [SerializeField] private GameObject failImage;

    [Header("Game End Panels")]
    [SerializeField] private GameObject gameOverPanel;    // 게임 오버 패널
    [SerializeField] private GameObject gameWinPanel;     // 게임 승리 패널

    private List<int> currentSequence = new List<int>();
    private List<int> playerInputs = new List<int>();

    private int livesLeft;
    private int currentRound;
    private int currentSeqLength;

    private float timer;
    private bool inputPhaseActive;
    private bool roundSuccess;
    private bool roundFailed;

    private GameState state = GameState.Idle;

    private void Start()
    {
        // 시작할 때 커튼 활성화
        if (curtainImage != null)
            curtainImage.SetActive(true);

        // 패널들 초기화
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (gameWinPanel != null)
            gameWinPanel.SetActive(false);

        // Bind button callbacks dynamically
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
            int idx = i;
            buttons[i].onClick.AddListener(() => OnButtonPressed(idx));
        }
        StartCoroutine(PlayGameRoutine());
    }

    private void Update()
    {
        if (state == GameState.WaitingInput && inputPhaseActive)
        {
            timer -= Time.deltaTime;
            if (timerFill != null)
                timerFill.fillAmount = timer / inputTimeLimit;

            if (timer <= 0f)
            {
                // Time up
                livesLeft--;
                UpdateLivesUI();
                roundFailed = true;
                EndInputPhase();
            }
        }
    }

    private IEnumerator PlayGameRoutine()
    {
        Debug.Log("PlayGameRoutine started!");

        // Initialize lives
        livesLeft = maxLives;
        UpdateLivesUI();
        Debug.Log($"Lives initialized: {livesLeft}");

        for (currentRound = 1; currentRound <= targetRounds; currentRound++)
        {
            Debug.Log($"Starting round {currentRound}");

            // Sequence length fixed per game settings
            currentSeqLength = startSequenceLength;
            UpdateRoundUI();

            bool roundCleared = false;
            while (!roundCleared && livesLeft > 0)
            {
                // Generate a random sequence
                currentSequence.Clear();
                for (int i = 0; i < currentSeqLength; i++)
                    currentSequence.Add(Random.Range(0, buttons.Length));

                // Clear input display
                foreach (var slot in inputSlots)
                    slot.gameObject.SetActive(false);

                // Show 'Watch the Sequence' message
                yield return ShowOverlay(showSequenceImage, 1.0f);

                // Play sequence (커튼을 내리고 시퀀스 재생)
                yield return PlaySequence();

                // Show 'Your Turn' message
                yield return ShowOverlay(playerTurnImage, 0.8f);

                // Start player input phase
                state = GameState.WaitingInput;
                timer = inputTimeLimit;
                roundSuccess = false;
                roundFailed = false;
                playerInputs.Clear();
                inputPhaseActive = true;
                EnableButtons(true);

                // Wait until player finishes input or fails
                yield return new WaitUntil(() => !inputPhaseActive);
                EnableButtons(false);

                // Show result message
                if (roundSuccess)
                {
                    roundCleared = true;
                    yield return ShowOverlay(successImage, 1.0f);
                }
                else
                {
                    // 실패해도 라운드 클리어로 처리하여 다음 라운드로 진행
                    roundCleared = true;
                    yield return ShowOverlay(failImage, 1.0f);
                }
            }

            if (livesLeft <= 0)
                break;
        }

        // Game end
        if (livesLeft > 0)
            HandleWin();
        else
            HandleGameOver();
    }

    private IEnumerator PlaySequence()
    {
        // 시퀀스 시작할 때 커튼 내리기
        if (curtainImage != null)
            curtainImage.SetActive(false);

        foreach (int idx in currentSequence)
        {
            GameObject go = Instantiate(sequenceItemPrefab, sequenceArea);
            Image img = go.GetComponent<Image>();
            img.sprite = buttons[idx].image.sprite;

            // RectTransform으로 직접 크기 설정
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            float targetSize = 60f * sequenceImageScale; // 기본 크기 60픽셀에 스케일 적용
            rectTransform.sizeDelta = new Vector2(targetSize, targetSize);

            go.transform.localScale = Vector3.zero;

            // Pop-in animation
            float animTime = 0.2f;
            for (float t = 0; t < animTime; t += Time.deltaTime)
            {
                go.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / animTime);
                yield return null;
            }
            go.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(0.3f);
            Destroy(go);
            yield return new WaitForSeconds(0.1f);
        }

        // 시퀀스 끝날 때 커튼 다시 올리기
        if (curtainImage != null)
            curtainImage.SetActive(true);
    }

    private IEnumerator ShowOverlay(GameObject messageObj, float duration)
    {
        // Activate overlay
        overlayGroup.gameObject.SetActive(true);
        overlayGroup.alpha = 0f;

        // Hide all messages
        showSequenceImage.SetActive(false);
        playerTurnImage.SetActive(false);
        successImage.SetActive(false);
        failImage.SetActive(false);

        // Show this one
        messageObj.SetActive(true);

        // Fade in
        yield return FadeCanvasGroup(overlayGroup, 0f, 1f, 0.2f);
        yield return new WaitForSeconds(duration);
        // Fade out
        yield return FadeCanvasGroup(overlayGroup, 1f, 0f, 0.2f);

        messageObj.SetActive(false);
        overlayGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            cg.alpha = Mathf.Lerp(from, to, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = to;
    }

    public void OnButtonPressed(int btnIndex)
    {
        Debug.Log($"[Input Debug] btnIndex={btnIndex}, spriteName={buttons[btnIndex].image.sprite.name}");
        if (state != GameState.WaitingInput || !inputPhaseActive)
            return;

        // Reset timer
        timer = inputTimeLimit;

        // Record input
        int inputIdx = playerInputs.Count;
        playerInputs.Add(btnIndex);
        inputSlots[inputIdx].sprite = buttons[btnIndex].image.sprite;
        inputSlots[inputIdx].gameObject.SetActive(true);

        // Check correctness
        if (playerInputs[inputIdx] != currentSequence[inputIdx])
        {
            // Wrong
            livesLeft--;
            UpdateLivesUI();
            roundFailed = true;
            EndInputPhase();
        }
        else if (playerInputs.Count >= currentSequence.Count)
        {
            // Correct full sequence
            roundSuccess = true;
            EndInputPhase();
        }
    }

    private void EndInputPhase()
    {
        inputPhaseActive = false;
        state = GameState.ShowingResult;
    }

    private void EnableButtons(bool enabled)
    {
        foreach (var btn in buttons)
            btn.interactable = enabled;
    }

    private void UpdateRoundUI()
    {
        roundText.text = $"Round {currentRound} / {targetRounds}";
    }

    private void UpdateLivesUI()
    {
        for (int i = 0; i < livesImages.Length; i++)
            livesImages[i].enabled = (i < livesLeft);
    }

    /// <summary>
    /// 난이도별 보상 지급
    /// </summary>
    private void GiveReward()
    {
        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning("MoneyManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        switch (difficulty)
        {
            case CapyGameDifficulty.Easy:
                MoneyManager.Instance.AddCoins(100);
                MoneyManager.Instance.AddGems(1);
                MoneyManager.Instance.AddExperience(10);
                Debug.Log("Easy 난이도 클리어! 코인 100, 보석 1, 경험치 10 획득");
                break;

            case CapyGameDifficulty.Normal:
                MoneyManager.Instance.AddCoins(200);
                MoneyManager.Instance.AddGems(2);
                MoneyManager.Instance.AddExperience(20);
                Debug.Log("Normal 난이도 클리어! 코인 200, 보석 2, 경험치 20 획득");
                break;

            case CapyGameDifficulty.Hard:
                MoneyManager.Instance.AddCoins(300);
                MoneyManager.Instance.AddGems(4);
                MoneyManager.Instance.AddExperience(30);
                Debug.Log("Hard 난이도 클리어! 코인 300, 보석 4, 경험치 30 획득");
                break;
        }
    }

    private void HandleWin()
    {
        Debug.Log("You Win!");

        // 보상 지급
        GiveReward();

        // 승리 패널 표시
        if (gameWinPanel != null)
        {
            gameWinPanel.SetActive(true);
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over");

        // 게임 오버 패널 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 게임오버 패널에서 재시작 버튼 클릭 시 호출
    /// </summary>
    public void OnGameOverRestartPressed()
    {
        // 현재 씬을 다시 로드하여 게임 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 게임오버 패널에서 나가기 버튼 클릭 시 호출
    /// </summary>
    public void OnGameOverExitPressed()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 게임승리 패널에서 확인 버튼 클릭 시 호출
    /// </summary>
    public void OnGameWinConfirmPressed()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("Main");
    }
}