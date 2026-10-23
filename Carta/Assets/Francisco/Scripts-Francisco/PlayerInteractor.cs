
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera cameraJogador;
    [SerializeField] private InventoryManager inventario;
    [SerializeField] private GameObject painelLoja;

    [Header("Coleta")]
    [SerializeField] private float alcance = 10f;
    [SerializeField] private LayerMask camadasColetaveis = ~0;

    private void Update()
    {
        if (painelLoja != null && painelLoja.activeInHierarchy)
            return;

        if (Input.GetMouseButtonDown(0))
            TentarColetar();
    }

    private void TentarColetar()
    {
        if (cameraJogador == null || inventario == null)
        {
            Debug.LogError("Falta atribuir a Camera ou o Inventario!");
            return;
        }

        Ray raio = cameraJogador.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Debug.DrawRay(raio.origin, raio.direction * alcance, Color.red, 2f);

        if (!Physics.Raycast(raio, out RaycastHit hit, alcance,
            camadasColetaveis, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("O raio não acertou nenhum objeto coletável.");
            return;
        }

        Debug.Log("Raio acertou: " + hit.collider.name);

        WorldItem item = hit.collider.GetComponentInParent<WorldItem>();

        if (item == null)
        {
            Debug.LogWarning(
                "O objeto atingido não possui WorldItem: " +
                hit.collider.name
            );
            return;
        }

        if (!item.CompareTag("Item"))
        {
            Debug.LogWarning("O objeto não está com a tag Item.");
            return;
        }

        if (item.Dados == null)
        {
            Debug.LogError("O WorldItem está sem ItemData!");
            return;
        }

        inventario.AdicionarItem(item.Dados);
        Debug.Log("Item coletado: " + item.Dados.nomeItem);

        Destroy(item.gameObject);
    }
}
