
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Scene1Manager : MonoBehaviour
{
    public static Scene1Manager Instance;

    [Header("Vó e tapete")]
    public GrandmaToCar grandma;

    [Header("Próxima cena")]
    public string proximaCena = "Cena2";
    public float tempoTelaPreta = 5f;

    private int caixas = 0;
    private bool conversaLiberada = false;
    private bool conversaIniciada = false;
    private bool descansoLiberado = false;
    private bool dormindo = false;

    private Canvas canvas;
    private TextMeshProUGUI objetivoText;
    private TextMeshProUGUI dialogoText;
    private GameObject painelDialogo;
    private Image fadeImage;

    public bool PodeFalarComAvo()
    {
        return conversaLiberada && !conversaIniciada;
    }

    public bool PodeDescansar()
    {
        return descansoLiberado && !dormindo;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CriarInterface();
    }

    void Start()
    {
        AtualizarObjetivo("Coloque as caixas no carro: 0/5");
    }

    void CriarInterface()
    {
        GameObject canvasObj = new GameObject("InterfaceCena1");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Objetivo no canto superior esquerdo
        GameObject objetivoObj = CriarTexto(
            "Objetivo", canvas.transform, 30,
            TextAlignmentOptions.TopLeft
        );

        RectTransform objetivoRect =
            objetivoObj.GetComponent<RectTransform>();

        objetivoRect.anchorMin = new Vector2(0, 1);
        objetivoRect.anchorMax = new Vector2(0, 1);
        objetivoRect.pivot = new Vector2(0, 1);
        objetivoRect.anchoredPosition = new Vector2(35, -30);
        objetivoRect.sizeDelta = new Vector2(850, 120);

        objetivoText = objetivoObj.GetComponent<TextMeshProUGUI>();

        // Painel de diálogo na parte inferior
        painelDialogo = new GameObject("PainelDialogo");
        painelDialogo.transform.SetParent(canvas.transform, false);

        Image painelImagem = painelDialogo.AddComponent<Image>();
        painelImagem.color = new Color(0, 0, 0, 0.8f);

        RectTransform painelRect =
            painelDialogo.GetComponent<RectTransform>();

        painelRect.anchorMin = new Vector2(0.05f, 0.04f);
        painelRect.anchorMax = new Vector2(0.95f, 0.24f);
        painelRect.offsetMin = Vector2.zero;
        painelRect.offsetMax = Vector2.zero;

        GameObject dialogoObj = CriarTexto(
            "TextoDialogo", painelDialogo.transform, 28,
            TextAlignmentOptions.Center
        );

        RectTransform dialogoRect =
            dialogoObj.GetComponent<RectTransform>();

        dialogoRect.anchorMin = Vector2.zero;
        dialogoRect.anchorMax = Vector2.one;
        dialogoRect.offsetMin = new Vector2(25, 15);
        dialogoRect.offsetMax = new Vector2(-25, -15);

        dialogoText = dialogoObj.GetComponent<TextMeshProUGUI>();
        painelDialogo.SetActive(false);

        // Tela preta para dormir
        GameObject fadeObj = new GameObject("FadeTela");
        fadeObj.transform.SetParent(canvas.transform, false);

        fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;

        RectTransform fadeRect = fadeObj.GetComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.offsetMin = Vector2.zero;
        fadeRect.offsetMax = Vector2.zero;

        fadeObj.transform.SetAsLastSibling();
    }

    GameObject CriarTexto(
        string nome,
        Transform pai,
        int tamanho,
        TextAlignmentOptions alinhamento)
    {
        GameObject obj = new GameObject(nome);
        obj.transform.SetParent(pai, false);

        TextMeshProUGUI texto = obj.AddComponent<TextMeshProUGUI>();
        texto.fontSize = tamanho;
        texto.color = Color.white;
        texto.alignment = alinhamento;
        texto.textWrappingMode = TextWrappingModes.Normal;
        texto.raycastTarget = false;

        return obj;
    }

    void AtualizarObjetivo(string texto)
    {
        if (objetivoText != null)
            objetivoText.text = texto;
    }

    public void CaixaColocada()
    {
        if (conversaLiberada || caixas >= 5)
            return;

        caixas = Mathf.Min(caixas + 1, 5);

        AtualizarObjetivo(
            caixas < 5
            ? "Coloque as caixas no carro: " + caixas + "/5"
            : "Vá falar com sua vó. Aproxime-se e aperte E."
        );

        if (caixas >= 5)
            conversaLiberada = true;
    }

    public IEnumerator Falar(string mensagem)
    {
        painelDialogo.SetActive(true);
        dialogoText.text = mensagem;

        // Cada fala avança sozinha.
        yield return new WaitForSeconds(2.5f);
    }

    public IEnumerator ConversaComAvo()
    {
        if (!PodeFalarComAvo())
            yield break;

        conversaIniciada = true;

        yield return Falar("Neto: Vó, terminei de colocar as caixas no carro.");
        yield return Falar("Vó: Muito bem, meu filho!");
        yield return Falar("Neto: E como a senhora vai viajar?");
        yield return Falar("Vó: Vou enrolada naquele tapete, em cima do carro!");
        yield return Falar("Neto: A senhora só pode estar brincando...");
        yield return Falar("Vó: Anda logo, menino!");

        painelDialogo.SetActive(false);

        if (grandma != null)
        {
            grandma.IrAteCarro();
        }
        else
        {
            Debug.LogError("Arraste a Grandma no campo Grandma do Scene1Manager.");
        }
    }

    public void AvoChegouAoCarro()
    {
        StartCoroutine(SequenciaDescanso());
    }

    IEnumerator SequenciaDescanso()
    {
        yield return Falar("Pronto. Agora está tudo preparado para a viagem.");
        yield return Falar("Estou cansado... o dia foi cheio demais.");
        yield return Falar("Vou dormir um pouco antes de partir.");

        painelDialogo.SetActive(false);

        descansoLiberado = true;
        AtualizarObjetivo("Vá até a cama e aperte E para dormir.");
    }

    public void IniciarSono()
    {
        if (!PodeDescansar())
            return;

        dormindo = true;
        StartCoroutine(DormirETrocarCena());
    }

    IEnumerator DormirETrocarCena()
    {
        AtualizarObjetivo("");
        painelDialogo.SetActive(false);

        // Escurece a tela suavemente.
        float duracaoFade = 2f;
        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;

            Color cor = fadeImage.color;
            cor.a = Mathf.Clamp01(tempo / duracaoFade);
            fadeImage.color = cor;

            yield return null;
        }

        Color preta = fadeImage.color;
        preta.a = 1f;
        fadeImage.color = preta;

        // O jogador dorme por 5 segundos com a tela preta.
        yield return new WaitForSeconds(tempoTelaPreta);

        if (Application.CanStreamedLevelBeLoaded(proximaCena))
        {
            SceneManager.LoadScene(proximaCena);
        }
        else
        {
            Debug.LogError(
                "A cena '" + proximaCena +
                "' não foi encontrada. Confira o nome e o Build Profiles."
            );

            dormindo = false;
        }
    }
}
