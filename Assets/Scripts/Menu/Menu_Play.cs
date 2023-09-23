using UnityEngine;
using UnityEngine.Events;

public class Menu_Play : MonoBehaviour
{
    public UnityEvent OnSingleOption;
    public UnityEvent OnMultiOption;

    public GameObject skip;

    public void Play()
    {
        if(PlayerPrefs.GetInt("reachedNight") == 1) OnMultiOption.Invoke();
        else OnSingleOption.Invoke();
    }
}
