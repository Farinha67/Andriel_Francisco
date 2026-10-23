
using UnityEngine;
using System.Collections;

public class GrandmaInteraction : MonoBehaviour
{
    public Transform player;
    public float distanciaInteracao = 3f;

    private bool jogadorPerto;
    private bool conversando;
    private bool conversaTerminada;

    void Start()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
                player = obj.transform;
        }
    }

    void Update()
    {
        if (player == null || Scene1Manager.Instance == null)
            return;

        jogadorPerto =
            Vector3.Distance(player.position, transform.position)
            <= distanciaInteracao;

        if (jogadorPerto &&
            Input.GetKeyDown(KeyCode.E) &&
            Scene1Manager.Instance.PodeFalarComAvo() &&
            !conversando &&
            !conversaTerminada)
        {
            StartCoroutine(IniciarConversa());
        }
    }

    IEnumerator IniciarConversa()
    {
        conversando = true;

        yield return StartCoroutine(
            Scene1Manager.Instance.ConversaComAvo()
        );

        conversaTerminada = true;
        conversando = false;
    }

    void OnGUI()
    {
        if (!jogadorPerto || conversando || conversaTerminada)
            return;

        if (Scene1Manager.Instance == null ||
            !Scene1Manager.Instance.PodeFalarComAvo())
            return;

        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 24;
        estilo.alignment = TextAnchor.MiddleCenter;
        estilo.normal.textColor = Color.white;

        GUI.Label(
            new Rect(
                Screen.width / 2 - 200,
                Screen.height - 120,
                400,
                50
            ),
            "Aperte E para falar com a vó",
            estilo
        );
    }
}
