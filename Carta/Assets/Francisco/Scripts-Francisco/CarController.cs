using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 15f;
    public float velocidadeRotacao = 100f;

    [Header("Câmera")]
    public Camera cameraCarro;

    [Header("Saída do carro")]
    public Transform exitPoint;

    [Header("Som")]
    public AudioSource audioMotor;


    private Rigidbody rb;

    private float movimento;
    private float direcao;

    private float tempoParado;

    private bool estaDirigindo = false;
    public bool EstaDirigindo => estaDirigindo;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (audioMotor != null)
        {
            audioMotor.loop = true;
            audioMotor.Stop();
        }

        // O jogador começa FORA do carro
        DesativarModoDirigir();
    }


    void Update()
    {
        if (!estaDirigindo)
            return;

        movimento = Input.GetAxis("Vertical");
        direcao = Input.GetAxis("Horizontal");

        bool estaMovendo =
            Mathf.Abs(movimento) > 0.1f ||
            Mathf.Abs(direcao) > 0.1f;

        if (audioMotor != null)
        {
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
    }


    void FixedUpdate()
    {
        if (!estaDirigindo)
            return;


        // Movimento para frente/trás
        Vector3 frente =
            transform.forward *
            movimento *
            velocidade *
            Time.fixedDeltaTime;

        rb.MovePosition(rb.position + frente);


        // Rotação
        float rotacao =
            direcao *
            velocidadeRotacao *
            Time.fixedDeltaTime;

        Quaternion giro =
            Quaternion.Euler(0, rotacao, 0);

        rb.MoveRotation(rb.rotation * giro);
    }


    // =========================================================
    // ENTRAR NO CARRO
    // =========================================================

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


    // =========================================================
    // SAIR / DESATIVAR CARRO
    // =========================================================

    public void DesativarModoDirigir()
    {
        estaDirigindo = false;

        if (cameraCarro != null)
        {
            AudioListener listener =
                cameraCarro.GetComponent<AudioListener>();

            if (listener != null)
                listener.enabled = false;

            cameraCarro.enabled = false;
        }

        if (audioMotor != null && audioMotor.isPlaying)
        {
            audioMotor.Stop();
        }

        Debug.Log("Modo dirigir DESATIVADO");
    }
}