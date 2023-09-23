using UnityEngine;

public class Interactable_House_Log : MonoBehaviour, IInteractable
{
    private GameManager gameManager;
    private LogManager logManager;
    private int which = 0;

    private void Start()
    {
        logManager = FindObjectOfType<LogManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnInteract(Interactable i)
    {
        if (gameManager.trip == gameManager.getNightAtTrip)
        {
            logManager.DropLog();
            which += 1;
        }
        else if (which == 1)
        {
            which = 2;
            gameManager.ChangeObjective("objectiveSleep");
            gameManager.carDoor.gameObject.SetActive(true);
            gameManager.house.gameObject.SetActive(false);
        }
        else
        {
            logManager.DropLog();
            gameManager.ChangeObjective((gameManager.isNight) ? "objectiveRemain" : "objectiveCar");
            if (gameManager.trip > gameManager.getNightAtTrip + 1 && TranslationManager.instance.GetText("message" + (MainManager.instance.hardMode ? "Hard" : "") + gameManager.messageState) != "") gameManager.noteOnCar.SetActive(true);
            gameManager.carLog.gameObject.SetActive(true);
            gameManager.house.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.nightManagerRef != null && gameManager.manWalkAway != null && gameManager.manWalkAway.gameObject.activeInHierarchy && gameManager.manWalkAway.DistanceToTarget() < 5) WalkAwayMan();
    }

    private void OnDisable()
    {
        if (gameManager.nightManagerRef != null && gameManager.manWalkAway != null && gameManager.manWalkAway.gameObject.activeInHierarchy) WalkAwayMan();
    }

    private void WalkAwayMan()
    {
        if (gameManager.manWalkAway.CanSwitchState()) gameManager.manWalkAway.Start_Walk_Away();
    }
}
