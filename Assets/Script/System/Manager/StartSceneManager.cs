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
    public TextMeshProUGUI noticeText;
    public GameObject synopsis1;
    public GameObject synopsis2;
    public GameObject namePanel;

    [Header("Button References")]
    public Button nextButton;
    public Button nameSettingButton;
    public Button confirmButton;

    [Header("Name Input")]
    public TMP_InputField nameInputField;

    private const int MAX_NAME_LENGTH = 10;
    private const string PLAYER_PREFS_KEY = "CapybaraName";

    private bool hasPressedKey = false;

    void Start()
    {
        // ÃÊ±â ¼¼ÆÃ
        startPanel.SetActive(true);
        synopsis1.SetActive(false);
        synopsis2.SetActive(false);
        namePanel.SetActive(false);

        StartCoroutine(BlinkNoticeText());

        // ¹öÆ° ÀÌº¥Æ® ¿¬°á
        nextButton.onClick.AddListener(OnNextClicked);
        nameSettingButton.onClick.AddListener(OnNameSettingClicked);
        confirmButton.onClick.AddListener(SetCapybaraName);

        // ±âÁ¸ ÀÌ¸§ ºÒ·¯¿À±â
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            nameInputField.text = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
        }
    }

    void Update()
    {
        if (!hasPressedKey && Input.anyKeyDown)
        {
            hasPressedKey = true;
            startPanel.SetActive(false);
            StopCoroutine(BlinkNoticeText());
            noticeText.alpha = 1f;
            synopsis1.SetActive(true);
        }
    }

    IEnumerator BlinkNoticeText()
    {
        while (true)
        {
            for (float alpha = 1; alpha >= 0; alpha -= 0.05f)
            {
                noticeText.alpha = alpha;
                yield return new WaitForSeconds(0.03f);
            }
            for (float alpha = 0; alpha <= 1; alpha += 0.05f)
            {
                noticeText.alpha = alpha;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }

    void OnNextClicked()
    {
        synopsis1.SetActive(false);
        synopsis2.SetActive(true);
    }

    void OnNameSettingClicked()
    {
        synopsis2.SetActive(false);
        namePanel.SetActive(true);
    }

    void SetCapybaraName()
    {
        string inputName = nameInputField.text.Trim();

        if (!IsValidName(inputName))
        {
            Debug.LogWarning("ÀÌ¸§Àº ÇÑ±Û ¶Ç´Â ¿µ¾î¸¸ °¡´ÉÇÕ´Ï´Ù.");
            return;
        }

        if (inputName.Length > MAX_NAME_LENGTH)
        {
            Debug.LogWarning("ÀÌ¸§Àº ÃÖ´ë 10ÀÚ±îÁö ÀÔ·ÂÇÒ ¼ö ÀÖ½À´Ï´Ù.");
            return;
        }

        PlayerPrefs.SetString(PLAYER_PREFS_KEY, inputName);
        PlayerPrefs.Save();

        Debug.Log("Ä«ÇÇ¹Ù¶ó ÀÌ¸§ ÀúÀå ¿Ï·á: " + inputName);

        // Main ¾ÀÀ¸·Î ÀÌµ¿
        SceneManager.LoadScene("Main");
    }

    bool IsValidName(string name)
    {
        return Regex.IsMatch(name, "^[°¡-ÆRa-zA-Z]+$");
    }
}
