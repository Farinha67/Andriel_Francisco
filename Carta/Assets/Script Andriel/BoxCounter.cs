using TMPro;
using UnityEngine;

public class BoxCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;

    public int currentBoxes = 0;
    public int maxBoxes = 5;

    void Start()
    {
        UpdateText();
    }

    public void AddBox()
    {
        currentBoxes++;

        if (currentBoxes > maxBoxes)
            currentBoxes = maxBoxes;

        UpdateText();

        if (currentBoxes == maxBoxes)
        {
            Debug.Log("MISSÃO COMPLETA!");
        }
    }

    void UpdateText()
    {
        counterText.text = currentBoxes + "/" + maxBoxes;
    }
}