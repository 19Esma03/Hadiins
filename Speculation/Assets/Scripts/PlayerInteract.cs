using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 5f;
    public GameObject interactUI;

    void Update()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            MirrorChoice mc = hit.collider.GetComponentInParent<MirrorChoice>();

            if (mc != null)
            {
                if (interactUI != null && !interactUI.activeSelf)
                    interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    mc.InteractWithMirror();
                }
                return;
            }
        }

        if (interactUI != null && interactUI.activeSelf)
            interactUI.SetActive(false);
    }
}