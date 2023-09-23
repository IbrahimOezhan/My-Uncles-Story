using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending_Outside : MonoBehaviour
{
    [SerializeField] private Interactable steve;
    [SerializeField] private Animator man;
    [SerializeField] private Animator steveAnim;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        steve.Interact();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(6);
    }

    private void SteveAnim()
    {
        steveAnim.Play("Running");
    }

    private void ManAnim()
    {
        man.Play("Walk");
        StartCoroutine(FMOD_ManOutsideBreath());
    }

    private void SteveAnimDead()
    {
        steveAnim.Play("Dead");
    }

    private void Suprised()
    {
        steveAnim.Play("Suprised");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/Jumpscare_OutPlayer");
    }

    private IEnumerator FMOD_ManOutsideBreath()
    {
        int counter = 2;
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Chars/manFoley/manBreatheOut", man.gameObject);
    replay:
        yield return new WaitForSeconds(2.3f);
        if (man != null)
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Chars/manFoley/manBreatheOut", man.gameObject);
        counter--;
        //Debug.Log(counter);
        if (counter != 0) goto replay;
    }
}
