using UnityEngine;

public class MirrorPathManager : MonoBehaviour
{
    [Header("Yollar Listesi")]
    public GameObject[] allPaths; // 5 yolu buraya sýrayla sürükle

    private int currentPathIndex = 0;

    void Start()
    {
        // Baþlangýçta sadece 1. yolu aç, diðerlerini kapat
        ActivatePath(0);
    }

    public void NextPath()
    {
        currentPathIndex++;
        if (currentPathIndex < allPaths.Length)
        {
            ActivatePath(currentPathIndex);
        }
        else
        {
            // Tüm aynalar bittiðinde yapýlacaklar
            Debug.Log("Tüm aynalar tamamlandý!");
        }
    }

    private void ActivatePath(int index)
    {
        for (int i = 0; i < allPaths.Length; i++)
        {
            allPaths[i].SetActive(i == index);
        }
    }
}