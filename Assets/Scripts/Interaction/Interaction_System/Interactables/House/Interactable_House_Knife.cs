using UnityEngine;

public class Interactable_House_Knife : MonoBehaviour, IInteractable
{
    public void OnInteract(Interactable i)
    {
        GetComponent<Renderer>().enabled = false;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/playerKnifePU");
        PlayerPrefs.SetInt("hasKnife", 1);
    }
}
