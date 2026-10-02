using UnityEngine;
using System.Collections;

public class GrandmaInteraction : MonoBehaviour
{
    public GrandmaToCar grandma;

    private bool playerPerto = false;
    private bool conversou = false;

    void Update()
    {
        if (!playerPerto)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !conversou)
        {
            conversou = true;

            StartCoroutine(Conversar());
        }
    }

    IEnumerator Conversar()
    {
        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Neto: Vó, você realmente vai em cima do carro?"
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Vó: Vou sim, meu filho."
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Vó: Só me enrola naquele tapete."
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Neto: Tá bom então..."
            )
        );

        grandma.IrAteCarro();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;
        }
    }
}