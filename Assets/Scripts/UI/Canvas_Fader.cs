using UnityEngine;

public class Canvas_Fader : MonoBehaviour
{
    private CanvasGroup group;
    public float speed;

    private void Start()
    {
        Destroy(this, 5);
        if (speed < 0) Destroy(gameObject, 25);
        group = GetComponent<CanvasGroup>();
    }

    private void FixedUpdate()
    {
        group.alpha += speed;
    }
}
