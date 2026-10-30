
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("MOTOR")]
    [SerializeField] private float aceleracao = 22f;
    [SerializeField] private float velocidadeMaxima = 25f;
    [SerializeField] private float velocidadeRe = 10f;

    [Header("DIRECAO")]
    [SerializeField] private float velocidadeRotacao = 90f;
    [SerializeField] private float velocidadeMinimaParaVirar = 0.3f;

    [Header("FREIO")]
    [SerializeField] private float forcaFreio = 12f;
    [SerializeField] private float desaceleracao = 2f;

    [Header("ADERENCIA")]
    [SerializeField] private float aderenciaLateral = 5f;

    [Header("FISICA")]
    [SerializeField] private float massa = 1200f;
    [SerializeField] private float drag = 0.15f;
    [SerializeField] private float angularDrag = 2f;

    [Header("CAMERA")]
    [SerializeField] private Camera cameraCarro;

    [Header("SAIDA DO CARRO")]
    public Transform exitPoint;

    [Header("SOM")]
    [SerializeField] private AudioSource audioMotor;

    private Rigidbody rb;

    private float movimento;
    private float direcao;

    private bool estaDirigindo;

    public bool EstaDirigindo => estaDirigindo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ConfigurarFisica();
        DesativarModoDirigir();
    }

    private void ConfigurarFisica()
    {
        rb.mass = massa;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;

        rb.useGravity = true;
        rb.isKinematic = false;

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        rb.WakeUp();

        if (audioMotor != null)
        {
            audioMotor.loop = true;
            audioMotor.Stop();
        }
    }

    private void Update()
    {
        if (!estaDirigindo)
            return;

        movimento = Input.GetAxisRaw("Vertical");
        direcao = Input.GetAxisRaw("Horizontal");

        ControlarSom();
    }

    private void FixedUpdate()
    {
        if (!estaDirigindo)
            return;

        Acelerar();
        AplicarFreio();
        AplicarAderencia();
        GirarCarro();
        LimitarVelocidade();
    }

    private void Acelerar()
    {
        if (Mathf.Abs(movimento) < 0.01f)
            return;

        float velocidadeFrente = Vector3.Dot(
            rb.linearVelocity,
            transform.forward
        );

        // Limite de velocidade para frente.
        if (movimento > 0 && velocidadeFrente >= velocidadeMaxima)
            return;

        // Limite de velocidade para ré.
        if (movimento < 0 && velocidadeFrente <= -velocidadeRe)
            return;

        rb.AddForce(
            transform.forward * movimento * aceleracao,
            ForceMode.Acceleration
        );
    }

    private void AplicarFreio()
    {
        Vector3 velocidade = rb.linearVelocity;

        Vector3 horizontal = new Vector3(
            velocidade.x,
            0f,
            velocidade.z
        );

        // Freio ao apertar espaço.
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 novaVelocidade = Vector3.MoveTowards(
                horizontal,
                Vector3.zero,
                forcaFreio * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector3(
                novaVelocidade.x,
                velocidade.y,
                novaVelocidade.z
            );

            return;
        }

        // Desaceleração natural quando solta o acelerador.
        if (Mathf.Abs(movimento) < 0.01f)
        {
            Vector3 novaVelocidade = Vector3.MoveTowards(
                horizontal,
                Vector3.zero,
                desaceleracao * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector3(
                novaVelocidade.x,
                velocidade.y,
                novaVelocidade.z
            );
        }
    }

    private void AplicarAderencia()
    {
        Vector3 velocidadeLateral =
            transform.right *
            Vector3.Dot(rb.linearVelocity, transform.right);

        rb.AddForce(
            -velocidadeLateral * aderenciaLateral,
            ForceMode.Acceleration
        );
    }

    private void GirarCarro()
    {
        float velocidadeAtual = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        ).magnitude;

        if (velocidadeAtual < velocidadeMinimaParaVirar)
            return;

        float intensidade = Mathf.Clamp01(
            velocidadeAtual / 5f
        );

        float sentido = movimento < 0f ? -1f : 1f;

        float rotacao = direcao *
                        velocidadeRotacao *
                        intensidade *
                        sentido *
                        Time.fixedDeltaTime;

        Quaternion novaRotacao =
            rb.rotation *
            Quaternion.Euler(0f, rotacao, 0f);

        rb.MoveRotation(novaRotacao);
    }

    private void LimitarVelocidade()
    {
        Vector3 velocidade = rb.linearVelocity;

        Vector3 horizontal = new Vector3(
            velocidade.x,
            0f,
            velocidade.z
        );

        float limite = movimento < 0f
            ? velocidadeRe
            : velocidadeMaxima;

        if (horizontal.magnitude > velocidadeMaxima)
        {
            horizontal = horizontal.normalized * velocidadeMaxima;
        }

        // Mantém a velocidade de ré controlada.
        float velocidadeFrente = Vector3.Dot(
            horizontal,
            transform.forward
        );

        if (velocidadeFrente < -velocidadeRe)
        {
            horizontal = transform.forward * -velocidadeRe;
        }

        rb.linearVelocity = new Vector3(
            horizontal.x,
            velocidade.y,
            horizontal.z
        );
    }

    private void ControlarSom()
    {
        if (audioMotor == null)
            return;

        bool movendo =
            Mathf.Abs(movimento) > 0.1f ||
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            ).magnitude > 0.5f;

        if (movendo)
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

        movimento = 0f;
        direcao = 0f;

        rb.isKinematic = false;
        rb.WakeUp();

        if (cameraCarro != null)
        {
            cameraCarro.enabled = true;

            AudioListener listener =
                cameraCarro.GetComponent<AudioListener>();

            if (listener != null)
                listener.enabled = true;
        }

        Debug.Log("Modo dirigir ATIVADO!");
    }

    public void DesativarModoDirigir()
    {
        estaDirigindo = false;

        movimento = 0f;
        direcao = 0f;

        if (audioMotor != null)
            audioMotor.Stop();

        if (cameraCarro != null)
        {
            AudioListener listener =
                cameraCarro.GetComponent<AudioListener>();

            if (listener != null)
                listener.enabled = false;

            cameraCarro.enabled = false;
        }

        Debug.Log("Modo dirigir DESATIVADO!");
    }
}
