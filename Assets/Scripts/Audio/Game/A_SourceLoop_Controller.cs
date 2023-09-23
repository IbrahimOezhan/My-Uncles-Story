using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_SourceLoop_Controller : MonoBehaviour
{
    public static A_SourceLoop_Controller instance;
    private List<GameObject> SL_Total_Emitters_gameObject_List = new(); //SL_Emitters_GameObj_List

    private List<GameObject> emitter_gameObjects_List_A = new(); //emitter_Group1_Int_UList
    private List<GameObject> emitter_gameObjects_List_B = new(); //emitter_Group2_Int_UList
    private List<GameObject> emitter_gameObjects_List_C = new(); //emitter_Group3_Int_UList

    private int SL_Types = 3;
    private int emitters_Per_List;
    private float overlapLimiter = 18;
    private bool sl_Emitter_Init = false;
    private bool checkLoopStates = false;
    private bool isEverythingStopped = false;
    private bool debug_SL_Emitters = false;
    private EventInstance cicadaLoopInstance;
    private EventInstance cricketLoopInstance;
    private EventInstance grasshopperLoopInstance;
    private PLAYBACK_STATE cicadaState;
    private PLAYBACK_STATE cricketState;
    private PLAYBACK_STATE grasshopperState;

    //public FMOD.Studio.EventDescription cicadaEventDescription;
    //public FMOD.Studio.EventInstance[] cicadaArray;
    //int cicadaCount;
    //cicadaLoopInstance.getDescription(out cicadaEventDescription);
    //cicadaEventDescription.getInstanceCount(out int count1);
    //cicadaCount = count1;

    public void Start()
    {
        if (instance != null) Destroy(gameObject);
        else
        {
            instance = this;
            sl_Emitter_Init = false;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (checkLoopStates)
        {
            cicadaLoopInstance.getPlaybackState(out cicadaState);
            cricketLoopInstance.getPlaybackState(out cricketState);
            grasshopperLoopInstance.getPlaybackState(out grasshopperState);

            if (cicadaState == PLAYBACK_STATE.STOPPED
                && cricketState == PLAYBACK_STATE.STOPPED
                && grasshopperState == PLAYBACK_STATE.STOPPED)
                isEverythingStopped = true;
            else isEverythingStopped = false;
        }

        if (debug_SL_Emitters)
        {
            cicadaLoopInstance.getDescription(out EventDescription eventDescription1);
            eventDescription1.getInstanceCount(out int count1);
            Debug.Log("cicada " + count1);

            cricketLoopInstance.getDescription(out EventDescription eventDescription2);
            eventDescription2.getInstanceCount(out int count2);
            Debug.Log("cricket " + count2);

            grasshopperLoopInstance.getDescription(out EventDescription eventDescription3);
            eventDescription3.getInstanceCount(out int count3);
            Debug.Log("grasshopper " + count3);

            Debug.Log("sl_Emitter_Init " + sl_Emitter_Init);
        }

        //Debug.Log(isEverythingStopped);
        //Debug.Log(checkLoopStates);
    }

    public void FMOD_SL_Count_Apply_Play()
    {
        foreach (GameObject SL_Emitters in GameObject.FindGameObjectsWithTag("A_SL_Emitter"))
        {
            SL_Total_Emitters_gameObject_List.Add(SL_Emitters);

            if (SL_Emitters.GetComponent<SphereCollider>() == null)
            {
                SL_Emitters.AddComponent<SphereCollider>();
                SphereCollider col = SL_Emitters.GetComponent<SphereCollider>();
                col.isTrigger = true;
                col.radius = 5f;
            }
            SL_Emitters.transform.position = new Vector3(SL_Emitters.transform.position.x, 5, SL_Emitters.transform.position.z);
        }
        emitters_Per_List = SL_Total_Emitters_gameObject_List.Count / SL_Types;

        if (debug_SL_Emitters)
        {
            Debug.Log("list total " + SL_Total_Emitters_gameObject_List.Count);
            Debug.Log("target group chunks " + emitters_Per_List);
        }

        Physics.SyncTransforms();

        Check_Apply_Play_Emitters_A();

        if (debug_SL_Emitters)
        {
            Debug.Log("list A total " + emitter_gameObjects_List_A.Count);
            Debug.Log("remaining from total " + SL_Total_Emitters_gameObject_List.Count);
        }

        Check_Apply_Play_Emitters_B();

        if (debug_SL_Emitters)
        {
            Debug.Log("list B total " + emitter_gameObjects_List_B.Count);
            Debug.Log("remaining from total " + SL_Total_Emitters_gameObject_List.Count);
        }

        Check_Apply_Play_Emitters_C();

        if (debug_SL_Emitters)
        {
            Debug.Log("list C total " + emitter_gameObjects_List_C.Count);
            Debug.Log("remaining from total " + SL_Total_Emitters_gameObject_List.Count);
        }

        SL_Total_Emitters_gameObject_List.Clear();
        sl_Emitter_Init = true;
    }

    private void Check_Apply_Play_Emitters_A()
    {
        for (int a = emitters_Per_List; a > 0; a--)
        {
            int randomIndex = Random.Range(0, SL_Total_Emitters_gameObject_List.Count - 1);
            GameObject temp = SL_Total_Emitters_gameObject_List[randomIndex];
            SL_Total_Emitters_gameObject_List.Remove(temp);
            emitter_gameObjects_List_A.Add(temp);

            if (temp.layer != LayerMask.GetMask("Default")) temp.layer = 0;

            if (Physics.OverlapSphere(temp.transform.position, overlapLimiter, LayerMask.GetMask("SL_Emitter_A")).Length > 0)
            {
                emitter_gameObjects_List_A.Remove(temp);
                temp.layer = 0;
            }
            else
            {
                cicadaLoopInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envCicadaLoop);
                cicadaLoopInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(temp));
                cicadaLoopInstance.start();
                temp.layer = LayerMask.NameToLayer("SL_Emitter_A");
            }
            //ttemp.AddComponent<FMODUnity.StudioEventEmitter>();
            //ttemp.GetComponent<FMODUnity.StudioEventEmitter>().EventReference = AudioManager.instance.envCicadaLoop;
        }
    }

    private void Check_Apply_Play_Emitters_B()
    {
        for (int b = emitters_Per_List; b > 0; b--)
        {
            int randomIndex = Random.Range(0, SL_Total_Emitters_gameObject_List.Count - 1);
            GameObject temp = SL_Total_Emitters_gameObject_List[randomIndex];
            SL_Total_Emitters_gameObject_List.Remove(temp);
            emitter_gameObjects_List_B.Add(temp);
            if (temp.layer != LayerMask.GetMask("Default")) temp.layer = 0;
            if (Physics.OverlapSphere(temp.transform.position, overlapLimiter, LayerMask.GetMask("SL_Emitter_B")).Length > 0)
            {
                emitter_gameObjects_List_B.Remove(temp);
                temp.layer = 0;
            }
            else
            {
                cricketLoopInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envCricketLoop);
                cricketLoopInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(temp));
                cricketLoopInstance.start();

                temp.layer = LayerMask.NameToLayer("SL_Emitter_B");
            }
        }
    }

    private void Check_Apply_Play_Emitters_C()
    {
        for (int c = SL_Total_Emitters_gameObject_List.Count; c > 0; c--)
        {
            int randomIndex = Random.Range(0, SL_Total_Emitters_gameObject_List.Count - 1);
            GameObject temp = SL_Total_Emitters_gameObject_List[randomIndex];
            SL_Total_Emitters_gameObject_List.Remove(temp);
            emitter_gameObjects_List_C.Add(temp);
            if (temp.layer != LayerMask.GetMask("Default")) temp.layer = 0;
            if (Physics.OverlapSphere(temp.transform.position, overlapLimiter, LayerMask.GetMask("SL_Emitter_C")).Length > 0)
            {
                emitter_gameObjects_List_C.Remove(temp);
                temp.layer = 0;
            }
            else
            {
                grasshopperLoopInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envGrasshopperLoop);
                grasshopperLoopInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(temp));
                grasshopperLoopInstance.start();

                temp.layer = LayerMask.NameToLayer("SL_Emitter_C");
            }
        }
    }

    /*private void OnDrawGizmos() //Draw gizmos is updated per frame and always on
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            if (debug_SL_Emitters && AudioManager.instance.sceneNbr == 1 || AudioManager.instance.sceneNbr == 4)
            {
                foreach (GameObject gameObject in emitter_gameObjects_List_A)
                {
                    if (emitter_gameObjects_List_A.Count > 0)
                    {
                        Gizmos.DrawWireSphere(gameObject.transform.position, overlapLimiter);
                    }
                }
            }

            Gizmos.color = Color.yellow;
            if (debug_SL_Emitters && AudioManager.instance.sceneNbr == 1 || AudioManager.instance.sceneNbr == 4)
            {
                foreach (GameObject gameObject in emitter_gameObjects_List_B)
                {
                    if (emitter_gameObjects_List_B.Count > 0)
                    {
                        Gizmos.DrawWireSphere(gameObject.transform.position, overlapLimiter);
                    }
                }
            }

            Gizmos.color = Color.green;
            if (debug_SL_Emitters && AudioManager.instance.sceneNbr == 1 || AudioManager.instance.sceneNbr == 4)
            {
                foreach (GameObject gameObject in emitter_gameObjects_List_C)
                {
                    if (emitter_gameObjects_List_C.Count > 0)
                    {
                        Gizmos.DrawWireSphere(gameObject.transform.position, overlapLimiter);
                    }
                }
            }
        }
    }*/

    public void FMOD_Master_Reset_SL_Loop_Emitters()
    {
        if (debug_SL_Emitters) Debug.Log("SL loop master emitters reseted");

        if (!sl_Emitter_Init) return;
        FMOD_Reset_SL_Emitters_A();
        FMOD_Reset_SL_Emitters_B();
        FMOD_Reset_SL_Emitters_C();
    }

    public void FMOD_Reset_SL_Emitters_A()
    {
        if (emitter_gameObjects_List_A != null && emitter_gameObjects_List_A.Count > 0) emitter_gameObjects_List_A.Clear();

        cicadaLoopInstance.getDescription(out EventDescription cicadaEventDescription);
        if (!cicadaEventDescription.isValid()) return;
        cicadaEventDescription.getInstanceList(out EventInstance[] cicadaArray);
        if (cicadaArray.Length > 0)
        {
            foreach (EventInstance instance in cicadaArray)
            {
                instance.stop(STOP_MODE.ALLOWFADEOUT);
                instance.release();
                if (debug_SL_Emitters)
                    Debug.Log("cicada released");
            }
        }
    }

    public void FMOD_Reset_SL_Emitters_B()
    {
        if (emitter_gameObjects_List_B != null && emitter_gameObjects_List_B.Count > 0) emitter_gameObjects_List_B.Clear();

        cricketLoopInstance.getDescription(out EventDescription cricketEventDescription);
        if (!cricketEventDescription.isValid()) return;
        cricketEventDescription.getInstanceList(out EventInstance[] cricketArray);
        if (cricketArray.Length > 0)
        {
            foreach (EventInstance instance in cricketArray)
            {
                instance.stop(STOP_MODE.ALLOWFADEOUT);
                instance.release();
                if (debug_SL_Emitters)
                    Debug.Log("cricket released");
            }
        }
    }

    public void FMOD_Reset_SL_Emitters_C()
    {
        if (emitter_gameObjects_List_C != null && emitter_gameObjects_List_C.Count > 0) emitter_gameObjects_List_C.Clear();

        grasshopperLoopInstance.getDescription(out EventDescription grasshopperEventDescription);
        if (!grasshopperEventDescription.isValid()) return;
        grasshopperEventDescription.getInstanceList(out EventInstance[] grasshopperArray);
        if (grasshopperArray.Length > 0)
        {
            foreach (EventInstance instance in grasshopperArray)
            {
                instance.stop(STOP_MODE.ALLOWFADEOUT);
                instance.release();
                if (debug_SL_Emitters)
                    Debug.Log("grasshopper released");
            }
        }
    }

    public IEnumerator Wait_FMOD_SL_Instances_To_Reset_Reapply()
    {
        if (debug_SL_Emitters)
            Debug.Log("SL Loops wait for fmod instances stopping coroutine called ");

        if (!sl_Emitter_Init) yield break;

        int randomTime = Random.Range(1, 3);
        FMOD_Reset_SL_Emitters_A();
        yield return new WaitForSeconds(randomTime);

        randomTime = Random.Range(1, 3);
        FMOD_Reset_SL_Emitters_B();
        yield return new WaitForSeconds(randomTime);

        randomTime = Random.Range(1, 3);
        FMOD_Reset_SL_Emitters_C();
        yield return new WaitForSeconds(randomTime);

        checkLoopStates = true;
        yield return new WaitUntil(() => isEverythingStopped);
        yield return new WaitForSeconds(1);
        isEverythingStopped = false;
        checkLoopStates = false;

        yield return new WaitForSeconds(2);
        FMOD_SL_Count_Apply_Play();
    }
}
