using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    [SerializeField] private Image targetImage;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;

    public float scaleMultiplier = 1.1f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetImage.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetImage.color = hoverColor;
        transform.localScale = originalScale * scaleMultiplier;

        if (audioSource && hoverSound && !audioSource.isPlaying)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetImage.color = normalColor;
        transform.localScale = originalScale;
    }
}