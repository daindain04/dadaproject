using UnityEngine;

public class DifficultySelector : MonoBehaviour
{
    public GameObject easyPanel;
    public GameObject normalPanel;
    public GameObject hardPanel;

    public void OnEasy() => Show(easyPanel);
    public void OnNormal() => Show(normalPanel);
    public void OnHard() => Show(hardPanel);

    void Show(GameObject panel)
    {
        easyPanel.SetActive(false);
        normalPanel.SetActive(false);
        hardPanel.SetActive(false);
        panel.SetActive(true);
    }
}
