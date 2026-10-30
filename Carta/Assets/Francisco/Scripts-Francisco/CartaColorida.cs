
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CartaColorida : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private Image imagem;

    [SerializeField] private float duracaoFade = 2f;
    [SerializeField] private float intervalo = 0.5f;

    private void Start()
    {
        if (imagem == null)
            imagem = GetComponent<Image>();

        StartCoroutine(AlternarCores());
    }

    private IEnumerator AlternarCores()
    {
        Color corAtual = imagem.color;

        while (true)
        {
            Color proximaCor = Random.ColorHSV(
                0f, 1f,
                0.7f, 1f,
                0.8f, 1f
            );

            float tempo = 0f;

            while (tempo < duracaoFade)
            {
                tempo += Time.deltaTime;

                float progresso = Mathf.Clamp01(
                    tempo / duracaoFade
                );

                imagem.color = Color.Lerp(
                    corAtual,
                    proximaCor,
                    progresso
                );

                yield return null;
            }

            corAtual = proximaCor;

            yield return new WaitForSeconds(intervalo);
        }
    }
}
