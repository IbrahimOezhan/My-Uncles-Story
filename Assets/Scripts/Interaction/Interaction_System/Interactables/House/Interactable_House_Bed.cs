using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable_House_Bed : MonoBehaviour, IInteractable
{
    private Ending_Knock ending_Knock;

    private void Start()
    {
        ending_Knock = FindObjectOfType<Ending_Knock>();
    }

    public void OnInteract(Interactable i)
    {
        if (ending_Knock.allowBed)
        {
            MainManager.instance.whichEnding = (PlayerPrefs.GetInt("hasDuck") == 1) ? 1 : 2;
            SceneManager.LoadScene(5);
        }
    }
}
