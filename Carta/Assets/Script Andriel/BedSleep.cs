using UnityEngine;

public class BedSleep : MonoBehaviour
{
    private bool playerPerto = false;

    private void Update()
    {
        if (!playerPerto)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Scene1Manager.Instance != null &&
                Scene1Manager.Instance.PodeDormir())
            {
                Scene1Manager.Instance.Dormir();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;
        }
    }
}