using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    Idle,
    ShowingSequence,
    WaitingInput,
    ShowingResult,
    GameOver
}

public class CapySaysGameManager : MonoBehaviour
{
    [Header("Game Settings (Adjust in Inspector)")]
    [SerializeField] private int startSequenceLength = 3;
    [SerializeField] private int targetRounds = 5;
    [SerializeField] private float inputTimeLimit = 4f;
    [SerializeField] private int maxLives = 1;

    [Header("UI References")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private Image timerFill;
    [SerializeField] private Image[] livesImages;

    [SerializeField] private RectTransform sequenceArea;
    [SerializeField] private GameObject sequenceItemPrefab;

    [SerializeField] private Button[] buttons;            // 9 buttons in the grid
    [SerializeField] private Image[] inputSlots;          // input display slots

    [Header("Overlay Message")]
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private GameObject showSequenceImage;
    [SerializeField] private GameObject playerTurnImage;
    [SerializeField] private GameObject successImage;
    [SerializeField] private GameObject failImage;

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
        // Initialize lives
        livesLeft = maxLives;
        UpdateLivesUI();

        for (currentRound = 1; currentRound <= targetRounds; currentRound++)
        {
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

                // Play sequence
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
        foreach (int idx in currentSequence)
        {
            // Spawn a sequence item
            GameObject go = Instantiate(sequenceItemPrefab, sequenceArea);
            Image img = go.GetComponent<Image>();
            img.sprite = buttons[idx].image.sprite;
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

    private void HandleWin()
    {
        Debug.Log("You Win!");
        // TODO: Win 처리 (보상, 다음 화면 등)
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over");
        // TODO: Game Over 처리 (리트라이, 메인 메뉴 등)
    }
}
