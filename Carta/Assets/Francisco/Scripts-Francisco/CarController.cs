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

    [Header("SAIDA")]
    public Transform exitPoint;

    [Header("SOM")]
    [SerializeField] private AudioSource audioMotor;

    [Header("DEBUG")]
    [SerializeField] private bool debugar = true;

    private Rigidbody rb;

    private float movimento;
    private float direcao;

    private bool estaDirigindo;

    public bool EstaDirigindo => estaDirigindo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        AplicarMaterialSemAtrito();
    }

    private void Start()
    {
        rb.mass = massa;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;

        rb.useGravity = true;
        rb.isKinematic = false;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        if (audioMotor != null)
        {
            audioMotor.loop = true;
            audioMotor.Stop();
        }

        // CORRIGIDO: só desativa se ninguém ativou antes
        if (!estaDirigindo)
            DesativarModoDirigir();
    }

    // Colliders com atrito alto prendem o carro no chão
    private void AplicarMaterialSemAtrito()
    {
        PhysicsMaterial mat = new PhysicsMaterial("CarroSemAtrito")
        {
            dynamicFriction = 0f,
            staticFriction = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum,
            bounciness = 0f
        };

        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            if (!c.isTrigger)
                c.material = mat;
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

        if (rb.isKinematic)
        {
            Debug.LogWarning("Rigidbody está KINEMATIC! Algum outro script ativou isso.");
            return;
        }

        Acelerar();
        AplicarFreio();
        AplicarAderencia();
        GirarCarro();
        LimitarVelocidade();

        if (debugar && Mathf.Abs(movimento) > 0.01f)
        {
            Debug.Log($"Vel: {rb.linearVelocity.magnitude:F2} | Pos: {rb.position}");
        }
    }

    private void Acelerar()
    {
        if (Mathf.Abs(movimento) < 0.01f)
            return;

        float velocidadeFrente = Vector3.Dot(
            rb.linearVelocity,
            transform.forward
        );

        if (movimento > 0 && velocidadeFrente >= velocidadeMaxima)
            return;

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
        Vector3 horizontal = new Vector3(velocidade.x, 0f, velocidade.z);

        if (Input.GetKey(KeyCode.Space))
        {
            horizontal = Vector3.MoveTowards(
                horizontal, Vector3.zero, forcaFreio * Time.fixedDeltaTime);
        }
        else if (Mathf.Abs(movimento) < 0.01f)
        {
            horizontal = Vector3.MoveTowards(
                horizontal, Vector3.zero, desaceleracao * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector3(horizontal.x, velocidade.y, horizontal.z);
    }

    private void AplicarAderencia()
    {
        Vector3 lateral = transform.right *
            Vector3.Dot(rb.linearVelocity, transform.right);

        rb.AddForce(-lateral * aderenciaLateral, ForceMode.Acceleration);
    }

    private void GirarCarro()
    {
        float velocidade = new Vector3(
            rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;

        if (velocidade < 0.3f)
            return;

        float intensidade = Mathf.Clamp01(velocidade / 5f);
        float sentido = movimento < 0 ? -1f : 1f;

        float rotacao = direcao * velocidadeRotacao * intensidade *
                        sentido * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotacao, 0f));
    }

    private void LimitarVelocidade()
    {
        Vector3 velocidade = rb.linearVelocity;
        Vector3 horizontal = new Vector3(velocidade.x, 0f, velocidade.z);

        float limite = movimento < 0 ? velocidadeRe : velocidadeMaxima;

        if (horizontal.magnitude > limite)
            horizontal = horizontal.normalized * limite;

        rb.linearVelocity = new Vector3(horizontal.x, velocidade.y, horizontal.z);
    }

    private void ControlarSom()
    {
        if (audioMotor == null)
            return;

        bool acelerando = Mathf.Abs(movimento) > 0.1f;

        if (acelerando && !audioMotor.isPlaying)
            audioMotor.Play();
        else if (!acelerando && audioMotor.isPlaying)
            audioMotor.Stop();
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

            AudioListener listener = cameraCarro.GetComponent<AudioListener>();
            if (listener != null)
                listener.enabled = true;
        }

        Debug.Log("MODO DIRIGIR ATIVADO!");
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
            AudioListener listener = cameraCarro.GetComponent<AudioListener>();
            if (listener != null)
                listener.enabled = false;

            cameraCarro.enabled = false;
        }

        Debug.Log("MODO DIRIGIR DESATIVADO!");
    }
}