using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float forcaMotor = 25f;
    [SerializeField] private float velocidadeMaxima = 20f;
    [SerializeField] private float velocidadeRotação = 80f;
    [SerializeField] private float freio = 5f;

    [Header("Física & Estabilidade")]
    [SerializeField] private float massa = 1200f;
    [SerializeField] private float drag = 0.2f;
    [SerializeField] private float angularDrag = 3f;
    [SerializeField] private float forcaParaBaixo = 50f; // Prende o carro no chão
    [SerializeField] private LayerMask camadaTerreno;    // Selecione a Layer 'Terrain'
    [SerializeField] private float distanciaChao = 1.2f;  // Distância do centro do carro até o chão

    [Header("Câmera")]
    [SerializeField] private Camera cameraCarro;

    [Header("Saída do carro")]
    public Transform exitPoint;

    [Header("Som")]
    [SerializeField] private AudioSource audioMotor;

    private Rigidbody rb;
    private float movimento;
    private float direção;
    private bool estaDirigindo = false;
    private bool estaNoChao = false;

    public bool EstaDirigindo => estaDirigindo;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = massa;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (audioMotor != null)
        {
            audioMotor.loop = true;
            audioMotor.Stop();
        }

        DesativarModoDirigir();
    }

    private void Update()
    {
        if (!estaDirigindo)
            return;

        movimento = Input.GetAxis("Vertical");
        direção = Input.GetAxis("Horizontal");

        ControlarSom();
    }

    private void FixedUpdate()
    {
        if (!estaDirigindo)
            return;

        ChecarChao();
        AplicarDownforce();
        MovimentoFisico();
        RotacaoFisica();
        LimitarVelocidade();
    }

    private void ChecarChao()
    {
        // Dispara um raio para baixo a partir do centro do carro
        estaNoChao = Physics.Raycast(transform.position, -transform.up, distanciaChao, camadaTerreno);
    }

    private void AplicarDownforce()
    {
        // Se estiver no chão (ou muito perto dele), empurra o carro para baixo
        if (estaNoChao)
        {
            rb.AddForce(-transform.up * forcaParaBaixo, ForceMode.Acceleration);
        }
    }

    private void MovimentoFisico()
    {
        // Só acelera se estiver tocando o chão
        if (estaNoChao && Mathf.Abs(movimento) > 0.01f)
        {
            Vector3 forca = transform.forward * movimento * forcaMotor;
            rb.AddForce(forca, ForceMode.Acceleration);
        }
        else if (estaNoChao)
        {
            Vector3 velocidade = rb.linearVelocity;

            Vector3 velocidadeHorizontal = new Vector3(velocidade.x, 0f, velocidade.z);

            velocidadeHorizontal = Vector3.Lerp(
                velocidadeHorizontal,
                Vector3.zero,
                freio * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector3(
                velocidadeHorizontal.x,
                velocidade.y,
                velocidadeHorizontal.z
            );
        }
    }

    private void RotacaoFisica()
    {
        if (!estaNoChao || Mathf.Abs(movimento) < 0.05f)
            return;

        float velocidadeAtual = rb.linearVelocity.magnitude;

        if (velocidadeAtual < 0.1f)
            return;

        float fatorVelocidade = Mathf.Clamp01(velocidadeAtual / velocidadeMaxima);

        float rotacao =
            direção *
            velocidadeRotação *
            fatorVelocidade *
            Time.fixedDeltaTime;

        Quaternion novaRotacao =
            rb.rotation *
            Quaternion.Euler(0f, rotacao, 0f);

        rb.MoveRotation(novaRotacao);
    }

    private void LimitarVelocidade()
    {
        Vector3 velocidade = rb.linearVelocity;

        Vector3 velocidadeHorizontal = new Vector3(
            velocidade.x,
            0f,
            velocidade.z
        );

        if (velocidadeHorizontal.magnitude > velocidadeMaxima)
        {
            velocidadeHorizontal =
                velocidadeHorizontal.normalized *
                velocidadeMaxima;

            rb.linearVelocity = new Vector3(
                velocidadeHorizontal.x,
                velocidade.y,
                velocidadeHorizontal.z
            );
        }
    }

    private void ControlarSom()
    {
        if (audioMotor == null)
            return;

        bool estaMovendo =
            Mathf.Abs(movimento) > 0.1f ||
            rb.linearVelocity.magnitude > 0.5f;

        if (estaMovendo)
        {
            if (!audioMotor.isPlaying)
                audioMotor.Play();
        }
        else
        {
            if (audioMotor.isPlaying)
                audioMotor.Stop();
        }
    }

    public void AtivarModoDirigir()
    {
        estaDirigindo = true;

        if (cameraCarro != null)
        {
            cameraCarro.enabled = true;

            AudioListener listener = cameraCarro.GetComponent<AudioListener>();

            if (listener != null)
                listener.enabled = true;
        }

        Debug.Log("Modo dirigir ATIVADO");
    }

    public void DesativarModoDirigir()
    {
        estaDirigindo = false;

        movimento = 0f;
        direção = 0f;

        if (audioMotor != null && audioMotor.isPlaying)
        {
            audioMotor.Stop();
        }

        if (cameraCarro != null)
        {
            AudioListener listener = cameraCarro.GetComponent<AudioListener>();

            if (listener != null)
                listener.enabled = false;

            cameraCarro.enabled = false;
        }

        Debug.Log("Modo dirigir DESATIVADO");
    }
}