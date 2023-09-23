using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuGroup;
    [SerializeField] private CanvasGroup blackscreen;
    [SerializeField] private GameObject noIntroButton;
    [SerializeField] private Button hardButton;

    private void Start()
    {
        if (PlayerPrefs.GetInt("beatGame") == 1) hardButton.gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("reachedNight") == 1) noIntroButton.SetActive(true);
    }

    private void Update()
    {
        if (menuGroup.interactable)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Play()
    {
        StartCoroutine(IntroSequence(0));
    }

    public void PlayHard()
    {
        MainManager.instance.hardMode = true;
        PlayNoIntro();
    }

    public void PlayNoIntro()
    {
        AudioManager.instance.menuMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        MainManager.instance.skipDay = true;
        StartCoroutine(IntroSequence(1));
    }

    private IEnumerator IntroSequence(int skipIntro)
    {
        menuGroup.interactable = false;
        Destroy(menuGroup.GetComponent<A_Menu_Sounds>());
        menuGroup.gameObject.AddComponent<Canvas_Fader>().speed = -0.02f;
        yield return new WaitForSeconds(1);
        blackscreen.gameObject.AddComponent<Canvas_Fader>().speed = 0.02f;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SkipIntro", skipIntro);

        if (skipIntro == 0)
        {
            AudioManager.instance.menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            AudioManager.instance.menuMusicInstance.release();
            yield return new WaitForSeconds(1);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "In_Game_Day");
            yield return new WaitForSeconds(12);
        }
        else yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(1);
    }
}
