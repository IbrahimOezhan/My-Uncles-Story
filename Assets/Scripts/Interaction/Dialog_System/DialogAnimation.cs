using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogAnimation : MonoBehaviour
{
    [HideInInspector] public float delay = 0.08f;
    [SerializeField] private Text textToAnimate;
    [HideInInspector] public Interactable i;
    private string fullText = "";

    private void OnEnable()
    {
        StartCoroutine(ShowText());
    }

    public IEnumerator ShowText()
    {
        fullText = textToAnimate.text;
        for (int i = 0; i <= fullText.Length; i++)
        {
            textToAnimate.text = fullText.Substring(0, i);
            if (i != fullText.Length && (fullText[i] != '.' || fullText[i] != ' '))
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Dialogue_Letter");
            if (i != fullText.Length) yield return new WaitForSeconds(delay + ((fullText[i] == '.') ? delay : 0));
        }
        if (!i.dialog.skipable) DialogManager.instance.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        else DialogManager.instance.textDismiss.enabled = true;
    }
}