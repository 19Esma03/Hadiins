using UnityEngine;

public class MirrorChoice : MonoBehaviour
{
    public bool isCorrectMirror;
    public MeshRenderer mirrorRenderer;

    public void SetupMirror(bool isCorrect, Material mat)
    {
        isCorrectMirror = isCorrect;

        if (mirrorRenderer != null)
            mirrorRenderer.material = mat;
    }

    public void InteractWithMirror()
    {
        FindAnyObjectByType<MirrorLabyrinthManager>()
            .OnMirrorChosen(isCorrectMirror, gameObject);
    }
}