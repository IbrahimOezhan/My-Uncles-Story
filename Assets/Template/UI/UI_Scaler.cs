using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Scaler : MonoBehaviour
{
    private Text text;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<Text>();
        TranslationManager.scaleUpdate += AdjustSizeDelta;
    }

    private void OnEnable()
    {
        AdjustSizeDelta();
        StartCoroutine(AdjustSizeDeltaIE());
    }

    private IEnumerator AdjustSizeDeltaIE()
    {
        yield return new WaitForSeconds(0.1f);
        AdjustSizeDelta();
        yield return new WaitForSeconds(0.4f);
        AdjustSizeDelta();
    }

    private void OnDestroy()
    {
        TranslationManager.scaleUpdate -= AdjustSizeDelta;
    }

    public void AdjustSizeDelta()
    {
        int horitontalCharLenght = -1;
        string[] allLines = text.text.Split("\n");
        for (int i = 0; i < allLines.Length; i++)
            if (allLines[i].Length > horitontalCharLenght) horitontalCharLenght = allLines[i].Length;
        rectTransform.sizeDelta = new(horitontalCharLenght * text.fontSize + 1, allLines.Length * text.fontSize + 1);
    }
}
