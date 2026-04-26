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
        Instantiate(PlayerPref, spawnPoint.position, spawnPoint.rotation);
        PlayerPref.SetActive(true);
    }
}
