using UnityEngine;

public class Interactable_Car_Door : MonoBehaviour, IInteractable
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnInteract(Interactable i)
    {
        gameManager.nightManagerRef = Instantiate(gameManager.nightManager).GetComponent<NightManager>();
    }
}
