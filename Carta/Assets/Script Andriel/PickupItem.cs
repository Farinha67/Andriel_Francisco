
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
        // Procura o HandPoint automaticamente.
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

        // Procura a camera automaticamente.
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
            // Perto do carro: coloca a caixa.
            if (carDropZone != null &&
                carDropZone.JogadorPerto())
            {
                carDropZone.ColocarCaixa(this);
            }
            else
            {
                // Longe do carro: solta a caixa.
                DropItem();
            }
        }
    }

    void TryPickup()
    {
        if (handPoint == null || cameraTransform == null)
            return;

        RaycastHit hit;

        // Detecta o objeto na direcao da camera.
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

            // Verifica a tag na caixa ou no objeto atingido.
            if (!objeto.CompareTag("Pickup") &&
                !hit.collider.CompareTag("Pickup"))
                return;

            heldItem = objeto;
            heldRb = rbEncontrado;

            heldColliders =
                heldItem.GetComponentsInChildren<Collider>();

            // Desativa a fisica enquanto segura.
            if (heldRb != null)
            {
                heldRb.linearVelocity = Vector3.zero;
                heldRb.angularVelocity = Vector3.zero;
                heldRb.isKinematic = true;
                heldRb.useGravity = false;
            }

            // Desativa os colliders para evitar conflitos.
            foreach (Collider col in heldColliders)
            {
                col.enabled = false;
            }

            // Coloca a caixa na mao do jogador.
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

        // Limpa as referencias antes de destruir.
        heldItem = null;
        heldRb = null;
        heldColliders = null;

        Destroy(caixa);

        Debug.Log("Caixa colocada no carro!");
    }

    void DropItem()
    {
        if (heldItem == null)
            return;

        GameObject caixa = heldItem;

        // Retira a caixa da mao.
        caixa.transform.SetParent(null);

        caixa.transform.position =
            transform.position + transform.forward * 1.5f;

        // Reativa os colliders.
        if (heldColliders != null)
        {
            foreach (Collider col in heldColliders)
            {
                if (col != null)
                    col.enabled = true;
            }
        }

        // Reativa a fisica.
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

        Debug.Log("Caixa solta!");
    }
}
