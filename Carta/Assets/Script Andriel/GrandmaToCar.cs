
using UnityEngine;

public class GrandmaToCar : MonoBehaviour
{
    public Transform pontoCarro;
    public GameObject tapete;
    public Transform pontoTapete;
    public float velocidade = 2f;

    private bool andando;
    private bool terminou;

    private Rigidbody rb;
    private Collider[] colliders;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void IrAteCarro()
    {
        if (andando || terminou)
            return;

        if (pontoCarro == null)
        {
            Debug.LogError("GrandmaToCar: falta atribuir o Ponto Carro.");
            return;
        }

        andando = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        foreach (Collider col in colliders)
            col.enabled = false;
    }

    void Update()
    {
        if (!andando || terminou)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            pontoCarro.position,
            velocidade * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, pontoCarro.position) <= 0.15f)
            FinalizarChegada();
    }

    void FinalizarChegada()
    {
        andando = false;
        terminou = true;

        transform.position = pontoCarro.position;

        if (tapete != null)
        {
            tapete.SetActive(true);
        }
        else
        {
            Debug.LogError("GrandmaToCar: falta atribuir o Tapete.");
        }

        gameObject.SetActive(false);

        if (Scene1Manager.Instance != null)
            Scene1Manager.Instance.AvoChegouAoCarro();
    }
}
