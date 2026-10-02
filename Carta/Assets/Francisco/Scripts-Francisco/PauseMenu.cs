using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameObject pausePanel;
    private bool pausado = false;

    void Start()
    {
        CriarPause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarPause();
        }
    }

    void AlternarPause()
    {
        pausado = !pausado;

        pausePanel.SetActive(pausado);

        if (pausado)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void CriarPause()
    {
        // CANVAS
        GameObject canvasObj = new GameObject("PauseCanvas");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // PAINEL
        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasObj.transform, false);

        Image panelImage = pausePanel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.85f);

        RectTransform panelRect = pausePanel.GetComponent<RectTransform>();

        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;

        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // TEXTO
        GameObject textoObj = new GameObject("PauseText");
        textoObj.transform.SetParent(pausePanel.transform, false);

        Text texto = textoObj.AddComponent<Text>();

        texto.text = "PAUSADO\n\nESC para continuar";
        texto.fontSize = 45;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = Color.white;

        RectTransform textoRect = textoObj.GetComponent<RectTransform>();

        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;

        textoRect.offsetMin = Vector2.zero;
        textoRect.offsetMax = Vector2.zero;

        pausePanel.SetActive(false);
    }
}