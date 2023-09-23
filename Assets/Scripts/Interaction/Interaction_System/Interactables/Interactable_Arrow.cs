using UnityEngine;

public class Interactable_Arrow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float fadeoutDistance, fadeinDistance;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < fadeoutDistance) spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, distance - 2);
        else if (distance > fadeinDistance) spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 13 - distance);
        else spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 255);
    }
}
