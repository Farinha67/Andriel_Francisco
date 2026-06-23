using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SleepTransition : MonoBehaviour
{
    public Image fadeImage; // imagem preta cobrindo a tela
    private bool sleeping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sleeping)
        {
            StartCoroutine(Sleep());
        }
    }

    IEnumerator Sleep()
    {
        sleeping = true;

        Color color = fadeImage.color;

        // Escurecer
        for (float a = 0; a <= 1; a += Time.deltaTime)
        {
            color.a = a;
            fadeImage.color = color;
            yield return null;
        }

        // Tela preta por 3 segundos
        yield return new WaitForSeconds(3f);

        // Clarear
        for (float a = 1; a >= 0; a -= Time.deltaTime)
        {
            color.a = a;
            fadeImage.color = color;
            yield return null;
        }

        sleeping = false;
    }
}