using UnityEngine;

public class NaxtStages : MonoBehaviour
{
    [SerializeField] private string cenaParaCarregar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(cenaParaCarregar);
        }
    }
}

