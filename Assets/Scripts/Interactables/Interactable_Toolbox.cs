using UnityEngine;

public class Interactable_Toolbox : MonoBehaviour, IInteractable
{
    Player_Compass player_Compass;

    private void Start()
    {
        player_Compass = FindObjectOfType<Player_Compass>();
    }

    public void OnInteract(Interactable i)
    {
        StartCoroutine(player_Compass.RepairAnim(i));
    }
}
