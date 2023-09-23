using UnityEngine;

public class A_MusicStarterNight : MonoBehaviour
{
    private GameManager gm;
    private bool doOnce = true;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && gm.isNight && doOnce)
        {
            //Debug.Log("music night start");
            AudioManager.instance.gameMusicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE inGameMusicState);
            if (inGameMusicState == FMOD.Studio.PLAYBACK_STATE.STOPPED || inGameMusicState == FMOD.Studio.PLAYBACK_STATE.STOPPING)
            {
                AudioManager.instance.gameMusicInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.inGameMusic);
                AudioManager.instance.gameMusicInstance.start();
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("StartBackgroundNightAmb", 10);
            doOnce = false;
        }
    }
}
