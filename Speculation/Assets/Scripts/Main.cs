using UnityEngine;

public class Main : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject PlayerPref;

    private void Awake()
    {
        SpawnIt();
    }

    private void SpawnIt()
    {
        // 1. Oyuncuyu oluþtur ve deðiþkene ata
        GameObject spawnedInstance = Instantiate(PlayerPref, spawnPoint.position, spawnPoint.rotation);

        // 2. Sahnede GameIntroManager'ý bul ve oyuncuyu ona teslim et
        GameIntroManager introManager = FindObjectOfType<GameIntroManager>();
        PlayerPref.SetActive(true);
        if (introManager != null)
        {
            introManager.SetupPlayer(spawnedInstance);
        }
        else
        {
            Debug.LogError("Sahnede GameIntroManager bulunamadý!");
        }
    }
}