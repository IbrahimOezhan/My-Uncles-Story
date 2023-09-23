
using System.Collections.Generic;
using UnityEngine;

public class A_RandomEmitters : MonoBehaviour
{
    public static A_RandomEmitters rm;
    private GameObject player;

    public bool allowTimer = false;

    //Far Emitter Vars
    [SerializeField] private GameObject parentObjectLoop;
    private GameObject emitterObject;
    private GameObject playerParent;
    private GameObject mainCamera;
    public GameObject man;
    public List<Transform> farEmittersList = new();
    private float CDTimerFarEmitters;
    public float minEmitterCD = 40; //timers seem to be at Game prefab component
    public float maxEmitterCD = 110;
    public bool debugRandomEmitter = true;
    public int forceCloseSticks = 0;
    public int forceSticks = 0;

    //Close Emitter Vars (tree foliage and hard collision)
    [SerializeField] private GameObject parentObjectOneShot;
    public bool allowCloseEmitter = false;
    public float CDTimerCloseEmitter;

    private void Start()
    {
        rm = this;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        allowTimer = true;
        //CDTimer = 5f;
        CDTimerFarEmitters = Random.Range(minEmitterCD, maxEmitterCD);
        Invoke(nameof(FMOD_Spawn_Random_Loop_Emitters), Random.Range(60f, 120f)); //have dynamic reverb on it and limited to within radius of player
    }

    private void OnEnable()
    {
        if (player == null) player = FindObjectOfType<Player_Interact>().gameObject;
    }

    private void Update()
    {
        if (player != null && emitterObject != null)
        {
            var camForwardVector = new Vector3(mainCamera.transform.forward.x, 0f, mainCamera.transform.forward.z);
            float angle = Vector3.SignedAngle((emitterObject.transform.position - mainCamera.transform.position).normalized, camForwardVector, Vector3.up);
            int angleInt = (int)Mathf.Round(angle);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ManualDirDynamicReverb", angleInt);
            if (debugRandomEmitter == true)
            {
                Debug.Log("Emitter Distance to Player " + Mathf.Ceil(Vector3.Distance(emitterObject.gameObject.transform.position, player.transform.position) * 100) / 100);
                Debug.Log("Player Angle Towards Emitter " + angleInt);
            }
        }
    }

    private void FixedUpdate()
    {
        //float secondsFar = Mathf.FloorToInt(CDTimerFarEmitters % 60);
        //float secondsClose = Mathf.FloorToInt(CDTimerCloseEmitter % 60);

        if (player != null)
        {
            if (Vector3.Distance(player.transform.position, AudioManager.instance.car.transform.position) <= 4) allowTimer = false;
            else allowTimer = true;
        }

        if (allowTimer == true)
        {
            if (CDTimerFarEmitters > 0)
            {
                CDTimerFarEmitters -= Time.deltaTime;
                //Debug.Log("far emitters timer " + secondsFar);
            }
            else
            {
                FMOD_Spawn_Random_Loop_Emitters(); //have dynamic reverb on it and limited to within radius of player
                Debug.Log("FMOD_Spawn_Random_Loop_Emitter");
            }

            if (CDTimerCloseEmitter > 0)
            {
                CDTimerCloseEmitter -= Time.deltaTime;
                //Debug.Log("close emitter timer " + secondsClose);
            }
            else allowCloseEmitter = true;
        }
    }
    public void FMOD_Spawn_Random_Loop_Emitters()
    {
        emitterObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        emitterObject.GetComponent<MeshRenderer>().enabled = false;
        emitterObject.GetComponent<BoxCollider>().enabled = false;
        emitterObject.transform.localScale = new Vector3(2, 1, 2);
        emitterObject.AddComponent<A_RandomEmitterObj>();

        emitterObject.transform.position = player.transform.position + Random.insideUnitSphere * 20;
        //emitterObject.transform.position += player.transform.position;
        emitterObject.transform.position = new Vector3(emitterObject.transform.position.x, 5, emitterObject.transform.position.z);
        emitterObject.transform.SetParent(parentObjectLoop.transform);

        if (AudioManager.instance.gm.trip >= 12)
        {
            minEmitterCD = 12;
            maxEmitterCD = 40;
        }
        else
        {
            minEmitterCD = 40;
            maxEmitterCD = 100;
        }
        CDTimerFarEmitters = Random.Range(minEmitterCD, maxEmitterCD);
    }

    /*private void OnDrawGizmos()
    {
        foreach (Transform transform in farEmittersList)
        {
            if (farEmittersList.Count > 0 && debugRandomEmitter == true)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, transform.localScale);
            }
        }
    }*/

}
