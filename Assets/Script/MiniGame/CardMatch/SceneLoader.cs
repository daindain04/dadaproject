using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
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
