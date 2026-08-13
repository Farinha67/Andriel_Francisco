using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Painel")]
    public GameObject pausePanel;

    private bool jogoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarPause();
        }
    }

    void AlternarPause()
    {
        jogoPausado = !jogoPausado;

        pausePanel.SetActive(jogoPausado);

        if (jogoPausado)
        {
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}