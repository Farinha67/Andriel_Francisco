using UnityEngine;

public class GrandmaToCar : MonoBehaviour
{
    public Transform carro;
    public float velocidade = 2f;

    private bool andando = false;

    void Update()
    {
        if (!andando) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            carro.position,
            velocidade * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, carro.position) < 1f)
        {
            gameObject.SetActive(false); // some ao chegar no carro
        }
    }

    public void IrAteCarro()
    {
        andando = true;
    }
}