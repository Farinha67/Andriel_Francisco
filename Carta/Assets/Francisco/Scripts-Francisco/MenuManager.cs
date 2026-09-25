using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string GameScene;

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
    