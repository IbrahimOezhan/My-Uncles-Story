using UnityEngine;

public class Interactable_Duck : MonoBehaviour, IInteractable
{
    public void OnInteract(Interactable i)
    {
        PlayerPrefs.SetInt("hasDuck", 1);
        gameObject.SetActive(false);
    }
}
