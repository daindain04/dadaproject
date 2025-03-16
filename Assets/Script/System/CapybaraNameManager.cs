using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class CapybaraNameManager : MonoBehaviour
{
    public TMP_InputField nameInputField;  // ÀÔ·ÂÃ¢
    public TMP_Text nameDisplayText;  // ÀÌ¸§ Ç¥½Ã
    public TMP_Text warningText;  // °æ°í ¸Ş½ÃÁö Ç¥½Ã
    public GameObject NamePanel; // ÀÌ¸§¼³Á¤Ã¢

    private const int MAX_NAME_LENGTH = 10;  // ÃÖ´ë 10±ÛÀÚ
    private const string PLAYER_PREFS_KEY = "CapybaraName";  // PlayerPrefs Å°

    private void Start()
    {

        NamePanel.SetActive(false);

        warningText.text = "";  // ÃÊ±âÈ­

        // ÀúÀåµÈ ÀÌ¸§ ºÒ·¯¿À±â
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            string savedName = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
            nameDisplayText.text = $"Capybara name : {savedName}";
            nameInputField.text = savedName;  // ÀÔ·ÂÃ¢¿¡µµ ±âÁ¸ ÀÌ¸§ Ç¥½Ã
        }
    }

    // ÀÌ¸§¼³Á¤ Ã¢ È°¼ºÈ­
    public void OpenNamePanel()
    {
        NamePanel.SetActive(true);
    }

    // ÀÌ¸§¼³Á¤ Ã¢ ºñÈ°¼ºÈ­
    public void CloseNamePanel()
    {
        NamePanel.SetActive(false);
    }

    public void SetCapybaraName()
    {
        string inputName = nameInputField.text.Trim(); // °ø¹é Á¦°Å

        // ÇÑ±Û°ú ¿µ¾î¸¸ Æ÷ÇÔÇÏ´ÂÁö °Ë»ç
        if (!IsValidName(inputName))
        {
            warningText.text = "ÀÌ¸§Àº ÇÑ±Û°ú ¿µ¾î¸¸ »ç¿ëÇÒ ¼ö ÀÖ½À´Ï´Ù!";
            return;
        }

        // ÀÌ¸§ ±æÀÌ Á¦ÇÑ °Ë»ç
        if (inputName.Length > MAX_NAME_LENGTH)
        {
            warningText.text = $"ÀÌ¸§Àº ÃÖ´ë {MAX_NAME_LENGTH}ÀÚ±îÁö °¡´ÉÇÕ´Ï´Ù!";
            return;
        }

        // ÀÌ¸§ ÀúÀå ¹× UI ¾÷µ¥ÀÌÆ®
        nameDisplayText.text = $"Capybara name : {inputName}";
        warningText.text = "";

        // ÀÌ¸§ ÀúÀå (PlayerPrefs »ç¿ë)
        PlayerPrefs.SetString(PLAYER_PREFS_KEY, inputName);
        PlayerPrefs.Save(); // ÀúÀå ½ÇÇà
    }

    private bool IsValidName(string name)
    {
        // ÇÑ±Û(°¡-ÆR) ¶Ç´Â ¿µ¾î(a-z, A-Z)¸¸ Çã¿ë
        return Regex.IsMatch(name, "^[°¡-ÆRa-zA-Z]+$");
    }
}
