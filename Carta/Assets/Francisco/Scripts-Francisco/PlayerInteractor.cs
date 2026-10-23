
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera cameraJogador;
    [SerializeField] private InventoryManager inventario;
    [SerializeField] private GameObject painelLoja;

    [Header("Coleta")]
    [SerializeField] private float alcance = 4f;
    [SerializeField] private LayerMask camadasColetaveis = ~0;

    private void Update()
    {
        if (painelLoja != null && painelLoja.activeInHierarchy)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TentarColetar();
        }
    }

    private void TentarColetar()
    {
        if (cameraJogador == null || inventario == null)
            return;

        Ray raio = cameraJogador.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(
            raio,
            out RaycastHit hit,
            alcance,
            camadasColetaveis,
            QueryTriggerInteraction.Ignore))
        {
            WorldItem item = hit.collider.GetComponentInParent<WorldItem>();

            if (item == null)
                return;

            if (!item.CompareTag("Item"))
                return;

            inventario.AdicionarItem(item.Dados);
            Destroy(item.gameObject);
        }
    }
}
