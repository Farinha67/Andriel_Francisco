
using UnityEngine;
using TMPro;

public class BoxCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;

    public int currentBoxes = 0;
    public int maxBoxes = 5;

    void Start()
    {
        AtualizarTexto();
    }

    public void AddBox()
    {
        if (currentBoxes >= maxBoxes)
            return;

        currentBoxes++;
        AtualizarTexto();
    }

    void AtualizarTexto()
    {
        if (counterText != null)
        {
            counterText.text = currentBoxes + "/" + maxBoxes;
        }
    }
}