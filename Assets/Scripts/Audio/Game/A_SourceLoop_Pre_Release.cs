using UnityEngine;

public class A_SourceLoop_Pre_Release : MonoBehaviour
{
    private FMOD.Studio.EventInstance ss_House_BuildInstance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player_Interact>() && AudioManager.instance.gm.trip > 17)
        {
            ss_House_BuildInstance = FMODUnity.RuntimeManager.CreateInstance("snapshot:/SS_HouseBuild");
            ss_House_BuildInstance.start(); //also stopped on House Build FMOD event CMD
        }
        if (AudioManager.instance.gm.trip >= 5) A_RandomEmitters.rm.allowTimer = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (AudioManager.instance.gm.trip >= 5) A_RandomEmitters.rm.allowTimer = true;

        if (other.gameObject.GetComponent<Player_Interact>() && AudioManager.instance.gm.trip > 17)
        {
            ss_House_BuildInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
