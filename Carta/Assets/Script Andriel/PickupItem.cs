
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Transform handPoint;
    public float pickupRange = 5f;

    private GameObject heldItem;
    private Rigidbody heldRb;
    private Collider[] heldColliders;

    private CarDropZone carDropZone;

    public bool TemCaixa()
    {
        return heldItem != null;
    }

    public bool EstaSegurando(GameObject objeto)
    {
        if (heldItem == null || objeto == null)
            return false;

        return objeto == heldItem ||
               objeto.transform.IsChildOf(heldItem.transform);
    }

    void Start()
    {
        carDropZone = FindFirstObjectByType<CarDropZone>();
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (heldItem == null)
        {
            TryPickup();
        }
        else
        {
            // Se estiver perto do carro, coloca a caixa.
            if (carDropZone != null && carDropZone.JogadorPerto())
            {
                carDropZone.ColocarCaixa(this);
            }
            else
            {
                // Se estiver longe, solta a caixa.
                DropItem();
            }
        }
    }

    void TryPickup()
    {
        if (handPoint == null)
        {
            Debug.LogError("PickupItem: HandPoint não configurado!");
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward,
            out hit, pickupRange))
        {
            if (!hit.collider.CompareTag("Pickup"))
                return;

            heldRb = hit.collider.attachedRigidbody;

            heldItem = heldRb != null
                ? heldRb.gameObject
                : hit.collider.gameObject;

            heldColliders = heldItem.GetComponentsInChildren<Collider>();

            if (heldRb != null)
            {
                heldRb.isKinematic = true;
                heldRb.useGravity = false;
                heldRb.linearVelocity = Vector3.zero;
                heldRb.angularVelocity = Vector3.zero;
            }

            foreach (Collider col in heldColliders)
                col.enabled = false;

            heldItem.transform.SetParent(handPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
        }
    }

    public void DepositarCaixa()
    {
        if (heldItem == null)
            return;

        GameObject caixa = heldItem;

        heldItem = null;
        heldRb = null;
        heldColliders = null;

        Destroy(caixa);
    }

    void DropItem()
    {
        if (heldItem == null)
            return;

        GameObject caixa = heldItem;

        caixa.transform.SetParent(null);

        caixa.transform.position =
            transform.position + transform.forward * 1.2f;

        if (heldColliders != null)
        {
            foreach (Collider col in heldColliders)
                col.enabled = true;
        }

        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.useGravity = true;
            heldRb.linearVelocity = Vector3.zero;
            heldRb.angularVelocity = Vector3.zero;
        }

        heldItem = null;
        heldRb = null;
        heldColliders = null;
    }
}
