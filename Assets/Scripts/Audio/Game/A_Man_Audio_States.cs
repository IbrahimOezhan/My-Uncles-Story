using UnityEngine;

public class A_Man_Audio_States : MonoBehaviour
{
    [SerializeField] private GameObject manEmitter;

    public FMOD.Studio.EventInstance manIdleAuraInstance;
    public FMOD.Studio.EventInstance manPreChaseVoxInstance;
    public FMOD.Studio.EventInstance manDuringChaseVoxInstance;

    private GameObject player;

    //actions
    public System.Action fmod_Chase_State_Action;

    //timer
    public float CD_Man_DuringChase_Vox = 7f;
    public bool allowManChaseVoxTimer = false;

    public bool allowMusicStop = false;

    private void Start()
    {
        if (player == null) player = FindObjectOfType<StarterAssets.FirstPersonController>().gameObject;

        CD_Man_DuringChase_Vox = Random.Range(4f, 8f);
        fmod_Chase_State_Action += A_Set_Chase_State_Off;
        fmod_Chase_State_Action.Invoke();
    }

    private void Update()
    {
        if (AudioManager.instance.sceneNbr == 1 || AudioManager.instance.sceneNbr == 5)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ManualDirManToPlayer", A_Direction_Angle());
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ManualDistMan", Vector3.Distance(player.transform.position, manEmitter.transform.position));
        }

        manPreChaseVoxInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state == FMOD.Studio.PLAYBACK_STATE.STARTING || state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            manPreChaseVoxInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter));
        }
        manDuringChaseVoxInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state2);
        if (state2 == FMOD.Studio.PLAYBACK_STATE.STARTING || state2 == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            manDuringChaseVoxInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter));
        }

        if (Input.GetKeyDown(KeyCode.H)) //REVIEW: H Button Man Test
        {
            //A_Play_PreChaseManVox_SFX();

            //fmod_DuringChase_ManVox_Action?.Invoke();

            //FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/manDeathRestart");
            //Debug.Log("distance to Prefab" + Vector3.Distance(player.transform.position, manEmitter.transform.position));
            //Debug.Log("direction angle to Prefab" + A_Direction_Angle());
        }
        //Debug.DrawLine(GameObject.FindGameObjectWithTag("MainCamera").transform.position, manEmitter.transform.position, Color.blue);
        //Debug.Log("direction angle to Prefab" + A_Direction_Angle());
    }

    //passive
    //state 0 only stands there and slowly moves to player when not looking at Prefab

    //non passive 
    //state 1 chases after player looked at Prefab after a short amount of time (0.3-3 sec). Triggered from state 0 or not
    //state 2 chases non escape when distance < 3.5. Triggered from state 0 and state 1

    //scripted behavior triggered on player log drop
    //state 3 walks away 
    //state 4 base line doing nothing

    //Distance < 1.5 goes to restart scene and restarts game

    private void FixedUpdate()
    {
        if (allowManChaseVoxTimer)
        {
            if (CD_Man_DuringChase_Vox > 0)
            {
                CD_Man_DuringChase_Vox -= Time.deltaTime;
                float seconds = Mathf.FloorToInt(CD_Man_DuringChase_Vox % 60);
                //Debug.Log(seconds);
            }
            else
            {
                A_Play_DuringChaseManVox_SFX();
                //Debug.Log("Triggered Laugh Vox by random timer");
            }
        }
    }

    public int A_Direction_Angle()
    {
        var camForwardVector = new Vector3(player.transform.forward.x, 0f, player.transform.forward.z);
        float angle = Vector3.SignedAngle((transform.position - player.transform.position).normalized, camForwardVector, Vector3.up);
        return (int)System.Math.Round(angle);
    }

    public void A_Chase_Reset() //Called on Man GetRidOfMan 
    {
        //Debug.Log("A_Chase_Reset");
        fmod_Chase_State_Action += A_Set_Chase_State_Off;
        fmod_Chase_State_Action -= A_Set_Chase_State_On;
        fmod_Chase_State_Action?.Invoke();
    }

    public void A_Chase_Start()
    {
        //Vox and music change is out of sync due to gamemusic to chase TRX in fmod. Leave it to allow that delay or fix it? 
        //Debug.Log("A CHASE START");

        A_Play_PreChaseManVox_SFX();

        fmod_Chase_State_Action += A_Set_Chase_State_On;
        fmod_Chase_State_Action -= A_Set_Chase_State_Off;
        fmod_Chase_State_Action?.Invoke();
    }

    public void A_Play_NoChase_Jumpscare()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/manJumpscare");
        GetComponentInChildren<A_Man_Movement>().Stop_Release_IdleAura();
    }

    public void A_Play_PreChaseManVox_SFX()
    {
        //Debug.Log("fmod Pre Chase Man Vox");
        manPreChaseVoxInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.manPreChaseVox);
        manPreChaseVoxInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        manPreChaseVoxInstance.start();
        manPreChaseVoxInstance.release();
        GetComponentInChildren<A_Man_Movement>().Stop_Release_IdleAura();
    }

    public void A_Play_DuringChaseManVox_SFX()
    {
        //Debug.Log("fmod During Chase Man Vox");
        manDuringChaseVoxInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.manDuringChaseVox);
        manDuringChaseVoxInstance.start();
        manDuringChaseVoxInstance.release();
        CD_Man_DuringChase_Vox = Random.Range(8f, 14f);
    }

    public void A_Play_Death_SFX()
    {
        //Debug.Log("FMOD Game Over SFX");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Chars/manDeathRestart"); //laugh and kill sfx then wait for this to finish to TRX
    }

    public void A_Set_Chase_State_On()
    {
        //Debug.Log("FMOD isChasing - 1 On");
        allowManChaseVoxTimer = true;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isChasing", 1);
        GetComponentInChildren<A_Man_Movement>().Stop_Release_IdleAura(); //need testing to check for instances numbers
    }

    public void A_Set_Chase_State_Off()
    {
        //Debug.Log("FMOD isChasing - 0 Off");
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isChasing", 0);
        allowManChaseVoxTimer = false;
    }
}
