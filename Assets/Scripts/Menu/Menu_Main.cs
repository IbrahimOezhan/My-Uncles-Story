using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Main : MonoBehaviour
{
    public Settings settings;
    public static Menu_Main menu;
    [SerializeField] private Text score;
    [SerializeField] private Text scoreHard;

    private void Awake()
    {
        menu = this;
    }

    public void Itch()
    {
        Application.OpenURL("https://deadllysin.itch.io/");
        Application.OpenURL("https://ibrahim-oezhan.itch.io/");
    }

    private void Start()
    {
        settings.slider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        SetScreenMode();
        SetQuality();
        SetLevel();
        SetLanguage();
        StartCoroutine(GetNewestVersion(settings.URL));

        if (PlayerPrefs.GetFloat("timeTotal") != 0) score.text = TranslationManager.instance.GetText("time") + ConvertTime(PlayerPrefs.GetFloat("timeTotal"));
        if (PlayerPrefs.GetFloat("timeTotalHard") != 0) scoreHard.text = TranslationManager.instance.GetText("timeHard") + ConvertTime(PlayerPrefs.GetFloat("timeTotalHard"));
    }
    private string ConvertTime(float timeTotal)
    {
        int m = Mathf.FloorToInt(timeTotal / 60);
        float mRest = timeTotal % 60;
        int s = Mathf.FloorToInt(mRest);
        string sS = (s < 10) ? 0 + s.ToString() : s.ToString();
        return m.ToString() + ":" + sS;
    }

    private IEnumerator GetNewestVersion(string url)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        WWW www = new(url);
#pragma warning restore CS0618 // Type or member is obsolete
        yield return www;
        settings.versionText.text = "Version " + Application.version + "" + (Application.version != www.text ? " " + TranslationManager.instance.GetText("new") : "");
    }

    public void SwitchQualityLevel()
    {
        PlayerPrefs.SetInt("qual", (PlayerPrefs.GetInt("qual") == QualitySettings.names.Length - 1) ? 0 : PlayerPrefs.GetInt("qual") + 1);
        SetQuality();
    }

    private void SetQuality()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qual"), true);
        settings.qualityLevelName.GetComponent<UI_Translater>().tagName = "qual" + PlayerPrefs.GetInt("qual");
        TranslationManager.instance.UpdateLanguage();
    }

    public void SwitchLanguage()
    {
        PlayerPrefs.SetInt("lang", (PlayerPrefs.GetInt("lang") == TranslationManager.instance.languages.Length - 1) ? 0 : PlayerPrefs.GetInt("lang") + 1);
        SetLanguage();
    }

    public void SetLanguage()
    {
        settings.languageName.text = TranslationManager.instance.languages[PlayerPrefs.GetInt("lang")].languages.name;
        TranslationManager.instance.UpdateLanguage();
    }

    public void SetLevel() //FMOD Audio Settings
    {
        float value = settings.slider.value;
        AudioManager.instance.masterBus.setVolume(value); //test after
        AudioManager.instance.musicBus.setVolume(value);
        AudioManager.instance.environmentBus.setVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.SetFloat("AmbienceVolume", value);
    }

    public void SwitchScreenMode()
    {
        PlayerPrefs.SetInt("screen", (PlayerPrefs.GetInt("screen") == 2) ? 0 : PlayerPrefs.GetInt("screen") + 1);
        SetScreenMode();
    }

    private void SetScreenMode()
    {
        settings.screenstatetext.GetComponent<UI_Translater>().tagName = "screen" + PlayerPrefs.GetInt("screen");
        switch (PlayerPrefs.GetInt("screen"))
        {
            case 0:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
                break;
            case 1:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed);
                break;
            case 2:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.MaximizedWindow);
                break;
        }
        TranslationManager.instance.UpdateLanguage();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

[System.Serializable]
public class Settings
{
    public Text versionText;
    public Text qualityLevelName;
    public Text languageName;
    public Text screenstatetext;
    public Slider slider;
    public string URL;
}