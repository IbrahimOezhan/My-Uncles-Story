using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ending_Credits : MonoBehaviour
{
    [SerializeField] private Text endingText;
    [SerializeField] private Text hardText;
    private bool playedScare = false;

    private IEnumerator Start()
    {
        PlayerPrefs.SetInt("beatGame", 1);

        if (MainManager.instance.hardMode) hardText.enabled = true;
        endingText.text = "Ending " + MainManager.instance.whichEnding.ToString() + "/4";

        yield return new WaitForSecondsRealtime(13);

        if (AudioManager.instance.sceneNbr == 6 && MainManager.instance.whichEnding == 2)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/manJumpscare_Bed");
            playedScare = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AudioManager.instance.sceneNbr == 6 && !playedScare && MainManager.instance.whichEnding == 2) FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/manJumpscare_Bed");
            else FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Click_Off");
            SceneManager.LoadScene(0);
        }
    }
}
