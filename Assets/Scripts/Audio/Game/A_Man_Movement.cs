using System.Collections;
using UnityEngine;

public class A_Man_Movement : MonoBehaviour
{
    private Man manScript;
    [SerializeField] private GameObject manEmitter;
    private FMOD.Studio.EventInstance manFSInstance;
    private FMOD.Studio.EventInstance manWalkFSInstance;
    private FMOD.Studio.EventInstance manMovInstance;
    public FMOD.Studio.EventInstance manIdleAuraInstance;
    public FMOD.Studio.EventInstance manIdleBreatheInstance;
    public FMOD.Studio.EventInstance manInsideTreeInstance;

    private FMOD.Studio.EventInstance manIdleLookAtInstance_SS;

    public bool allowWalkStepsSound;
    private int forceSilentWalk;
    private int randomChanceWalkSteps = 100;

    //private float reduceWalkTime;
    //private float walkStepsTimer = 1f;
    private bool doManTreeOnce;
    private bool coroutineChanceStepsDone = true;

    private void Awake()
    {
        manScript = GetComponentInParent<Man>();
        manIdleLookAtInstance_SS = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.ss_LookingAtMan);
    }

    private void Start()
    {
        allowWalkStepsSound = true;
        manIdleLookAtInstance_SS.start();
        if (coroutineChanceStepsDone) StartCoroutine(ChanceWalkSteps());
    }

    private void FixedUpdate()
    {

        manIdleAuraInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE manAuraState);
        if (manAuraState == FMOD.Studio.PLAYBACK_STATE.STARTING || manAuraState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            manIdleAuraInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter.transform));
        }

        manInsideTreeInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE manTreeState);
        if (manTreeState == FMOD.Studio.PLAYBACK_STATE.STARTING || manTreeState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            manInsideTreeInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter.transform));
        }
        //Debug.Log("allowWalkSteps " + allowWalkStepsSound);
        //Debug.Log("randomChanceWalkSteps " + randomChanceWalkSteps);
        //Debug.Log("forceSilentWalk " + forceSilentWalk);
        //if (manScript.A_Direction_Angle() <= -50 && manScript.A_Direction_Angle() >= 50)
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("A_TreeCol"))
        {
            manScript.isInTree = true;
            AnimatorClipInfo[] clipInfo = manScript.animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo[0].clip.name == "Walk" && !doManTreeOnce)
            {
                Debug.Log("inside tree");
                doManTreeOnce = true;
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isInsideTreeMan", 1);
                manInsideTreeInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE manTreeState);

                if (manTreeState != FMOD.Studio.PLAYBACK_STATE.STARTING && manTreeState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                {
                    manInsideTreeInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Chars/manFoley/manStepsTree");
                    manInsideTreeInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter.transform));
                    manInsideTreeInstance.start();
                    Debug.Log("manInsideTreeInstance start");
                }
            }
            else if (clipInfo[0].clip.name != "Walk") StopManTreeSounds();
        }
    }

    private void StopManTreeSounds()
    {
        doManTreeOnce = false;
        manInsideTreeInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE manTreeState);
        if (manTreeState != FMOD.Studio.PLAYBACK_STATE.STOPPING && manTreeState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isInsideTreeMan", 0);
            manInsideTreeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            manInsideTreeInstance.release();
            Debug.Log("manInsideTreeInstance stop");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("A_TreeCol"))
        {
            manScript.isInTree = false;
            StopManTreeSounds();
        }
    }

    public IEnumerator ChanceWalkSteps() //trigger A_Set_Chase_State_Off action + anim.idle
    {
        //Debug.Log("ChanceWalkSteps started");
        coroutineChanceStepsDone = false;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("FakeDistMan", Random.Range(1, 4));
        yield return new WaitForSeconds(1);
        randomChanceWalkSteps = Random.Range(1, 100);
        Debug.Log("random chance walk " + randomChanceWalkSteps);
        if (randomChanceWalkSteps <= 20 || forceSilentWalk >= Random.Range(10, 30))
        {
            allowWalkStepsSound = false;
            forceSilentWalk = 0;
        }
        else
        {
            allowWalkStepsSound = true;
            forceSilentWalk++;
        }
        coroutineChanceStepsDone = true;
        //Debug.Log("ChanceWalkSteps ended");
    }

    /*public IEnumerator FMOD_WalkSteps() //REVIEW: needs to be loop and stop/reset on idle.anim -- not being used
    {
    returnhere:
        reduceWalkTime = Random.Range(0.1f, 0.3f);
        Play_Man_Walk_Steps();

        yield return new WaitForSeconds(walkStepsTimer -= reduceWalkTime);
        Mathf.Clamp(walkStepsTimer, 0.2f, 0.1f);

        Debug.Log("ChanceReRoll routine");
        Debug.Log("walkStepsTimer " + walkStepsTimer);
        if (!manScript.IsBeingSeen(Mathf.Infinity)) goto returnhere;
    }*/
    //UNDONE: Prefab movement idea below
    /*if (GetComponentInChildren<A_Man_Movement>().allowWalkStepsRoutine == true) //unfinished idea walk faster progressive
    {   
        Debug.Log("call FMOD_WalkSteps");
        StartCoroutine(GetComponentInChildren<A_Man_Movement>().FMOD_WalkSteps());
    }*/

    public void Play_Man_Steps()
    {
        manFSInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.manFS);
        manFSInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter.transform));
        manFSInstance.start();
        Debug.Log("manFSInstance start");
        //manFSInstance.release();
        manWalkFSInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED) manWalkFSInstance.release();
    }

    public void Play_Man_Walk_Steps()
    {
        if (AudioManager.instance.sceneNbr == 4)
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Chars/manMovement/manFS_Walk_Outside", manEmitter);

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ManNinjaMode", 0);

        if (allowWalkStepsSound == false)
        {
            Debug.Log("Ninja Mode Walk steps false");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ManNinjaMode", 1);
        }

        if (AudioManager.instance.sceneNbr != 1) return;

        Debug.Log("Man Walk Steps");
        manWalkFSInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.manWalkFS);
        manWalkFSInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter.transform));
        manWalkFSInstance.start();


        //consider moving walk steps here through a coroutine and make it async: play progressively faster 
    }

    public void Play_Man_Mov()
    {
        manMovInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.manMovFoley);
        manMovInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter.transform));
        manMovInstance.start();
        //Debug.Log("manMovFoley start");
    }

    public void Stop_Man_Mov()
    {
        manMovInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        manMovInstance.release();
        //Debug.Log("manMovFoley stop");
    }

    private void OnDestroy() //stop SS when chasing or other state than 0
    {
        Stop_Release_IdleAura();
        StopManTreeSounds();
    }
    private void OnDisable()
    {
        Stop_Release_IdleAura();
        StopManTreeSounds();
    }

    public void Play_Idle() //trigger in anim.idle
    {
        Debug.Log("Man Idle Trigged");
        manIdleAuraInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state != FMOD.Studio.PLAYBACK_STATE.STARTING && state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            if (AudioManager.instance.sceneNbr == 1 || AudioManager.instance.sceneNbr == 4)
            {
                Debug.Log("Man Idle start playing");
                manIdleAuraInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.manBlockIdle);
                manIdleAuraInstance.start();

                manInsideTreeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                manInsideTreeInstance.release();
            }
        }

        if (coroutineChanceStepsDone) StartCoroutine(ChanceWalkSteps());
    }

    public void Stop_Release_IdleAura()
    {
        manIdleAuraInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        manIdleAuraInstance.release();

        manIdleLookAtInstance_SS.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        manIdleLookAtInstance_SS.release();

        manIdleBreatheInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        manIdleBreatheInstance.release();

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isInsideTreeMan", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ManNinjaMode", 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("FakeDistMan", 1);

        Debug.Log("Stop_Release_IdleAura");
    }

    public void Play_Idle_Breath() //Trigger in anim.idle. Consider allowing breath to play when Prefab is behind
    {
        if (AudioManager.instance.sceneNbr == 1)
        {
            //Debug.Log("breath trigger");
            manIdleBreatheInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.manBreatheIdle);
            manIdleBreatheInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(manEmitter.transform));
            manIdleBreatheInstance.start();
            manIdleBreatheInstance.release();
        }
    }
}
