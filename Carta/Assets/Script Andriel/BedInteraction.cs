
using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [Header("Jogador")]
    public Transform player;
    public float distanciaInteracao = 2.5f;

    private bool jogadorPerto;

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

        jogadorPerto = Vector3.Distance(
            player.position,
            transform.position
        ) <= distanciaInteracao;

        if (jogadorPerto &&
            Input.GetKeyDown(KeyCode.E) &&
            Scene1Manager.Instance.PodeDescansar())
        {
            Scene1Manager.Instance.IniciarSono();
        }
    }

    void OnGUI()
    {
        if (!jogadorPerto ||
            Scene1Manager.Instance == null ||
            !Scene1Manager.Instance.PodeDescansar())
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
            "Aperte E para dormir",
            estilo
        );
    }
}
