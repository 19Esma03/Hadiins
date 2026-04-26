using UnityEngine;

public class MirrorLabyrinthManager : MonoBehaviour
{
    [Header("Oyuncu")]
    public Transform startPoint;
    private GameObject player;

    [Header("Ayna Sistemi")]
    public GameObject mirrorPrefab;
    public int mirrorCount = 4;
    public float spacing = 4f;

    public Material adultMaterial;
    public Material wrongMaterial;

    [Header("Oyun")]
    public int targetPassCount = 5;

    private int passCount = 0;
    private GameObject[] currentMirrors;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SpawnMirrors();
    }

    public void OnMirrorChosen(bool isCorrect, GameObject mirror)
    {
        if (isCorrect)
        {
            passCount++;
            Destroy(mirror);

            if (passCount >= targetPassCount)
            {
                Debug.Log("KAZANDIN!");
                return;
            }

            // Yeni set oluştur
            Invoke(nameof(SpawnMirrors), 0.2f);
        }
        else
        {
            passCount = 0;
            ResetAll();
        }
    }

    void SpawnMirrors()
    {
        ClearMirrors();

        currentMirrors = new GameObject[mirrorCount];

        int correctIndex = Random.Range(0, mirrorCount);

        for (int i = 0; i < mirrorCount; i++)
        {
            Vector3 pos = startPoint.position + startPoint.forward * 10 + new Vector3((i - 1.5f) * spacing, 0, 0);

            GameObject m = Instantiate(mirrorPrefab, pos, Quaternion.identity);

            MirrorChoice mc = m.GetComponent<MirrorChoice>();

            bool isCorrect = (i == correctIndex);

            mc.SetupMirror(isCorrect, isCorrect ? adultMaterial : wrongMaterial);

            currentMirrors[i] = m;
        }
    }

    void ClearMirrors()
    {
        if (currentMirrors == null) return;

        foreach (var m in currentMirrors)
        {
            if (m != null) Destroy(m);
        }
    }

    void ResetAll()
    {
        TeleportPlayer();
        SpawnMirrors();
    }

    void TeleportPlayer()
    {
        if (player == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = startPoint.position;
        player.transform.rotation = startPoint.rotation;

        if (cc != null) cc.enabled = true;
    }
}