using UnityEngine;
using UnityEngine.SceneManagement;

public class A_Fmod_End_Seq : MonoBehaviour
{
    //Ending 0: Temporary empty variable
    //Ending 1: Player Death - Interact with Bed after knocking - Man jumpscare bed
    //Ending 2: Player Death - Interact with Door without knife - Steve charge 
    //Ending 3: Player Kill - Interact with Door with knife - Player charge to kill Steve

    //Prefab distance outside 8.5

    public void FMOD_Play_HouseSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Events/House_Build");
    }

    public void FMOD_Play_HouseSFX_Door()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/playerDoorToOutro");
    }

    public void FMOD_EndOut_Kill()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Events/End_Player_Kill");
    }
    public void FMOD_EndOut_Death()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Events/End_Player_Death_Out");
    }

    public void FMOD_EndBed()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/End_Player_Death_Bed", GameObject.FindGameObjectWithTag("MainCamera"));
    }
    public void FMOD_EndBed_Duck()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/End_Player_Duck", GameObject.FindGameObjectWithTag("MainCamera"));
    }

    public void FMOD_EndBed_Glass()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/End_Player_Death_Glass", GameObject.FindGameObjectWithTag("MainCamera"));
    }

    public void FMOD_EndBed_Heart()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Events/End_Player_Death_Heart");
    }

    public void SceneChange()
    {
        SceneManager.LoadScene(3);
    }
}
