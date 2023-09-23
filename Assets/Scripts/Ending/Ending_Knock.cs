using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ending_Knock : MonoBehaviour
{

    private A_Outro_Audio outro_Audio;
    [HideInInspector] public bool allowBed = true;
    [SerializeField] private Interactable dialog;
    [SerializeField] private GameObject duck;
    [SerializeField] private Text objective;
    [SerializeField] private Transform player, bed;
    [SerializeField] private Interactable bedInteractable, doorInteractable, door2Interactable;
    [SerializeField] private int loopAmount, loopWaitTime;
    [SerializeField] private GameObject doorScareTrigger;

    private void Awake()
    {
        allowBed = true;
        outro_Audio = GetComponentInChildren<A_Outro_Audio>();
    }

    private void Start()
    {
        allowBed = true;
        if (PlayerPrefs.GetInt("hasDuck") == 1) duck.SetActive(true);
        objective.text = "";
        dialog.Interact();
    }

    public void StartKnocking()
    {
        StartCoroutine(Knocking());
    }

    private IEnumerator Knocking()
    {
        yield return new WaitUntil(() => Vector3.Distance(player.position, bed.position) < 3);
        allowBed = false;
        TranslationManager.instance.SetUIText(objective, "objectiveDoor");
        bedInteractable.ClearTriggerList(Trigger._dialogue);
        door2Interactable.dialog.tag = "not";
        doorInteractable.dialog.tag = "who";
        StartCoroutine(outro_Audio.Queue_Jumpscare());
        for (int i = 0; i < loopAmount; i++)
        {
            outro_Audio.doorKnockingInstance.start();
            objective.text += "!";
            yield return new WaitForSeconds(loopWaitTime);

        }
        yield return new WaitWhile(() => player.GetComponent<Player_Interact>().isInteracting);
        Destroy(doorScareTrigger);
        outro_Audio.allowTrigger = false;
        StopCoroutine(outro_Audio.Queue_Jumpscare());
        TranslationManager.instance.SetUIText(objective, "objectiveBed");
        bedInteractable.ClearTriggerList(Trigger._event);
        doorInteractable.dialog.tag = "door";
        door2Interactable.dialog.tag = "door";
        allowBed = true;
        outro_Audio.doorKnockingInstance.release();
    }
}
