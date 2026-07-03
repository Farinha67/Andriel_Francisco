using UnityEngine;

public class GrandmaInteraction : MonoBehaviour
{
    public GrandmaToCar grandmaMove;

    private bool playerPerto = false;
    private bool falou = false;

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.E) && !falou)
        {
            falou = true;

            Debug.Log("Neto: Vó, tem certeza?");
            Debug.Log("Vó: Claro meu filho.");

            Invoke(nameof(FazerVovoAndar), 3f);
        }
    }

    void FazerVovoAndar()
    {
        grandmaMove.IrAteCarro();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;
        }
    }
}