using UnityEngine;
using UnityEngine.UI;

public class RestartManager : MonoBehaviour
{
    [SerializeField] private GameObject blackscreen;
    [SerializeField] private Text skipText;

    private void Update()
    {
        if (PlayerPrefs.GetInt("hasDied") == 1) skipText.enabled = true;
        if (Input.GetKeyDown(KeyCode.E)) SkipCutscene();
    }

    private void SkipCutscene()
    {
        blackscreen.SetActive(true);
        Invoke(nameof(Skip), 1);
    }

    private void Skip()
    {
        MainManager.instance.Restart();
    }

    public void FMOD_Play_Drag()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/manFoley/manDragRestart");
    }

    public void FMOD_Play_ManSteps()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/manMovement/manStepsRestart");
    }

    public void FMOD_Play_Restart_Drone()
    {
        //Stopped in FMOD CMD when GameState changes
        FMODUnity.RuntimeManager.PlayOneShot("event:/Music/manRestartDrone");
    }
}
