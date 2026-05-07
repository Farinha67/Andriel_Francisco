using UnityEngine;
using TMPro;

public class VolumeController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI volumeText;

    [Header("Config")]
    [Range(0, 10)]
    public int volumeLevel = 5;

    void Start()
    {
        UpdateVolume();
    }

    public void IncreaseVolume()
    {
        if (volumeLevel < 10)
        {
            volumeLevel++;
            UpdateVolume();
        }
    }

    public void DecreaseVolume()
    {
        if (volumeLevel > 0)
        {
            volumeLevel--;
            UpdateVolume();
        }
    }

    void UpdateVolume()
    {
        volumeText.text = volumeLevel.ToString();

        // Converte de 0-10 para 0-1
        AudioListener.volume = volumeLevel / 10f;
    }
}