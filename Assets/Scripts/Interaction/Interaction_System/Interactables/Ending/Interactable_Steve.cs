using UnityEngine;

public class Interactable_Steve : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;

    public void OnInteract(Interactable i)
    {
        animator.Play((PlayerPrefs.GetInt("hasKnife") == 0) ? "Player" : "Steve");
    }
}
