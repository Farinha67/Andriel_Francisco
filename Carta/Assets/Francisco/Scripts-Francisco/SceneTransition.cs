using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage; // Arraste uma imagem preta de UI aqui
    public float fadeDuration = 1.5f; // Duração do escurecimento em segundos
    public string nextSceneName; // Nome da próxima cena

    void Start()
    {

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0;
            fadeImage.color = color;
        }
    }

    // Chame este método no fim da sua cutscene
    public void StartTransition()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        // Faz o Fade para o preto
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Carrega a próxima cena após a tela ficar preta
        SceneManager.LoadScene(nextSceneName);
    }
}
