
using UnityEngine;

public class NPCShopInteraction : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera cameraJogador;
    [SerializeField] private GameObject painelLoja;

    [Header("Interação")]
    [SerializeField] private float alcance = 3f;
    [SerializeField] private KeyCode teclaInteração = KeyCode.E;
    [SerializeField] private LayerMask camadasNPC = ~0;

    private bool lojaAberta;

    private void Start()
    {
        if (painelLoja != null)
            painelLoja.SetActive(false);
    }

    private void Update()
    {
        if (cameraJogador == null || painelLoja == null)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && lojaAberta)
        {
            FecharLoja();
            return;
        }

        if (lojaAberta)
            return;

        if (Input.GetKeyDown(teclaInteração))
        {
            TentarAbrirLoja();
        }
    }

    private void TentarAbrirLoja()
    {
        Ray raio = cameraJogador.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(
            raio,
            out RaycastHit hit,
            alcance,
            camadasNPC,
            QueryTriggerInteraction.Ignore))
        {
            Transform alvo = hit.collider.transform;

            if (alvo.CompareTag("NPC") ||
                alvo.root.CompareTag("NPC"))
            {
                AbrirLoja();
            }
        }
    }

    public void AbrirLoja()
    {
        lojaAberta = true;
        painelLoja.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FecharLoja()
    {
        lojaAberta = false;
        painelLoja.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}