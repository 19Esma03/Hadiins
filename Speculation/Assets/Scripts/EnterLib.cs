using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterLabyrinth : MonoBehaviour
{
    public string sceneName = "LabyrinthScene";
    public GameObject interactUI;
    public float distance = 3f;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene(sceneName);
                }
                return;
            }
        }

        interactUI.SetActive(false);
    }
}