using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class CapybaraNameManager : MonoBehaviour
{
    public TMP_InputField nameInputField;  // �Է�â
    public TMP_Text nameDisplayText;  // �̸� ǥ��
    public TMP_Text warningText;  // ��� �޽��� ǥ��
    public GameObject NamePanel; // �̸�����â

    private const int MAX_NAME_LENGTH = 10;  // �ִ� 10����
    private const string PLAYER_PREFS_KEY = "CapybaraName";  // PlayerPrefs Ű

    private void Start()
    {

        NamePanel.SetActive(false);

        warningText.text = "";  // �ʱ�ȭ

        // ����� �̸� �ҷ�����
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            string savedName = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
            nameDisplayText.text = $"Capybara name : {savedName}";
            nameInputField.text = savedName;  // �Է�â���� ���� �̸� ǥ��
        }
    }

    // �̸����� â Ȱ��ȭ
    public void OpenNamePanel()
    {
        NamePanel.SetActive(true);
    }

    // �̸����� â ��Ȱ��ȭ
    public void CloseNamePanel()
    {
        NamePanel.SetActive(false);
    }

    public void SetCapybaraName()
    {
        string inputName = nameInputField.text.Trim(); // ���� ����

        // �ѱ۰� ��� �����ϴ��� �˻�
        if (!IsValidName(inputName))
        {
            warningText.text = "�̸��� �ѱ۰� ��� ����� �� �ֽ��ϴ�!";
            return;
        }

        // �̸� ���� ���� �˻�
        if (inputName.Length > MAX_NAME_LENGTH)
        {
            warningText.text = $"�̸��� �ִ� {MAX_NAME_LENGTH}�ڱ��� �����մϴ�!";
            return;
        }

        // �̸� ���� �� UI ������Ʈ
        nameDisplayText.text = $"Capybara name : {inputName}";
        warningText.text = "";

        // �̸� ���� (PlayerPrefs ���)
        PlayerPrefs.SetString(PLAYER_PREFS_KEY, inputName);
        PlayerPrefs.Save(); // ���� ����
    }

    private bool IsValidName(string name)
    {
        // �ѱ�(��-�R) �Ǵ� ����(a-z, A-Z)�� ���
        return Regex.IsMatch(name, "^[��-�Ra-zA-Z]+$");
    }
}
