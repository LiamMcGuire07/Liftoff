using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string LevelScene;

    public void StartGame()
    {
        SceneManager.LoadScene(LevelScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
