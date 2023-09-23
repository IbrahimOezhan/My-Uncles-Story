using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable_House_Door : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject fadeIn;
    private Ending_Knock ending_Knock;
    private A_Outro_Audio outro_Audio;
    private FirstPersonController firstPersonController;
    private bool once;
    private int count;
    private bool checkCount = false;

    private void Start()
    {
        ending_Knock = FindObjectOfType<Ending_Knock>();
        outro_Audio = FindObjectOfType<A_Outro_Audio>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
    }

    public void OnInteract(Interactable i)
    {
        if (ending_Knock.allowBed || once) return;
        else
        {
            once = true;
            MainManager.instance.whichEnding = (PlayerPrefs.GetInt("hasKnife") == 1) ? 3 : 4;
            firstPersonController.canMove = false;
            StartCoroutine(OpenDoor());
            StartCoroutine(A_CheckInstanceCount());
        }
    }

    private IEnumerator A_CheckInstanceCount()
    {
    reTrigger:
        yield return new WaitUntil(() => checkCount); //or wait while checkCount is true, call checker and don't use goto?
        outro_Audio.outroWindInstance.getDescription(out FMOD.Studio.EventDescription eventDescription);
        eventDescription.getInstanceCount(out int discCount);
        count = discCount;
        if (checkCount) goto reTrigger;
    }

    private IEnumerator OpenDoor()
    {
        checkCount = true;
        AudioManager.instance.FMOD_Outro_Endings_Outside();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/playerDoorToEnding");
        outro_Audio.outroWindInstance.getDescription(out FMOD.Studio.EventDescription outroEventDescription);
        outroEventDescription.getInstanceList(out FMOD.Studio.EventInstance[] outroArray);
        foreach (FMOD.Studio.EventInstance instance in outroArray)
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }

        Instantiate(fadeIn);
        yield return new WaitUntil(() => count == 0); //waits for FMOD ambience instances are fully released (1 second)
        checkCount = false;
        SceneManager.LoadScene(4);
    }
}
