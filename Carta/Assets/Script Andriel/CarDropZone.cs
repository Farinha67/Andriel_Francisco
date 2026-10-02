using UnityEngine;

public class CarDropZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pickup"))
            return;

        if (Scene1Manager.Instance == null)
            return;

        Scene1Manager.Instance.CaixaColocada();

        Destroy(other.gameObject);
    }
}