using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Scene1Manager : MonoBehaviour
{
    public static Scene1Manager Instance;

    [Header("CAIXAS")]
    public int caixasNecessarias = 5;

    private int caixasColocadas = 0;

    [Header("VÓ")]
    public GrandmaToCar grandma;

    [Header("TAPETE")]
    public GameObject tapetePrefab;
    public Transform pontoTapete;

    [Header("PRÓXIMA CENA")]
    public string proximaCena = "Cena2";

    private bool caixasCompletas = false;
    private bool avoTerminou = false;
    private bool podeDormir = false;
    private bool dormindo = false;

    private Canvas canvas;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI objectiveText;
    private GameObject dialoguePanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CriarInterface();

        objectiveText.text =
            "Coloque as caixas no carro: 0/" + caixasNecessarias;
    }

    // =========================================================
    // INTERFACE
    // =========================================================

    void CriarInterface()
    {
        GameObject canvasObj = new GameObject("Canvas_Dinamico");

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        UnityEngine.UI.CanvasScaler scaler =
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        scaler.uiScaleMode =
            UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // PAINEL DO DIÁLOGO

        dialoguePanel = new GameObject("DialoguePanel");

        dialoguePanel.transform.SetParent(
            canvasObj.transform,
            false
        );

        UnityEngine.UI.Image panelImage =
            dialoguePanel.AddComponent<UnityEngine.UI.Image>();

        panelImage.color =
            new Color(0, 0, 0, 0.75f);

        RectTransform panelRect =
            dialoguePanel.GetComponent<RectTransform>();

        panelRect.anchorMin =
            new Vector2(0.05f, 0.05f);

        panelRect.anchorMax =
            new Vector2(0.95f, 0.22f);

        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // TEXTO DO DIÁLOGO

        GameObject textObj =
            new GameObject("DialogueText");

        textObj.transform.SetParent(
            dialoguePanel.transform,
            false
        );

        dialogueText =
            textObj.AddComponent<TextMeshProUGUI>();

        dialogueText.fontSize = 28;
        dialogueText.alignment =
            TextAlignmentOptions.Center;

        dialogueText.color = Color.white;

        RectTransform textRect =
            dialogueText.GetComponent<RectTransform>();

        textRect.anchorMin =
            new Vector2(0.05f, 0.1f);

        textRect.anchorMax =
            new Vector2(0.95f, 0.9f);

        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        dialoguePanel.SetActive(false);

        // OBJETIVO

        GameObject objectiveObj =
            new GameObject("ObjectiveText");

        objectiveObj.transform.SetParent(
            canvasObj.transform,
            false
        );

        objectiveText =
            objectiveObj.AddComponent<TextMeshProUGUI>();

        objectiveText.fontSize = 25;
        objectiveText.alignment =
            TextAlignmentOptions.TopLeft;

        objectiveText.color = Color.white;

        RectTransform objectiveRect =
            objectiveText.GetComponent<RectTransform>();

        objectiveRect.anchorMin =
            new Vector2(0.03f, 0.88f);

        objectiveRect.anchorMax =
            new Vector2(0.6f, 0.98f);

        objectiveRect.offsetMin = Vector2.zero;
        objectiveRect.offsetMax = Vector2.zero;
    }

    // =========================================================
    // CAIXAS
    // =========================================================

    public void CaixaColocada()
    {
        if (caixasCompletas)
            return;

        caixasColocadas++;

        if (caixasColocadas > caixasNecessarias)
            caixasColocadas = caixasNecessarias;

        objectiveText.text =
            "Coloque as caixas no carro: " +
            caixasColocadas +
            "/" +
            caixasNecessarias;

        if (caixasColocadas >= caixasNecessarias)
        {
            caixasCompletas = true;

            StartCoroutine(
                DialogoDepoisDasCaixas()
            );
        }
    }

    IEnumerator DialogoDepoisDasCaixas()
    {
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(
            Falar(
                "Todas as caixas estão prontas."
            )
        );

        yield return StartCoroutine(
            Falar(
                "Preciso falar com a vó antes de sair."
            )
        );

        objectiveText.text =
            "Vá falar com sua vó.";
    }

    // =========================================================
    // PODE FALAR COM A VÓ?
    // =========================================================

    public bool PodeFalarComAvo()
    {
        return caixasCompletas &&
               !avoTerminou &&
               !dormindo;
    }

    // =========================================================
    // DIÁLOGO
    // =========================================================

    public IEnumerator Falar(
        string texto,
        float tempo = 2.5f
    )
    {
        dialoguePanel.SetActive(true);

        dialogueText.text = texto;

        yield return new WaitForSeconds(tempo);

        dialoguePanel.SetActive(false);
    }

    // =========================================================
    // VÓ CHEGOU NO CARRO
    // =========================================================

    public void AvoChegouNoCarro()
    {
        if (avoTerminou)
            return;

        avoTerminou = true;

        StartCoroutine(
            FinalizarParteDaAvo()
        );
    }

    IEnumerator FinalizarParteDaAvo()
    {
        yield return new WaitForSeconds(0.5f);

        // CRIA O TAPETE

        if (tapetePrefab != null &&
            pontoTapete != null)
        {
            Instantiate(
                tapetePrefab,
                pontoTapete.position,
                pontoTapete.rotation
            );
        }

        yield return StartCoroutine(
            Falar(
                "Neto: Pronto... agora sim."
            )
        );

        yield return StartCoroutine(
            Falar(
                "Estou cansado. O dia foi cheio demais."
            )
        );

        yield return StartCoroutine(
            Falar(
                "Vou descansar um pouco."
            )
        );

        objectiveText.text =
            "Vá até a cama descansar.";

        podeDormir = true;
    }

    // =========================================================
    // DORMIR
    // =========================================================

    public bool PodeDormir()
    {
        return podeDormir;
    }

    public void Dormir()
    {
        if (dormindo)
            return;

        dormindo = true;

        StartCoroutine(
            DormirCoroutine()
        );
    }

    IEnumerator DormirCoroutine()
    {
        objectiveText.text = "";

        yield return StartCoroutine(
            Falar(
                "Vou dormir..."
            )
        );

        yield return StartCoroutine(
            FadeParaPreto()
        );

        SceneManager.LoadScene(
            proximaCena
        );
    }

    // =========================================================
    // FADE
    // =========================================================

    IEnumerator FadeParaPreto()
    {
        GameObject fadeObj =
            new GameObject("Fade");

        fadeObj.transform.SetParent(
            canvas.transform,
            false
        );

        UnityEngine.UI.Image image =
            fadeObj.AddComponent<UnityEngine.UI.Image>();

        image.color =
            new Color(0, 0, 0, 0);

        RectTransform rect =
            fadeObj.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Color cor = image.color;

        // ESCURECER

        for (
            float alpha = 0;
            alpha < 1f;
            alpha += Time.deltaTime / 2f
        )
        {
            cor.a = alpha;
            image.color = cor;

            yield return null;
        }

        cor.a = 1f;
        image.color = cor;

        // 5 SEGUNDOS PRETO

        yield return new WaitForSeconds(5f);
    }
}