
using UnityEngine;

public class CarDropZone : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public PickupItem pickupItem;
    public BoxCounter boxCounter;

    [Header("Configurações")]
    public float distanciaInteracao = 4f;
    public int maxCaixas = 5;

    private int caixasColocadas = 0;

    void Start()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
                player = obj.transform;
        }

        if (pickupItem == null && player != null)
            pickupItem = player.GetComponent<PickupItem>();
    }

    public bool JogadorPerto()
    {
        if (player == null)
            return false;

        return Vector3.Distance(player.position, transform.position)
            <= distanciaInteracao;
    }

    public void ColocarCaixa(PickupItem jogador)
    {
        if (caixasColocadas >= maxCaixas)
            return;

        if (jogador == null || !jogador.TemCaixa())
            return;

        jogador.DepositarCaixa();

        caixasColocadas++;

        if (boxCounter != null)
            boxCounter.AddBox();

        if (Scene1Manager.Instance != null)
            Scene1Manager.Instance.CaixaColocada();

        Debug.Log("Caixas colocadas: " + caixasColocadas + "/" + maxCaixas);
    }

    void OnGUI()
    {
        if (!JogadorPerto())
            return;

        if (pickupItem == null || !pickupItem.TemCaixa())
            return;

        if (caixasColocadas >= maxCaixas)
            return;

        GUIStyle estilo = new GUIStyle(GUI.skin.label);

        estilo.fontSize = 25;
        estilo.alignment = TextAnchor.MiddleCenter;
        estilo.normal.textColor = Color.white;
        estilo.fontStyle = FontStyle.Bold;

        GUI.Label(
            new Rect(
                Screen.width / 2 - 300,
                Screen.height - 150,
                600,
                50
            ),
            "APERTE E PARA COLOCAR A CAIXA NO CARRO",
            estilo
        );
    }
}
