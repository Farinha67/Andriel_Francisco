using UnityEngine;

public class GrandmaToCar : MonoBehaviour
{
    public Transform pontoCarro;

    public float velocidade = 2f;

    private bool podeAndar = false;
    private bool andando = false;
    private bool terminou = false;

    void Update()
    {
        if (!andando || terminou)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            pontoCarro.position,
            velocidade * Time.deltaTime
        );

        // Faz a vó olhar para o ponto
        Vector3 direcao = pontoCarro.position - transform.position;

        if (direcao != Vector3.zero)
        {
            direcao.y = 0;

            transform.rotation = Quaternion.LookRotation(direcao);
        }

        if (Vector3.Distance(transform.position, pontoCarro.position) < 0.15f)
        {
            terminou = true;
            andando = false;

            ChegouNoCarro();
        }
    }

    public void PodeIr()
    {
        podeAndar = true;
    }

    public void IrAteCarro()
    {
        if (!podeAndar)
            return;

        andando = true;
    }

    void ChegouNoCarro()
    {
        // Pequena pausa antes de sumir
        Invoke(nameof(Sumir), 0.5f);
    }

    void Sumir()
    {
        gameObject.SetActive(false);

        if (Scene1Manager.Instance != null)
        {
            Scene1Manager.Instance.AvoChegouNoCarro();
        }
    }
}