using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Translater : MonoBehaviour
{
    private Text textToTranslate;
    public string tagName;

    private void Awake()
    {
        textToTranslate = GetComponent<Text>();
    }

    private void OnEnable()
    {
        TranslationManager.languageUpdate += Translate;
        StartCoroutine(TranslateIE());
    }

    private void OnDisable()
    {
        TranslationManager.languageUpdate -= Translate;
    }

    private void OnDestroy()
    {
        TranslationManager.languageUpdate -= Translate;
    }

    private IEnumerator TranslateIE()
    {
        yield return new WaitForSeconds(.005f);
        Translate();
    }

    private void Translate()
    {
        TranslationManager.instance.SetUIText(textToTranslate, tagName);
    }
}