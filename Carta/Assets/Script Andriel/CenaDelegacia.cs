
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CenaDelegacia : MonoBehaviour
{
    [Header("JOGADOR")]
    public Transform jogador;

    [Header("OFICIAL")]
    public Transform oficial;
    public float distanciaInteracao = 3f;

    [Header("VIATURA")]
    public GameObject carroPolicia;
    public Transform pontoChegada;
    public Transform pontoEntradaCarro;
    public float velocidadeCarro = 8f;
    public float distanciaCarro = 3f;

    [Header("PROXIMA CENA")]
    public string nomeCenaInterrogatorio = "Interrogatorio";
    public float duracaoFade = 4f;

    [Header("DIALOGOS")]
    public float tempoDialogo = 3f;

    private bool conversando;
    private bool dialogoConcluido;
    private bool aguardandoEntrada;
    private bool podeConversar;
    private bool podeEntrar;
    private bool fim;

    private string mensagemObjetivo =
        "Vá à delegacia e fale com um oficial.";

    private string mensagemDialogo = "";

    private float fadeAlpha;

    private GUIStyle estilo;

    void Start()
    {
        if (carroPolicia != null)
            carroPolicia.SetActive(false);
    }

    void Update()
    {
        if (jogador == null || fim)
            return;

        // NÃO PERMITE FALAR COM O OFICIAL NOVAMENTE

        if (!dialogoConcluido && !conversando &&
            !aguardandoEntrada && oficial != null)
        {
            float distancia = DistanciaHorizontal(
                jogador.position,
                oficial.position
            );

            podeConversar = distancia <= distanciaInteracao;

            if (podeConversar && Input.GetKeyDown(KeyCode.E))
            {
                dialogoConcluido = true;
                podeConversar = false;

                StartCoroutine(SequenciaDelegacia());
            }
        }
        else
        {
            podeConversar = false;
        }

        // INTERAÇÃO COM A VIATURA

        if (aguardandoEntrada && !conversando)
        {
            Transform alvo = pontoEntradaCarro != null
                ? pontoEntradaCarro
                : carroPolicia != null
                    ? carroPolicia.transform
                    : null;

            if (alvo != null)
            {
                float distancia = DistanciaHorizontal(
                    jogador.position,
                    alvo.position
                );

                podeEntrar = distancia <= distanciaCarro;

                if (podeEntrar && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(EntrarNaViatura());
                }
            }
        }
        else
        {
            podeEntrar = false;
        }
    }

    float DistanciaHorizontal(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;

        return Vector3.Distance(a, b);
    }

    IEnumerator SequenciaDelegacia()
    {
        conversando = true;
        mensagemObjetivo = "";

        mensagemDialogo = "Oficial: Encontramos seu carro.";
        yield return new WaitForSeconds(tempoDialogo);

        mensagemDialogo =
            "Oficial: Mas precisamos conversar sobre uma coisa.";
        yield return new WaitForSeconds(tempoDialogo);

        mensagemDialogo =
            "Oficial: Sua avó foi encontrada morta, enrolada em um tapete que estava sobre seu veículo.";
        yield return new WaitForSeconds(5f);

        mensagemDialogo =
            "Jogador: O QUÊ?! Eu não sabia de nada!";
        yield return new WaitForSeconds(tempoDialogo);

        mensagemDialogo =
            "Oficial: Precisamos entender o que aconteceu.";
        yield return new WaitForSeconds(tempoDialogo);

        mensagemDialogo =
            "Oficial: Me acompanhe até a delegacia. Precisamos fazer algumas perguntas.";
        yield return new WaitForSeconds(4f);

        mensagemDialogo =
            "Oficial: Entre na viatura, por favor.";
        yield return new WaitForSeconds(3f);

        mensagemDialogo = "";

        // CHEGADA DA VIATURA

        if (carroPolicia != null)
        {
            carroPolicia.SetActive(true);

            if (pontoChegada != null)
            {
                while (DistanciaHorizontal(
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

        // AGUARDAR O JOGADOR

        mensagemObjetivo = "Entre na viatura policial.";

        conversando = false;
        aguardandoEntrada = true;
    }

    IEnumerator EntrarNaViatura()
    {
        conversando = true;
        aguardandoEntrada = false;
        podeEntrar = false;

        mensagemObjetivo = "";
        mensagemDialogo = "Indo para a delegacia...";

        yield return new WaitForSeconds(1f);

        // FADE DE 4 SEGUNDOS

        yield return StartCoroutine(Fade(1f));

        fim = true;

        SceneManager.LoadScene(nomeCenaInterrogatorio);
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

        string textoTela = "";

        // PRIORIDADE DAS MENSAGENS

        if (!string.IsNullOrEmpty(mensagemDialogo))
        {
            textoTela = mensagemDialogo;
        }
        else if (aguardandoEntrada)
        {
            if (podeEntrar)
                textoTela = "Aperte E para entrar na viatura";
            else
                textoTela = "Vá até a viatura policial.";
        }
        else if (podeConversar && !dialogoConcluido)
        {
            textoTela = "Aperte E para falar com um oficial";
        }
        else if (!dialogoConcluido && !conversando)
        {
            textoTela = mensagemObjetivo;
        }

        // MOSTRAR APENAS UMA MENSAGEM

        if (!string.IsNullOrEmpty(textoTela))
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2 - 400,
                    Screen.height - 130,
                    800,
                    90
                ), ""
            );

            GUI.Label(
                new Rect(
                    Screen.width / 2 - 390,
                    Screen.height - 125,
                    780,
                    80
                ),
                textoTela,
                estilo
            );
        }

        // FADE PRETO

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
