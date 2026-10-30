
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Painel de Pause")]
    [SerializeField] private GameObject pausePainel;

    [Header("Configurações")]
    [SerializeField] private KeyCode teclaPause = KeyCode.Escape;
    [SerializeField] private bool iniciarPausado = false;

    private bool pausado = false;

    void Start()
    {
        pausado = iniciarPausado;

        if (pausePainel != null)
            pausePainel.SetActive(pausado);

        AtualizarEstado();
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaPause))
        {
            AlternarPause();
        }
    }

    public void AlternarPause()
    {
        pausado = !pausado;

        if (pausePainel != null)
            pausePainel.SetActive(pausado);

        AtualizarEstado();
    }

    public void Pausar()
    {
        pausado = true;

        if (pausePainel != null)
            pausePainel.SetActive(true);

        AtualizarEstado();
    }

    public void Continuar()
    {
        pausado = false;

        if (pausePainel != null)
            pausePainel.SetActive(false);

        AtualizarEstado();
    }

    private void AtualizarEstado()
    {
        Time.timeScale = pausado ? 0f : 1f;

        Cursor.lockState = pausado
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = pausado;
    }

    public void SairDoJogo()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void VoltarAoMenuPrincipal()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        pausePainel.SetActive(false);
    }
}
