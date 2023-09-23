using UnityEngine;

public class Interactable_Car_Log : MonoBehaviour, IInteractable
{
    private GameManager gameManager;
    private LogManager logManager;

    private void Start()
    {
        logManager = FindObjectOfType<LogManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnInteract(Interactable i)
    {
        if (gameManager.noteOnCar.activeInHierarchy || gameManager.trip == 0) gameManager.DisplayNote(i);
        else logManager.PickUpLog(i);
    }
}