using UnityEngine;
using TMPro; // se usar TextMeshPro
using System.Collections;

public class CarDropZone : MonoBehaviour
{
    public BoxCounter boxCounter;

    public GameObject dialoguePanel; // painel do diálogo
    public TextMeshProUGUI dialogueText; // texto na tela

    private int boxCount = 0;
    private bool dialogueShown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            boxCounter.AddBox();
            boxCount++;

            Destroy(other.gameObject);

            if (boxCount >= 5 && !dialogueShown)
            {
                dialogueShown = true;
                StartCoroutine(ShowDialogue());
            }
        }
    }

    IEnumerator ShowDialogue()
    {
        dialoguePanel.SetActive(true);

        dialogueText.text = "Beleza, 5 caixas no carro.";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "Já deu por hoje...";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "Tô morto de cansado.";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "Vou pra casa descansar.";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "Amanhã vai ser puxado.";
        yield return new WaitForSeconds(3f);

        dialoguePanel.SetActive(false);
    }
}