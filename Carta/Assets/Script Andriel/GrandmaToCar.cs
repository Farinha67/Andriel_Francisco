using UnityEngine;

public class GrandmaToCar : MonoBehaviour
{
    [Header("PONTO ONDE A VÓ VAI PARAR")]
    public Transform pontoCarro;

    [Header("TAPETE")]
    public GameObject tapete;
    public Transform pontoTapete;

    [Header("MOVIMENTO")]
    public float velocidade = 2f;
    public float distanciaParaChegar = 0.15f;

    private bool andando = false;
    private bool terminou = false;

    private Rigidbody rb;
    private Collider[] colliders;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (!andando || terminou)
            return;

        if (pontoCarro == null)
        {
            Debug.LogError("GrandmaToCar: O Ponto Carro não foi colocado!");
            andando = false;
            return;
        }

        // Movimento até o ponto
        transform.position = Vector3.MoveTowards(
            transform.position,
            pontoCarro.position,
            velocidade * Time.deltaTime
        );

        // Faz a vó olhar para o ponto
        Vector3 direcao = pontoCarro.position - transform.position;
        direcao.y = 0;

        if (direcao != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direcao);
        }

        // Chegou
        if (Vector3.Distance(transform.position, pontoCarro.position) <= distanciaParaChegar)
        {
            ChegouNoCarro();
        }
    }

    public void IrAteCarro()
    {
        if (terminou)
            return;

        andando = true;

        // Desliga a física enquanto ela anda
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void ChegouNoCarro()
    {
        terminou = true;
        andando = false;

        // Coloca exatamente no ponto
        transform.position = pontoCarro.position;

        // Para completamente a física
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Desliga todos os colliders da vó
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }

        // Mostra o tapete
        if (tapete != null)
        {
            tapete.SetActive(true);

            if (pontoTapete != null)
            {
                tapete.transform.position = pontoTapete.position;
                tapete.transform.rotation = pontoTapete.rotation;
            }
        }
        else
        {
            Debug.LogError("GrandmaToCar: O campo TAPETE está vazio!");
        }

        // Some com a vó
        gameObject.SetActive(false);

        Debug.Log("Vó entrou no carro! Tapete apareceu.");
    }
}