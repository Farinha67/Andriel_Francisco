using UnityEngine;

public class CarDropZone : MonoBehaviour
{
    public BoxCounter boxCounter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            // Soma 1 no contador
            boxCounter.AddBox();

            // Remove a caixa
            Destroy(other.gameObject);
        }
    }
}