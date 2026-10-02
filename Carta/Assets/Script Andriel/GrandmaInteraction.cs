using UnityEngine;
using System.Collections;

public class GrandmaInteraction : MonoBehaviour
{
    [Header("VÓ")]
    public GrandmaToCar grandma;

    [Header("PLAYER")]
    public Transform player;

    [Header("DISTÂNCIA PARA CONVERSAR")]
    public float distanciaInteracao = 3f;

    private bool conversou = false;
    private bool conversando = false;
    private bool jogadorPerto = false;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError(
                    "GrandmaInteraction: Não encontrei o Player. Verifique se ele tem a Tag 'Player'."
                );
            }
        }
    }

    void Update()
    {
        if (player == null)
            return;

        if (conversou || conversando)
            return;

        if (Scene1Manager.Instance == null)
            return;

        if (!Scene1Manager.Instance.PodeFalarComAvo())
            return;

        float distancia = Vector3.Distance(
            player.position,
            transform.position
        );

        jogadorPerto = distancia <= distanciaInteracao;

        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            conversando = true;

            StartCoroutine(Conversar());
        }
    }

    // =========================================================
    // MOSTRA [E] NA TELA
    // =========================================================

    void OnGUI()
    {
        if (!jogadorPerto)
            return;

        if (conversou || conversando)
            return;

        if (Scene1Manager.Instance == null)
            return;

        if (!Scene1Manager.Instance.PodeFalarComAvo())
            return;

        GUIStyle estilo = new GUIStyle(GUI.skin.label);

        estilo.fontSize = 28;
        estilo.alignment = TextAnchor.MiddleCenter;
        estilo.normal.textColor = Color.white;

        GUI.Label(
            new Rect(
                Screen.width / 2 - 250,
                Screen.height - 140,
                500,
                60
            ),
            "[E] Falar com a vó",
            estilo
        );
    }

    // =========================================================
    // CONVERSA
    // =========================================================

    IEnumerator Conversar()
    {
        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Neto: Vó, o carro já está cheio."
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Vó: Então vamos viajar!"
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Neto: Mas... onde você vai?"
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Vó: Vou em cima do carro."
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Neto: Em cima do carro?!"
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Vó: Sim. Vou enrolada naquele tapete."
            )
        );

        yield return StartCoroutine(
            Scene1Manager.Instance.Falar(
                "Neto: Você só pode estar brincando..."
            )
        );

        conversou = true;

        if (grandma != null)
        {
            grandma.IrAteCarro();
        }
        else
        {
            Debug.LogError(
                "GrandmaInteraction: O campo Grandma está vazio!"
            );
        }
    }
}