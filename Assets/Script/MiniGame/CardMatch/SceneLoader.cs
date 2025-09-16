using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainScene()
    {
        StartCoroutine(LoadMainSceneWithUIUpdate());
    }

    private IEnumerator LoadMainSceneWithUIUpdate()
    {
        SceneManager.LoadScene("MainScene");

        // �� �ε� �Ϸ���� ���
        yield return new WaitForSeconds(0.1f);

        // ���ξ��� UI ���� ������Ʈ
        MoneyUIManager mainUI = FindObjectOfType<MoneyUIManager>();
        if (mainUI != null)
        {
            mainUI.UpdateUI();
        }
    }

    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMiniGameScene()
    {
        SceneManager.LoadScene("MiniGame");
    }
}
