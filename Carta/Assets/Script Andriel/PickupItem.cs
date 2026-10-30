
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Referencias")]
    public Transform handPoint;
    public Transform cameraTransform;

    [Header("Configuracoes")]
    public float pickupRange = 5f;

    private GameObject heldItem;
    private Rigidbody heldRb;
    private Collider[] heldColliders;

    private CarDropZone carDropZone;

    void Awake()
    {
        if (handPoint == null)
        {
            Transform[] objetos =
                GetComponentsInChildren<Transform>(true);

            foreach (Transform objeto in objetos)
            {
                if (objeto.name == "HandPoint")
                {
                    handPoint = objeto;
                    break;
                }
            }
        }

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();

            if (cam != null)
                cameraTransform = cam.transform;
            else
                cameraTransform = Camera.main != null
                    ? Camera.main.transform
                    : transform;
        }

        if (handPoint == null)
        {
            Debug.LogError(
                "PickupItem: HandPoint nao encontrado no objeto: "
                + gameObject.name
            );
        }
    }

    void Start()
    {
        carDropZone = FindFirstObjectByType<CarDropZone>();

        if (carDropZone == null)
        {
            Debug.LogError(
                "PickupItem: nenhum CarDropZone encontrado na cena!"
            );
        }
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
            if (carDropZone != null &&
                carDropZone.JogadorPerto())
            {
                carDropZone.ColocarCaixa(this);
            }
        }
    }

    void TryPickup()
    {
        if (handPoint == null || cameraTransform == null)
            return;

        RaycastHit hit;

        if (Physics.Raycast(
            cameraTransform.position,
            cameraTransform.forward,
            out hit,
            pickupRange))
        {
            Rigidbody rbEncontrado =
                hit.collider.attachedRigidbody;

            GameObject objeto = rbEncontrado != null
                ? rbEncontrado.gameObject
                : hit.collider.gameObject;

            if (!objeto.CompareTag("Pickup") &&
                !hit.collider.CompareTag("Pickup"))
                return;

            heldItem = objeto;
            heldRb = rbEncontrado;

            heldColliders =
                heldItem.GetComponentsInChildren<Collider>();

            if (heldRb != null)
            {
                heldRb.linearVelocity = Vector3.zero;
                heldRb.angularVelocity = Vector3.zero;
                heldRb.isKinematic = true;
                heldRb.useGravity = false;
            }

            foreach (Collider col in heldColliders)
            {
                col.enabled = false;
            }

            heldItem.transform.SetParent(handPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;

            Debug.Log("Caixa pega com sucesso!");
        }
    }

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

    public void DepositarCaixa()
    {
        if (heldItem == null)
            return;

        GameObject caixa = heldItem;

        heldItem = null;
        heldRb = null;
        heldColliders = null;

        Destroy(caixa);

        Debug.Log("Caixa colocada no carro!");
    }
}
