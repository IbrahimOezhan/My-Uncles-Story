using FMOD.Studio;
using FMODUnity;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    #region FMOD_EventReferences
    [Header("Player")]
    public EventReference playerFS;
    public EventReference compassTurn;
    //wood logOnHand pick up on Interactable_Car_Log.cs = playerLogPUinstance
    //wood logOnHand drop on Interactable_House_Log.cs = playerLogDropinstance
    //knife pick up on Interactable_House_Knife.cs

    [Header("Man")]
    public EventReference manFS;
    public EventReference manWalkFS;
    public EventReference manMovFoley;
    public EventReference manJumpscare;
    public EventReference manBlockIdle;
    public EventReference manBreatheIdle;
    public EventReference manPreChaseVox;
    public EventReference manDuringChaseVox;

    [Header("Environment")]
    public EventReference envDayAmbience;
    public EventReference envNightAmbience;
    public EventReference envOutNightAmbience;
    public EventReference envOutroWind;
    public EventReference envCicadaLoop;
    public EventReference envCricketLoop;
    public EventReference envGrasshopperLoop;
    public EventReference envOwlChirpLoop; //in A_Random_EmitterObj with cooldown 
    public EventReference envTreeSticks; //in A_Random_EmitterObj with cooldown 
    public EventReference envTreeWindBrushLoop;
    public EventReference envTreeCollision; //in A_Tree_Proximity 51 - soft collision || hard scare line 63
    public EventReference envDoorKnocking;

    [Header("Music")]
    public EventReference menuMusic;
    public EventReference menuCar;
    public EventReference inGameMusic;
    public EventReference inOutroDrone;
    public EventReference creditsMusic;
    //Credit Stinger at Ending_Credits.cs

    [Header("Events")]
    public EventReference treeCollisionClose; //not referenced 
    //Events in FMOD bus are mapped to endings

    [Header("UI")]
    public EventReference uiClick;
    public EventReference uiEnter;

    #endregion

    [Header("Bus")]
    public Bus masterBus;
    public Bus musicBus;
    public Bus environmentBus;

    [Header("Snapshots")]
    public EventReference ss_LookingAtMan; //lowers volume and high freqs when player not looking at Prefab
    public EventReference ss_OutroScene;
    public EventReference ss_PauseGame; //on Menu_Pause.cs

    //Instances
    public EventInstance menuMusicInstance;
    public EventInstance menuCarLoopInstance;
    public EventInstance gameMusicInstance;
    public EventInstance outroDroneInstance;
    public EventInstance creditsMusicInstance;
    public EventInstance dayAmbienceInstance;
    public EventInstance nightAmbienceInstance;
    public EventInstance nightOutAmbienceInstance;

    public EventInstance ss_OutroSceneInstance;
    public EventInstance ss_ToOutroTRXInstance; //started on Interactable Log
    public EventInstance ss_RestartDroneInstance; //On Dead State

    [HideInInspector] public GameObject player;
    [HideInInspector] public GameManager gm;
    [HideInInspector] public GameObject car;
    public FirstPersonController firstPersonController;

    public bool createTreeWindInstance = false;

    private float distToCar;
    public int sceneNbr;

    [SerializeField] private bool debugFmodStates = false;
    [SerializeField] private bool debugBackgroundInstances = true;

    public void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        //buses and instances and snapshots controls
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        environmentBus = RuntimeManager.GetBus("bus:/EnvironmentBus");
    }

    //controlled by trip nbr ::: parcels increase by pick 
    //start night trip 4; pick trip 5 parcel added; drop -nothing- 6; pick 7; drop -walks away- 8; pick 9; drop -chases- 10; so on 
    //last trip is 15 that turns into 16 when log drops , auto goes to scene 2
    //impar trip = carro
    //par trip house
    //after menu to game - trip == 0 . then player picks first log - trip == 1

    private void OnSceneLoaded(Scene current, LoadSceneMode next)
    {
        sceneNbr = current.buildIndex;
        //Debug.Log(current.input);

        switch (current.buildIndex)
        {
            case 0:
                //Main Menu Scene
                FMOD_Menu_State();
                break;
            case 1:
                //Main Game Scene
                //player/car/game manager reference getters on GameManager due to load order changes
                FMOD_InGame_Day_State();
                break;
            case 2:
                //Animation house build action
                if (!player) player = GameObject.FindGameObjectWithTag("MainCamera");
                FMOD_InGame_Outro_State();
                break;
            case 3:
                //Outro Scene Inside House
                if (!player) player = GameObject.FindGameObjectWithTag("Player");
                FMOD_InGame_Outro_State();
                break;
            case 4:
                //Animation 2 Endings Outside
                //FMOD_Outro_Endings_Outside(); FMOD state change on Interactable door because needs to be before scene change
                break;
            case 5:
                //Animation Ending Bed
                FMOD_Outro_Ending_Bed();
                break;
            case 6:
                //Ending Credits
                FMOD_Outro_Ending_Credits();
                break;
            default:
                //default is Menu due to normal play order
                FMOD_Menu_State();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) //REVIEW: H Button test
        {
            //SceneManager.LoadScene(2); //building house animation pre ending knock

            //A_SourceLoop_Controller.instance.FMOD_Master_Reset_SL_Loop_Emitters();

            //A_SourceLoop_Controller.instance.FMOD_SL_Count_Apply_Play();

            //StartCoroutine(Test_Player_Teleport_House()); //set to house

        }
        //if (Input.GetKeyDown(KeyCode.J)) StartCoroutine(Test_Player_Teleport_Car());

        //if (sceneNbr == 1) Debug.Log("game trip " + gm.trip);

        //InGame Music + NightAmb + DayAmb instances debug
        if (debugBackgroundInstances) FMOD_Debug_Global_Instances();

        if (player != null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            dayAmbienceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(player));
            nightAmbienceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(player));
        } //Set FMOD day/night instance 3D attributes

        if (player != null && SceneManager.GetActiveScene().buildIndex == 4)
        {
            nightOutAmbienceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(player));
        } //Set FMOD Out Night Instance 3D

        if (gm != null && gm.isNight == false && gm.trip == 4)
        {
            distToCar = Vector3.Distance(car.transform.position, player.transform.position); //at house is 70 from car
            RuntimeManager.StudioSystem.setParameterByName("MDistToCar", distToCar);
        } //Sets Dist to Car Parameter
    }

    //second idea for random sound stuff;; consider all emitters, make a list of all instances then when
    //one gets selected it reduces the chance of replaying. Consider doing in parameters or C#

    public void FMOD_Menu_State() //called at start + pause + ending
    {
        if (debugFmodStates) Debug.Log("FMOD_Menu_State");

        RuntimeManager.StudioSystem.setParameterByName("SkipIntro", 0);
        RuntimeManager.StudioSystem.setParameterByName("StartBackgroundNightAmb", 0);

        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "Menu_Main");
        RuntimeManager.StudioSystem.setParameterByName("Game_Trip", 0);

        creditsMusicInstance.getPlaybackState(out PLAYBACK_STATE creditsMusicState);
        if (creditsMusicState != PLAYBACK_STATE.STOPPED)
        {
            creditsMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            creditsMusicInstance.release();
        }
        nightAmbienceInstance.getPlaybackState(out PLAYBACK_STATE nightAmbState);
        if (nightAmbState != PLAYBACK_STATE.STOPPED)
        {
            nightAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            nightAmbienceInstance.release();
        }
        dayAmbienceInstance.getPlaybackState(out PLAYBACK_STATE dayAmbState);
        if (dayAmbState != PLAYBACK_STATE.STOPPED)
        {
            dayAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            dayAmbienceInstance.release();
        }
        gameMusicInstance.getPlaybackState(out PLAYBACK_STATE inGameMusicState);
        if (inGameMusicState != PLAYBACK_STATE.STOPPED || inGameMusicState != PLAYBACK_STATE.STOPPING)
        {
            gameMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            gameMusicInstance.release();
        }

        if (A_SourceLoop_Controller.instance != null)
            A_SourceLoop_Controller.instance.FMOD_Master_Reset_SL_Loop_Emitters();

        menuMusicInstance = RuntimeManager.CreateInstance(menuMusic);
        menuMusicInstance.start();
        menuCarLoopInstance = RuntimeManager.CreateInstance(menuCar);
        menuCarLoopInstance.start(); //also sets paramater GameState and Game_Trip on fmod side with command 
        menuCarLoopInstance.release();
    }

    public void FMOD_InGame_Day_State()
    {
        if (debugFmodStates) Debug.Log("FMOD_InGame_Day_State");
        if (MainManager.instance.skipDay) return;
        //the double car opening is not a bug, only happens when nightManager is loaded on start
        // on Intro.cs
        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "In_Game_Day");
        //menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 
        //menuMusicInstance.release();

        dayAmbienceInstance = RuntimeManager.CreateInstance(envDayAmbience);
        dayAmbienceInstance.start();
    }

    public void FMOD_InGame_Night_State() //Called in Nightmanager.cs and SpawnGame.cs (called in RestartManager.cs) //consider pausing insts
    {
        if (debugFmodStates) Debug.Log("FMOD_InGame_Night_State");

        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "In_Game_Night");

        dayAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        dayAmbienceInstance.release();

        //night music starts in A_MusicStarterNight Collider on Car Object

        nightAmbienceInstance.getPlaybackState(out PLAYBACK_STATE nightAmbState);
        if (nightAmbState == PLAYBACK_STATE.STOPPED || nightAmbState == PLAYBACK_STATE.STOPPING)
        {
            nightAmbienceInstance = RuntimeManager.CreateInstance(envNightAmbience);
            nightAmbienceInstance.start();
        }

        gm.GetComponent<A_RandomEmitters>().enabled = true;

        if (A_SourceLoop_Controller.instance != null)
            A_SourceLoop_Controller.instance.FMOD_Master_Reset_SL_Loop_Emitters();
        A_SourceLoop_Controller.instance.Invoke(nameof(A_SourceLoop_Controller.instance.FMOD_SL_Count_Apply_Play), 2f);
    }

    public void FMOD_InGame_Dead_State() //In SpawnGame.cs called in Man.cs -spawnGame.Death(gameObject);
    {
        if (debugFmodStates) Debug.Log("FMOD_InGame_Dead_State");

        if (A_SourceLoop_Controller.instance != null)
            A_SourceLoop_Controller.instance.FMOD_Master_Reset_SL_Loop_Emitters();

        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "Dead");
        ss_RestartDroneInstance = RuntimeManager.CreateInstance("snapshot:/SS_RestartDrone_SideChain");
        ss_RestartDroneInstance.start();
        RuntimeManager.StudioSystem.setParameterByName("isChasing", 0);
    }

    public void FMOD_InGame_Outro_State()
    {
        if (debugFmodStates) Debug.Log("FMOD_InGame_Outro_State");
        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "In_Game_Outro");

        if (sceneNbr == 2)
        {
            gameMusicInstance.getPlaybackState(out PLAYBACK_STATE inGameMusicState);
            if (inGameMusicState != PLAYBACK_STATE.STOPPED || inGameMusicState != PLAYBACK_STATE.STOPPING)
            {
                gameMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                gameMusicInstance.release();
            }
            nightAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            nightAmbienceInstance.release();

            if (A_SourceLoop_Controller.instance != null)
                A_SourceLoop_Controller.instance.FMOD_Master_Reset_SL_Loop_Emitters();
            return;
        }

        outroDroneInstance = RuntimeManager.CreateInstance(inOutroDrone);
        outroDroneInstance.start();

        ss_OutroSceneInstance = RuntimeManager.CreateInstance(ss_OutroScene);
        ss_OutroSceneInstance.start();
    }

    public void FMOD_Outro_Ending_Bed()
    {
        if (debugFmodStates) Debug.Log("FMOD_InEnding_State_Bed");
        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "In_Ending_Bed");

        outroDroneInstance.getPlaybackState(out PLAYBACK_STATE inOutroDroneState);
        if (inOutroDroneState == PLAYBACK_STATE.PLAYING)
        {
            outroDroneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            outroDroneInstance.release();
        }

        ss_OutroSceneInstance.getPlaybackState(out PLAYBACK_STATE inOutroSnapShotState);
        if (inOutroSnapShotState != PLAYBACK_STATE.PLAYING)
        {
            ss_OutroSceneInstance = RuntimeManager.CreateInstance(ss_OutroScene);
            ss_OutroSceneInstance.start();
        }
    }

    public void FMOD_Outro_Endings_Outside()
    {
        if (debugFmodStates) Debug.Log("FMOD_InEnding_State_Outside");
        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "In_Ending_Outside");

        nightOutAmbienceInstance = RuntimeManager.CreateInstance(envOutNightAmbience);
        nightOutAmbienceInstance.start();

        outroDroneInstance.getPlaybackState(out PLAYBACK_STATE inOutroDroneState);
        if (inOutroDroneState == PLAYBACK_STATE.PLAYING)
        {
            outroDroneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            outroDroneInstance.release();
        }
        ss_OutroSceneInstance.getPlaybackState(out PLAYBACK_STATE inOutroSnapShotState);
        if (inOutroSnapShotState == PLAYBACK_STATE.PLAYING || inOutroSnapShotState == PLAYBACK_STATE.STARTING)
        {
            ss_OutroSceneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ss_OutroSceneInstance.release();
        }

        A_SourceLoop_Controller.instance.FMOD_SL_Count_Apply_Play();
    }

    public void FMOD_Outro_Ending_Credits()
    {
        if (debugFmodStates) Debug.Log("FMOD_InEnding_State_Credits");
        RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameState", "In_Ending_Credits");

        if (A_SourceLoop_Controller.instance != null)
            A_SourceLoop_Controller.instance.FMOD_Master_Reset_SL_Loop_Emitters();
        if (sceneNbr != 5)
        {
            creditsMusicInstance = RuntimeManager.CreateInstance(creditsMusic);
            creditsMusicInstance.start();
        }
        nightOutAmbienceInstance.getPlaybackState(out PLAYBACK_STATE nightOutAmbienceState);
        if (nightOutAmbienceState == PLAYBACK_STATE.PLAYING)
        {
            nightOutAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            nightOutAmbienceInstance.release();
        }
    }

    //Debug Functions

    private IEnumerator Test_Player_Teleport_House()
    {
        firstPersonController.canMove = false;
        player.transform.position = FindObjectOfType<A_SourceLoop_Pre_Release>().gameObject.transform.position;
        yield return new WaitForSeconds(0.5f);
        firstPersonController.canMove = true;
    }

    private IEnumerator Test_Player_Teleport_Car()
    {
        firstPersonController.canMove = false;
        player.transform.position = car.transform.position;
        yield return new WaitForSeconds(0.5f);
        firstPersonController.canMove = true;
    }

    public void A_AudioSourcePosition_GameObj() //set and use function to receive other obj position 
    {
        GameObject aSourceObj;
        aSourceObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        aSourceObj.name = "AudioSourceDebugObject";
        aSourceObj.GetComponent<BoxCollider>().enabled = false;
        aSourceObj.transform.localScale = new Vector3(1, 1, 1);
        aSourceObj.transform.position = transform.position;
    }

    private void FMOD_Debug_Global_Instances()
    {
        nightAmbienceInstance.getPlaybackState(out PLAYBACK_STATE nightAmbState);
        Debug.Log("night amb state " + nightAmbState);
        nightAmbienceInstance.getDescription(out EventDescription nightAmbDescription);
        nightAmbDescription.getInstanceCount(out int nightcount);
        Debug.Log("night amb count " + nightcount);

        dayAmbienceInstance.getPlaybackState(out PLAYBACK_STATE dayAmbState);
        Debug.Log("day amb state " + dayAmbState);
        dayAmbienceInstance.getDescription(out EventDescription dayAmbDescription);
        dayAmbDescription.getInstanceCount(out int daycount);
        Debug.Log("day amb count " + daycount);

        gameMusicInstance.getPlaybackState(out PLAYBACK_STATE inGameMusicState);
        Debug.Log("music state " + inGameMusicState);
        gameMusicInstance.getDescription(out EventDescription musicDescription);
        musicDescription.getInstanceCount(out int countMusic);
        Debug.Log("music count " + countMusic);

        if (sceneNbr == 3)
        {
            FindObjectOfType<A_Outro_Audio>().outroWindInstance.getDescription(out EventDescription outroWindDescription);
            outroWindDescription.getInstanceCount(out int outroWindCount);
            Debug.Log("outro wind count " + outroWindCount);
            outroWindDescription.getInstanceList(out EventInstance[] Instarray);
            foreach (EventInstance inst in Instarray) Debug.Log("outro how many instances active " + inst);
        }

    }


    /*void OnDrawGizmos() //Draw gizmos is updated per frame and always on. Create function to activate gizmos?
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        foreach (Transform transform in X_list)
        {
            if (x_list.Count > 0 && debugASource == true)
            {

            }
        }
    }*/

    /*void Remind_Debug_Lines()
     {
         //ss_OutroSceneInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE State);
         //Debug.Log(State);

         //reminder
         //manFSInstance.getDescription(out FMOD.Studio.EventDescription eventDescription);
         //eventDescription.getInstanceCount(out int count);
         //Debug.Log(count);

         /*environmentBus.getPaused(out bool pauseG);
         if (pauseG == false) environmentBus.setPaused(true);
         else environmentBus.setPaused(false);
         Debug.Log(pauseG);

        FMOD.Studio.PLAYBACK_STATE state;
        nightAmbienceInstance.getPlaybackState(out state);
        Debug.Log(state);


        if (Input.GetKeyDown(KeyCode.F)) tempDistanceToggle = !tempDistanceToggle;
        private AnimatorClipInfo[] TEMPclipInfo;
        TEMPclipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Debug.Log(TEMPclipInfo[0].clip.name + " world loc " + transform.position + " distance " + Vector3.Distance(transform.position, playerTransform.position));
    }*/
}
