using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManger : MonoBehaviour
{
    private const string CoverSceneName = "Cover";

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(CoverSceneName);
    }
}
