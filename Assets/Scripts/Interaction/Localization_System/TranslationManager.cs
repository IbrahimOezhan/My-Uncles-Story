using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TranslationManager : MonoBehaviour
{
    public static TranslationManager instance;
    public static event Action languageUpdate;
    public static event Action scaleUpdate;
    public Language[] languages;
    public bool debugWarnings;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < languages.Length; i++)
            {
                string fileContent = Encoding.UTF8.GetString(languages[i].languages.bytes);
                string[] linesInFile = fileContent.Split('\n');
                foreach (string line in linesInFile)
                {
                    //Debug.Log(line);
                    if (line[0] == '#' || line == "") continue;
                    languages[i].lines.Add(line);
                }
                if (languages[i].sysLanguage == Application.systemLanguage && PlayerPrefs.GetInt("first") == 0)
                {
                    PlayerPrefs.SetInt("lang", i);
                    PlayerPrefs.SetInt("first", 1);
                }
            }

            UpdateLanguage();
        }
    }

    private void Start()
    {
        UpdateLanguage();
    }

    private void OnDestroy()
    {
        languageUpdate = null;
        scaleUpdate = null;
    }

    public void SetUIText(Text text, string tag)
    {
        text.text = "";
        string[] translatedText = GetTextArray(tag);
        for (int i = 0; i < translatedText.Length; i++) text.text += translatedText[i] + ((translatedText.Length > 1) ? "\n" : "");
    }

    public void UpdateLanguage()
    {
        languageUpdate?.Invoke();
        scaleUpdate?.Invoke();
    }

    public string[] GetTextArray(string tag)
    {
        return GetText(tag).Split("/");
    }

    public string GetText(string tag)
    {
        int language = PlayerPrefs.GetInt("lang");
        for (int i = 0; i < languages[language].lines.Count; i++)
        {
            string[] translation = languages[language].lines[i].Split('=');
            translation[0].Replace(" ", "");
            if (translation[0] == tag) return translation[1];
        }
        if (debugWarnings) Debug.LogWarning("Text with tag: " + tag + " not found");
        return "";
    }
}

[System.Serializable]
public class Language
{
    public SystemLanguage sysLanguage;
    [HideInInspector] public List<string> lines = new();
    public TextAsset languages;
}
