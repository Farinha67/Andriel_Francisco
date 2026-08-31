using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float forcaMotor = 25f;
    [SerializeField] private float velocidadeMaxima = 20f;
    [SerializeField] private float velocidadeRotacao = 80f;
    [SerializeField] private float freio = 5f;

    [Header("Física & Estabilidade")]
    [SerializeField] private float massa = 1200f;
    [SerializeField] private float drag = 0.2f;
    [SerializeField] private float angularDrag = 3f;

    [Header("Detecção do chão")]
    [SerializeField] private LayerMask camadaTerreno;
    [SerializeField] private float distanciaChao = 1.5f;

    [Header("Câmera")]
    [SerializeField] private Camera cameraCarro;

    [Header("Saída do carro")]
    public Transform exitPoint;

    [Header("Som")]
    [SerializeField] private AudioSource audioMotor;

    private Rigidbody rb;

    private float movimento;
    private float direcao;

    private bool estaDirigindo = false;
    private bool estaNoChao = false;

    public bool EstaDirigindo => estaDirigindo;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = massa;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;

        rb.useGravity = true;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

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
        direcao = Input.GetAxis("Horizontal");

        ControlarSom();
    }

    private void FixedUpdate()
    {
        if (!estaDirigindo)
            return;

        ChecarChao();
        MovimentoFisico();
        RotacaoFisica();
        LimitarVelocidade();
    }

    private void ChecarChao()
    {
        Vector3 origem = transform.position;

        estaNoChao = Physics.Raycast(
            origem,
            Vector3.down,
            distanciaChao,
            camadaTerreno,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawRay(
            origem,
            Vector3.down * distanciaChao,
            estaNoChao ? Color.green : Color.red
        );
    }

    private void MovimentoFisico()
    {
        if (!estaNoChao)
            return;

        if (Mathf.Abs(movimento) > 0.01f)
        {
            Vector3 forca =
                transform.forward *
                movimento *
                forcaMotor;

            rb.AddForce(
                forca,
                ForceMode.Acceleration
            );
        }
        else
        {
            Vector3 velocidade = rb.linearVelocity;

            Vector3 velocidadeHorizontal =
                new Vector3(
                    velocidade.x,
                    0f,
                    velocidade.z
                );

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
        if (!estaNoChao)
            return;

        if (Mathf.Abs(movimento) < 0.05f)
            return;

        float velocidadeAtual =
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            ).magnitude;

        if (velocidadeAtual < 0.1f)
            return;

        float fatorVelocidade =
            Mathf.Clamp01(
                velocidadeAtual / velocidadeMaxima
            );

        float rotacao =
            direcao *
            velocidadeRotacao *
            fatorVelocidade *
            Time.fixedDeltaTime;

        Quaternion novaRotacao =
            rb.rotation *
            Quaternion.Euler(
                0f,
                rotacao,
                0f
            );

        rb.MoveRotation(novaRotacao);
    }

    private void LimitarVelocidade()
    {
        Vector3 velocidade = rb.linearVelocity;

        Vector3 velocidadeHorizontal =
            new Vector3(
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
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            ).magnitude > 0.5f;

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

            AudioListener listener =
                cameraCarro.GetComponent<AudioListener>();

            if (listener != null)
                listener.enabled = true;
        }

        Debug.Log("Modo dirigir ATIVADO");
    }

    public void DesativarModoDirigir()
    {
        estaDirigindo = false;

        movimento = 0f;
        direcao = 0f;

        if (audioMotor != null &&
            audioMotor.isPlaying)
        {
            audioMotor.Stop();
        }

        if (cameraCarro != null)
        {
            AudioListener listener =
                cameraCarro.GetComponent<AudioListener>();

            if (listener != null)
                listener.enabled = false;

            cameraCarro.enabled = false;
        }

        Debug.Log("Modo dirigir DESATIVADO");
    }
}