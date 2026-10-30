
using UnityEngine;
using System.Collections;

public class CenaDelegacia : MonoBehaviour
{
    [Header("JOGADOR")]
    public Transform jogador;
    public float distanciaInteracao = 3f;

    [Header("OFICIAL")]
    public Transform oficial;

    [Header("CARRO DA POLICIA")]
    public GameObject carroPolicia;
    public Transform pontoChegada;
    public float velocidadeCarro = 8f;

    [Header("PRISAO")]
    public Transform pontoPrisao;
    public float duracaoFade = 2f;

    private bool podeConversar;
    private bool conversando;
    private bool fim;

    private string mensagemObjetivo =
        "Vá à delegacia e fale com um oficial.";

    private string mensagemDialogo = "";

    private float fadeAlpha = 0f;

    private GUIStyle estilo;

    void Start()
    {
        if (carroPolicia != null)
            carroPolicia.SetActive(false);
    }

    void Update()
    {
        if (jogador == null || oficial == null)
            return;

        if (conversando || fim)
            return;

        float distancia = Vector3.Distance(
            jogador.position,
            oficial.position
        );

        podeConversar = distancia <= distanciaInteracao;

        if (podeConversar && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SequenciaDelegacia());
        }
    }

    IEnumerator SequenciaDelegacia()
    {
        conversando = true;
        mensagemObjetivo = "";

        mensagemDialogo = "Jogador: Preciso do meu carro de volta.";
        yield return new WaitForSeconds(3f);

        mensagemDialogo =
            "Oficial: Estamos fazendo a última revista no veículo.";
        yield return new WaitForSeconds(4f);

        mensagemDialogo = "Jogador: OK...";
        yield return new WaitForSeconds(2f);

        mensagemDialogo = "";

        // CHEGADA DO CARRO

        if (carroPolicia != null)
        {
            carroPolicia.SetActive(true);

            if (pontoChegada != null)
            {
                while (Vector3.Distance(
                    carroPolicia.transform.position,
                    pontoChegada.position) > 0.2f)
                {
                    Vector3 direcao =
                        pontoChegada.position -
                        carroPolicia.transform.position;

                    direcao.y = 0;

                    if (direcao.sqrMagnitude > 0.001f)
                    {
                        Quaternion rotacao =
                            Quaternion.LookRotation(direcao);

                        carroPolicia.transform.rotation =
                            Quaternion.Slerp(
                                carroPolicia.transform.rotation,
                                rotacao,
                                Time.deltaTime * 3f
                            );
                    }

                    carroPolicia.transform.position =
                        Vector3.MoveTowards(
                            carroPolicia.transform.position,
                            pontoChegada.position,
                            velocidadeCarro * Time.deltaTime
                        );

                    yield return null;
                }
            }
        }

        yield return new WaitForSeconds(1.5f);

        mensagemDialogo =
            "Policial: Você está preso por homicídio culposo!";

        yield return new WaitForSeconds(4f);

        mensagemDialogo = "";

        // FADE OUT

        yield return StartCoroutine(Fade(1f));

        // TELETRANSPORTE PARA A PRISAO

        if (pontoPrisao != null && jogador != null)
        {
            CharacterController controle =
                jogador.GetComponent<CharacterController>();

            if (controle != null)
                controle.enabled = false;

            jogador.SetPositionAndRotation(
                pontoPrisao.position,
                pontoPrisao.rotation
            );

            if (controle != null)
                controle.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        // FADE IN

        yield return StartCoroutine(Fade(0f));

        mensagemDialogo =
            "Jogador: Acabou... Eu sabia que poderia acontecer isso...";

        yield return new WaitForSeconds(5f);

        mensagemDialogo = "FIM";

        fim = true;
    }

    IEnumerator Fade(float destino)
    {
        float inicio = fadeAlpha;
        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;

            fadeAlpha = Mathf.Lerp(
                inicio,
                destino,
                tempo / duracaoFade
            );

            yield return null;
        }

        fadeAlpha = destino;
    }

    void OnGUI()
    {
        if (estilo == null)
        {
            estilo = new GUIStyle(GUI.skin.label);
            estilo.fontSize = 22;
            estilo.fontStyle = FontStyle.Bold;
            estilo.alignment = TextAnchor.MiddleCenter;
            estilo.normal.textColor = Color.white;
            estilo.wordWrap = true;
        }

        if (!string.IsNullOrEmpty(mensagemObjetivo))
        {
            GUI.Box(new Rect(
                Screen.width / 2 - 300,
                Screen.height - 100, 600, 55), "");

            GUI.Label(new Rect(
                Screen.width / 2 - 300,
                Screen.height - 100, 600, 55),
                mensagemObjetivo, estilo);
        }

        if (podeConversar && !conversando && !fim)
        {
            GUI.Box(new Rect(
                Screen.width / 2 - 250,
                Screen.height - 100, 500, 55), "");

            GUI.Label(new Rect(
                Screen.width / 2 - 250,
                Screen.height - 100, 500, 55),
                "Aperte E para falar com um oficial", estilo);
        }

        if (!string.IsNullOrEmpty(mensagemDialogo))
        {
            GUI.Box(new Rect(
                Screen.width / 2 - 400,
                Screen.height - 130, 800, 90), "");

            GUI.Label(new Rect(
                Screen.width / 2 - 390,
                Screen.height - 125, 780, 80),
                mensagemDialogo, estilo);
        }

        // TELA PRETA

        if (fadeAlpha > 0f)
        {
            Color anterior = GUI.color;

            GUI.color = new Color(0, 0, 0, fadeAlpha);

            GUI.DrawTexture(
                new Rect(0, 0, Screen.width, Screen.height),
                Texture2D.whiteTexture
            );

            GUI.color = anterior;
        }
    }
}
