using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameIntroManager : MonoBehaviour
{
    [Header("UI Arayüzleri")]
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject runTextUI;

    [Header("Karakter ve Referanslar")]
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour playerController;

    // Artýk tek tek modeller yerine bu ana objeyi kullanacaðýz
    [SerializeField] private GameObject peoples;

    [Header("Ses Kaynaklarý")]
    [SerializeField] private AudioSource crowdAudio;
    [SerializeField] private AudioSource breathAudio;
    [SerializeField] private AudioSource villainAudio;

    [Header("Zamanlama ve Ayarlar")]
    [SerializeField] private float scaleDuration = 10f; // Büyüme ve sesin artma süresi (10 sn ideal)
    [SerializeField] private float targetScaleMultiplier = 3f; // Kaç kat büyüyecek

    private void Start()
    {
        startMenuUI.SetActive(true);
        runTextUI.SetActive(false);

        if (playerController != null)
            playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetupPlayer(GameObject spawnedPlayer)
    {
        player = spawnedPlayer;
        playerController = player.GetComponent<FPSPlayerController>();

        if (playerController != null)
            playerController.enabled = false;

        // Kulak çakýþmasýný önleyen kod (Sarý hatayý engeller)
        if (Camera.main != null && Camera.main.gameObject != player)
        {
            AudioListener oldEar = Camera.main.GetComponent<AudioListener>();
            if (oldEar != null) oldEar.enabled = false;
        }
    }

    public void OnPlayButtonClicked()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        Debug.Log("ADIM 1: Butona basýldý, menü kapanýyor.");
        startMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null) playerController.enabled = false;

        Debug.Log("ADIM 2: Ses baþlatýlýyor.");
        crowdAudio.volume = 0f;
        crowdAudio.loop = true;
        crowdAudio.Play();

        float elapsedTime = 0f;
        Vector3 initialPeoplesScale = Vector3.one;

        if (peoples != null)
        {
            initialPeoplesScale = peoples.transform.localScale;
            Debug.Log("ADIM 3: Peoples objesi bulundu. Ýlk boyut: " + initialPeoplesScale);
        }
        else
        {
            Debug.LogError("KIRMIZI ALARM: Peoples objesi KOD TARAFINDAN BULUNAMADI! (Inspector boþ olabilir)");
        }

        Debug.Log("ADIM 4: 10 Saniyelik Büyüme Döngüsü BAÞLIYOR. Süre: " + scaleDuration);

        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scaleDuration;

            crowdAudio.volume = Mathf.Lerp(0f, 1f, t);

            if (peoples != null)
            {
                peoples.transform.localScale = Vector3.Lerp(initialPeoplesScale, initialPeoplesScale * targetScaleMultiplier, t);
            }

            yield return null;
        }

        Debug.Log("ADIM 5: Döngü süresi BÝTTÝ. Ýnsanlar þimdi siliniyor.");
        crowdAudio.Stop();
        if (peoples != null) peoples.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        Debug.Log("ADIM 6: Nefes sesi çalýyor...");
        breathAudio.Play();
        yield return new WaitForSeconds(1.5f);

        villainAudio.Play();
        yield return new WaitForSeconds(2.5f);

        Debug.Log("ADIM 7: Final. Karakter dönüyor, RUN yazýyor.");
        player.transform.Rotate(0, 90f, 0);
        runTextUI.SetActive(true);

        if (playerController != null)
            playerController.enabled = true;

        yield return new WaitForSeconds(3f);
        runTextUI.SetActive(false);
    }
}