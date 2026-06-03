using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 15f;
    public float velocidadeRotacao = 100f;

    [Header("Som")]
    public AudioSource audioMotor;

    private Rigidbody rb;
    private float movimento;
    private float direcao;

    [Header("Reset")]
    [SerializeField]private float tempoParaResetar = 25f;

    private float tempoParado;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (audioMotor != null)
        {
            audioMotor.loop = true;
            audioMotor.Stop();
        }
    }

    void Update()
    {
        movimento = Input.GetAxis("Vertical");
        direcao = Input.GetAxis("Horizontal");

        bool estaMovendo = Mathf.Abs(movimento) > 0.1f || Mathf.Abs(direcao) > 0.1f;

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


        if (rb.linearVelocity.magnitude < 0.2f)
        {
            tempoParado += Time.deltaTime;

            if (tempoParado >= tempoParaResetar)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            tempoParado = 0f;
        }
        if (Vector3.Dot(transform.up, Vector3.down) > 0.7f)
        {
            tempoParado += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        Vector3 frente = transform.forward * movimento * velocidade * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + frente);

        float rotacao = direcao * velocidadeRotacao * Time.fixedDeltaTime;
        Quaternion giro = Quaternion.Euler(0, rotacao, 0);

        rb.MoveRotation(rb.rotation * giro);
    }
}