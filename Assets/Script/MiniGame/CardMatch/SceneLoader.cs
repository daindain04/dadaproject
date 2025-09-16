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

        // 씬 로드 완료까지 대기
        yield return new WaitForSeconds(0.1f);

        // 메인씬의 UI 강제 업데이트
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
