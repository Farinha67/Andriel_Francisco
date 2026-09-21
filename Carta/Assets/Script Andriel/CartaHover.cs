using UnityEngine;
using UnityEngine.EventSystems;

public class CartaHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float multiplicadorHover = 1.15f;
    public float velocidade = 8f;

    private Vector3 escalaOriginal;
    private Vector3 escalaAlvo;

    void Start()
    {
        escalaOriginal = transform.localScale;
        escalaAlvo = escalaOriginal;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaAlvo,
            velocidade * Time.deltaTime
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal * multiplicadorHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal;
    }
}