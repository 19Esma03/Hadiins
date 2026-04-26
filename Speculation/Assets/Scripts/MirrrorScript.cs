using UnityEngine;

public class MirrorScript : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform mirrorPlane; // Sahnedeki Quad
    public Camera playerCam;      // Main Camera

    private Vector3 initialPosition;

    void Start()
    {
        if (playerCam == null) playerCam = Camera.main;

        // Kameranýn editörde koyduðun ilk yerini kaydet
        initialPosition = transform.position;
    }

    void LateUpdate()
    {
        if (playerCam == null || mirrorPlane == null) return;

        // 1. KONUMU SABÝTLE (Kamera yerinden oynamaz)
        transform.position = initialPosition;

        // 2. Y EKSENÝNDE DÖNÜÞ (Sadece Saða-Sola)
        // Oyuncunun pozisyonuna doðru bir yön vektörü oluþtur
        Vector3 directionToPlayer = playerCam.transform.position - transform.position;

        // Yüksekliði (yukarý-aþaðý bakýþý) iptal et, sadece yatay düzleme odaklan
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            // Bu yöne bakacak rotasyonu hesapla
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Sadece Y eksenindeki açýyý al, X ve Z'yi 0'la
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        }

    }
}