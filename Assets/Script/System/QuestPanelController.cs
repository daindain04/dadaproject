using UnityEngine;
using UnityEngine.UI;

public class QuestPanelController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button infoButton;        // 인포버튼
    public GameObject questPanel;    // 퀘스트 패널
    public Button closeButton;       // 퀘스트 패널 안의 닫기 버튼

    void Start()
    {
        // 시작할 때 퀘스트 패널 비활성화
        questPanel.SetActive(false);

        // 버튼 이벤트 연결
        infoButton.onClick.AddListener(OpenQuestPanel);
        closeButton.onClick.AddListener(CloseQuestPanel);
    }

    /// <summary>
    /// 인포버튼 클릭 시 퀘스트 패널 열기
    /// </summary>
    public void OpenQuestPanel()
    {
        questPanel.SetActive(true);
    }

    /// <summary>
    /// 닫기 버튼 클릭 시 퀘스트 패널 닫기
    /// </summary>
    public void CloseQuestPanel()
    {
        questPanel.SetActive(false);
    }

    /// <summary>
    /// 퀘스트 패널 토글 (열려있으면 닫고, 닫혀있으면 열기)
    /// </summary>
    public void ToggleQuestPanel()
    {
        questPanel.SetActive(!questPanel.activeSelf);
    }
}