using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Main : MonoBehaviour
{
    public Settings settings;
    public static Menu_Main menu;
    [SerializeField] private Text score;

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
        StartCoroutine(GetVersion());

        if (PlayerPrefs.GetFloat("timeTotalHard") != 0) score.text += TranslationManager.instance.GetText("timeHard") + ConvertTime(PlayerPrefs.GetFloat("timeTotalHard")) + "\n";
        if (PlayerPrefs.GetFloat("timeTotal") != 0) score.text += TranslationManager.instance.GetText("time") + ConvertTime(PlayerPrefs.GetFloat("timeTotal"));
    }
    private string ConvertTime(float timeTotal)
    {
        int m = Mathf.FloorToInt(timeTotal / 60);
        float mRest = timeTotal % 60;
        int s = Mathf.FloorToInt(mRest);
        string sS = (s < 10) ? 0 + s.ToString() : s.ToString();
        return m.ToString() + ":" + sS;
    }

    private IEnumerator GetVersion()
    {
        settings.versionText.text = "Version " + Application.version;
        using WWW _requestGet = new(settings.URL);
        yield return _requestGet;

        string _dots = ".";
        float _timePassed = 0;
        while (!_requestGet.isDone)
        {
            _timePassed += Time.deltaTime;
            yield return new WaitForSeconds(.4f);
            _dots = _dots.Length == 3 ? "." : _dots + ".";

            if (_timePassed > 5)
            {
                _requestGet.Dispose();
            }
        }

        string _latestVersion = _requestGet.text;
        (bool _isOutdated, bool _error) = IsOutdated(Application.version.Split('.'), _latestVersion.Split('.'));

        _requestGet.Dispose();
        if (_isOutdated) OnOutdated();
    }

    public (bool, bool) IsOutdated(string[] _current, string[] _latest)
    {
        for (int i = 0; i < _latest.Length; i++)
        {
            if (int.TryParse(_latest[i], out var _latestVersion))
            {
                int _currentVersion = int.Parse(_current[i]);
                if (_latestVersion < _currentVersion) break;
                if (_latestVersion > _currentVersion) return (true, false);
            }
            else return (false, true);
        }
        return (false, false);
    }

    private void OnOutdated()
    {
        settings.versionText.text = "Version " + Application.version + "" + TranslationManager.instance.GetText("new");
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