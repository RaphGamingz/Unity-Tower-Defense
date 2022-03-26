using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public string MainGameScene = "Game";
    public void Play()
    {
        SceneManager.LoadScene(MainGameScene);
    }
    public void Quit()
    {
        Application.Quit();
    }
}