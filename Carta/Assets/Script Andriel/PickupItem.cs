using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Transform handPoint;
    public float pickupRange = 5f;

    private GameObject heldItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                TryPickup();
            }
            else
            {
                DropItem();
            }
        }
    }

    void TryPickup()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
        {
            Debug.Log("Acertou: " + hit.collider.name);

            if (hit.collider.CompareTag("Pickup"))
            {
                heldItem = hit.collider.gameObject;

                Rigidbody rb = heldItem.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                heldItem.transform.position = handPoint.position;
                heldItem.transform.parent = handPoint;
            }
        }
    }

    void DropItem()
    {
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();

        heldItem.transform.parent = null;

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        heldItem = null;
    }
}   