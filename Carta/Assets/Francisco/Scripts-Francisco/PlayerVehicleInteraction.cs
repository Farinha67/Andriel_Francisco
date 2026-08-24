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

    void EntrarNoCarro(CarController carro)
    {
        carroAtual = carro;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }

        if (playerAudioListener != null)
        {
            playerAudioListener.enabled = false;
        }


        if (playerVisual != null)
        {
            playerVisual.SetActive(false);
        }

        carro.AtivarModoDirigir();


        Debug.Log("ENTROU NO CARRO");
    }

    public void SairDoCarro()
    {
        if (carroAtual == null)
        {
            Debug.LogWarning(
                "Tentou sair, mas carroAtual está vazio."
            );

            return;
        }


        if (carroAtual.exitPoint == null)
        {
            Debug.LogError(
                "O carro não possui um ExitPoint configurado!"
            );

            return;
        }

        transform.position =
            carroAtual.exitPoint.position;

        transform.rotation =
            carroAtual.exitPoint.rotation;

        if (playerVisual != null)
        {
            playerVisual.SetActive(true);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = true;
        }

        if (playerAudioListener != null)
        {
            playerAudioListener.enabled = true;
        }

        carroAtual.DesativarModoDirigir();

        carroAtual = null;


        Debug.Log("SAIU DO CARRO");
    }
}