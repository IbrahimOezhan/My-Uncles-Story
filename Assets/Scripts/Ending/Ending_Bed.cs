using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending_Bed : MonoBehaviour
{
    private void Choose()
    {
        GetComponent<Animator>().Play((PlayerPrefs.GetInt("hasDuck") == 1) ? "Ending_Bed_Duck" : "Ending_Bed_NoDuck");
    }

    private void ChangeScene() //referenced on animation clip 
    {
        SceneManager.LoadScene(6);
    }

}
