using UnityEngine;

public class CartaGirando : MonoBehaviour
{
    public float velocidade = 100f;

    void Update()
    {
        transform.Rotate(0f, velocidade * Time.deltaTime, 0f);
    }
}