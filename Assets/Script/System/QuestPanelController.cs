using UnityEngine;
using UnityEngine.UI;

public class QuestPanelController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button infoButton;        // ������ư
    public GameObject questPanel;    // ����Ʈ �г�
    public Button closeButton;       // ����Ʈ �г� ���� �ݱ� ��ư

    void Start()
    {
        // ������ �� ����Ʈ �г� ��Ȱ��ȭ
        questPanel.SetActive(false);

        // ��ư �̺�Ʈ ����
        infoButton.onClick.AddListener(OpenQuestPanel);
        closeButton.onClick.AddListener(CloseQuestPanel);
    }

    /// <summary>
    /// ������ư Ŭ�� �� ����Ʈ �г� ����
    /// </summary>
    public void OpenQuestPanel()
    {
        questPanel.SetActive(true);
    }

    /// <summary>
    /// �ݱ� ��ư Ŭ�� �� ����Ʈ �г� �ݱ�
    /// </summary>
    public void CloseQuestPanel()
    {
        questPanel.SetActive(false);
    }

    /// <summary>
    /// ����Ʈ �г� ��� (���������� �ݰ�, ���������� ����)
    /// </summary>
    public void ToggleQuestPanel()
    {
        questPanel.SetActive(!questPanel.activeSelf);
    }
}