using UnityEngine;

public class PlayerVehicleInteraction : MonoBehaviour
{
    [Header("Referências")]
    public Camera playerCamera;
    public GameObject playerVisual;

    [Header("Interação")]
    public float distanciaInteracao = 4f;
    public KeyCode teclaInteracao = KeyCode.F;

    [Header("Movimento do Player")]
    public MonoBehaviour playerMovement;

    [Header("Interface")]
    public GameObject interactionF;

    private AudioListener playerAudioListener;

    // Carro que o jogador está usando
    private CarController carroAtual;


    void Start()
    {
        if (playerCamera != null)
        {
            playerAudioListener =
                playerCamera.GetComponent<AudioListener>();
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (carroAtual != null)
            {
                SairDoCarro();
            }
            else
            {
                ProcurarCarro();
            }
        }

        VerificarInteração();
    }
    void VerificarInteração()
    {
        if (playerCamera == null || interactionF == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            distanciaInteracao))
        {
            CarController carro =
                hit.collider.GetComponentInParent<CarController>();

            if (carro != null)
            {
                if (carro.EstaDirigindo)
                {
                    interactionF.SetActive(false);
                    return;
                }
                interactionF.SetActive(true);
                return;
            }
        }

        interactionF.SetActive(false);
    }
    void ProcurarCarro()
    {
        if (playerCamera == null)
        {
            Debug.LogError(
                "Player Camera não foi configurada!"
            );

            return;
        }


        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;


        if (Physics.Raycast(
            ray,
            out hit,
            distanciaInteracao))
        {
            CarController carro =
                hit.collider.GetComponentInParent<CarController>();


            if (carro != null)
            {
                EntrarNoCarro(carro);
            }
        }
    }


    // =========================================================
    // ENTRAR NO CARRO
    // =========================================================

    void EntrarNoCarro(CarController carro)
    {
        carroAtual = carro;


        // Desativa movimento do player
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }


        // Desativa câmera do player
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }


        // Desativa Audio Listener do player
        if (playerAudioListener != null)
        {
            playerAudioListener.enabled = false;
        }


        // Esconde o personagem
        if (playerVisual != null)
        {
            playerVisual.SetActive(false);
        }


        // Ativa carro
        carro.AtivarModoDirigir();


        Debug.Log("ENTROU NO CARRO");
    }


    // =========================================================
    // SAIR DO CARRO
    // =========================================================

    public void SairDoCarro()
    {
        if (carroAtual == null)
        {
            Debug.LogWarning(
                "Tentou sair, mas carroAtual está vazio."
            );

            return;
        }


        // Verifica ExitPoint
        if (carroAtual.exitPoint == null)
        {
            Debug.LogError(
                "O carro não possui um ExitPoint configurado!"
            );

            return;
        }


        // Coloca Player ao lado do carro
        transform.position =
            carroAtual.exitPoint.position;

        transform.rotation =
            carroAtual.exitPoint.rotation;


        // Mostra personagem
        if (playerVisual != null)
        {
            playerVisual.SetActive(true);
        }


        // Liga movimento
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }


        // Liga câmera do player
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
        }


        // Liga Audio Listener do player
        if (playerAudioListener != null)
        {
            playerAudioListener.enabled = true;
        }


        // Desliga carro
        carroAtual.DesativarModoDirigir();


        // Remove referência
        carroAtual = null;


        Debug.Log("SAIU DO CARRO");
    }
}