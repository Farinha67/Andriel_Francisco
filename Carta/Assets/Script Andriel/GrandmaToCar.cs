
using UnityEngine;

public class GrandmaToCar : MonoBehaviour
{
    [Header("Referencias")]
    public Transform pontoCarro;
    public GameObject tapete;
    public Transform pontoTapete;

    [Header("Movimentacao")]
    public float velocidade = 2f;

    [Header("Animacao")]
    [SerializeField] private Animator animator;
    [SerializeField] private string parametroAndando = "Andando";

    private bool andando;
    private bool terminou;

    private Rigidbody rb;
    private Collider[] colliders;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        // Procura o Animator automaticamente.
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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

        // Inicia a animacao de caminhada.
        if (animator != null)
            animator.SetBool(parametroAndando, true);

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

        // Volta para a animacao de parada.
        if (animator != null)
            animator.SetBool(parametroAndando, false);

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
