using UnityEngine;

public class A_Outro_Audio_Col : MonoBehaviour
{
    private A_Outro_Audio outro_Audio;
    private GameObject player;

    private void Start()
    {
        outro_Audio = GetComponentInParent<A_Outro_Audio>();
        player = FindObjectOfType<Player_Interact>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) outro_Audio.allowTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) outro_Audio.allowTrigger = false;
    }
}
