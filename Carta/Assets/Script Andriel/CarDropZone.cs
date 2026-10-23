
using UnityEngine;
using System.Collections.Generic;

public class CarDropZone : MonoBehaviour
{
    public BoxCounter boxCounter;

    private HashSet<GameObject> caixasContabilizadas = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pickup"))
            return;

        // Não aceita caixas que ainda estão sendo seguradas.
        PickupItem jogador = FindFirstObjectByType<PickupItem>();

        if (jogador != null && jogador.EstaSegurando(other.gameObject))
            return;

        // Impede contar a mesma caixa mais de uma vez.
        GameObject caixa = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (caixasContabilizadas.Contains(caixa))
            return;

        if (boxCounter == null || Scene1Manager.Instance == null)
        {
            Debug.LogError(
                "CarDropZone: atribua BoxCounter e confira o Scene1Manager."
            );
            return;
        }

        caixasContabilizadas.Add(caixa);

        boxCounter.AddBox();
        Scene1Manager.Instance.CaixaColocada();

        Destroy(caixa);
    }
}
