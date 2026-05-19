using UnityEngine;

public class CarDropZone : MonoBehaviour
{
    public Transform dropPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            other.transform.position = dropPoint.position;
            other.transform.rotation = dropPoint.rotation;

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}