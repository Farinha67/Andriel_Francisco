using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour
{
    public Transform handPoint;
    public float pickupRange = 5f;

    private GameObject heldItem;
    private bool dialogoAtivo = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !dialogoAtivo)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
            {
                Debug.Log("Acertou: " + hit.collider.name);

                // INTERAÇÃO COM A VÓ
                if (hit.collider.CompareTag("Grandma"))
                {
                    StartCoroutine(DialogoVovo(hit.collider.gameObject));
                    return;
                }
            }

            // SISTEMA NORMAL DAS CAIXAS
            if (heldItem == null)
                TryPickup();
            else
                DropItem();
        }
    }

    IEnumerator DialogoVovo(GameObject grandma)
    {
        dialogoAtivo = true;

        Debug.Log("Neto: Vó, tem certeza que quer ir assim?");
        yield return new WaitForSeconds(2f);

        Debug.Log("Vó: Meu filho, só me enrola no tapete.");
        yield return new WaitForSeconds(2f);

        Debug.Log("Colocando vó no carro...");
        yield return new WaitForSeconds(1f);

        grandma.SetActive(false); // some da cena

        dialogoAtivo = false;
    }

    void TryPickup()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldItem = hit.collider.gameObject;

                Rigidbody rb = heldItem.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.isKinematic = true;

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
            rb.isKinematic = false;

        heldItem = null;
    }
}