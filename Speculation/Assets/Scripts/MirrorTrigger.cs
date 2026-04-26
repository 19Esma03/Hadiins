using UnityEngine;

public class MirrorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Managers objesindeki ana scripti bul ve "sýradakine geç" de
            FindObjectOfType<MirrorPathManager>().NextPath();
            gameObject.SetActive(false); // Bu tetikleyiciyi kapat
        }
    }
}