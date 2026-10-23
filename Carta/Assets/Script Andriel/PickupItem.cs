
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Transform handPoint;
    public float pickupRange = 5f;

    private GameObject heldItem;

    public bool EstaSegurando(GameObject objeto)
    {
        if (heldItem == null || objeto == null)
            return false;

        return objeto == heldItem ||
               objeto.transform.IsChildOf(heldItem.transform);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
                TryPickup();
            else
                DropItem();
        }
    }

    void TryPickup()
    {
        if (handPoint == null)
        {
            Debug.LogError("PickupItem: atribua o Hand Point no Inspector.");
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(
            transform.position,
            transform.forward,
            out hit,
            pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                // Pega o objeto principal da caixa, se houver Rigidbody.
                Rigidbody rb = hit.collider.attachedRigidbody;

                heldItem = rb != null
                    ? rb.gameObject
                    : hit.collider.gameObject;

                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                heldItem.transform.SetParent(handPoint);
                heldItem.transform.localPosition = Vector3.zero;
                heldItem.transform.localRotation = Quaternion.identity;
            }
        }
    }

    void DropItem()
    {
        if (heldItem == null)
            return;

        GameObject caixa = heldItem;
        Rigidbody rb = caixa.GetComponent<Rigidbody>();

        caixa.transform.SetParent(null);

        if (rb != null)
            rb.isKinematic = false;

        heldItem = null;
    }
}
