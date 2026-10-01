using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string GameScene;
    [SerializeField] private GameObject fade;
    [SerializeField] private float tempoFade = 1f;

    public void PlayGame()
    {
        StartCoroutine(CarregarCena());
    }

    private IEnumerator CarregarCena()
    {
        fade.SetActive(true);

        yield return new WaitForSeconds(tempoFade);

        SceneManager.LoadScene(GameScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
