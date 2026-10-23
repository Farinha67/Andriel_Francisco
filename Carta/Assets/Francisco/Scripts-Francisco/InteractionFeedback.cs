
using TMPro;
using UnityEngine;

public class InteractionFeedback : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera cameraJogador;
    [SerializeField] private TMP_Text crosshair;
    [SerializeField] private GameObject promptNPC;
    [SerializeField] private TMP_Text textoPrompt;

    [Header("Alcances")]
    [SerializeField] private float alcanceItem = 10f;
    [SerializeField] private float alcanceNPC = 3f;

    [Header("Cores da mira")]
    [SerializeField] private Color corNormal = Color.white;
    [SerializeField] private Color corItem = Color.green;
    [SerializeField] private Color corNPC = Color.yellow;

    [Header("Detecção")]
    [SerializeField] private LayerMask camadasDetectaveis = ~0;

    private void Start()
    {
        if (promptNPC != null)
            promptNPC.SetActive(false);

        AtualizarMira(corNormal);
    }

    private void Update()
    {
        if (cameraJogador == null)
            return;

        DetectarAlvo();
    }

    private void DetectarAlvo()
    {
        Ray raio = cameraJogador.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        float alcanceMaximo = Mathf.Max(alcanceItem, alcanceNPC);

        bool acertou = Physics.Raycast(
            raio,
            out RaycastHit hit,
            alcanceMaximo,
            camadasDetectaveis,
            QueryTriggerInteraction.Ignore
        );

        bool mirandoNPC = false;
        bool mirandoItem = false;

        if (acertou)
        {
            Transform alvo = hit.collider.transform;

            bool tagNPC = hit.collider.CompareTag("NPC") ||
                          alvo.root.CompareTag("NPC");

            if (tagNPC && hit.distance <= alcanceNPC)
            {
                mirandoNPC = true;
            }
            else
            {
                WorldItem item =
                    hit.collider.GetComponentInParent<WorldItem>();

                if (item != null &&
                    item.CompareTag("Item") &&
                    hit.distance <= alcanceItem)
                {
                    mirandoItem = true;
                }
            }
        }

        if (mirandoNPC)
        {
            AtualizarMira(corNPC);
            MostrarPromptNPC(true);
        }
        else if (mirandoItem)
        {
            AtualizarMira(corItem);
            MostrarPromptNPC(false);
        }
        else
        {
            AtualizarMira(corNormal);
            MostrarPromptNPC(false);
        }
    }

    private void AtualizarMira(Color cor)
    {
        if (crosshair != null)
            crosshair.color = cor;
    }

    private void MostrarPromptNPC(bool mostrar)
    {
        if (promptNPC != null &&
            promptNPC.activeSelf != mostrar)
        {
            promptNPC.SetActive(mostrar);
        }

        if (mostrar && textoPrompt != null)
        {
            textoPrompt.text = "E - Conversar";
        }
    }
}
