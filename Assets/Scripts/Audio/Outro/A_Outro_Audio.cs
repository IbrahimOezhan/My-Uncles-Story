using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class A_Outro_Audio : MonoBehaviour
{
    [SerializeField] private GameObject door;
    private GameObject player;
    [SerializeField] public List<GameObject> windowEmittersList = new();
    public bool allowTrigger = false;
    private bool allowTriggerFMOD = false;
    private float distToWindows;
    private float tempDist;

    public FMOD.Studio.EventInstance outroWindInstance;

    private class TimelineInfo
    {
        public int currentMusicBar = 0;
        public FMOD.StringWrapper lastMarker = new();
    }

    private TimelineInfo timelineInfo;
    private GCHandle timelineHandle;
    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    public FMOD.Studio.EventInstance doorKnockingInstance;

    private void Start()
    {
        player = FindObjectOfType<Player_Interact>().gameObject;

        allowTrigger = false;
        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

        doorKnockingInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envDoorKnocking);
        doorKnockingInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(door));

        doorKnockingInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        doorKnockingInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        if (windowEmittersList.Count > 0)
        {
            foreach (GameObject gameObject in windowEmittersList)
            {
                outroWindInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envOutroWind);
                outroWindInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                outroWindInstance.start();
                //Debug.Log("window wind start");
            }
        }
        distToWindows = 6;
        StartCoroutine(GetDistanceToWindows());
    }

    private void Update()
    {
        allowTriggerFMOD = (string)timelineInfo.lastMarker == "AllowScare";
        tempDist = Mathf.Clamp(tempDist, 4.5f, Mathf.Infinity);

        //Debug.Log("allowTriggerFMOD " + allowTriggerFMOD);
        //Debug.Log("allow Trigger " + allowTrigger);
    }

    private IEnumerator GetDistanceToWindows()
    {
    reTrigger:
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject windows in windowEmittersList)
        {
            tempDist = Vector3.Distance(player.transform.position, windows.transform.position);
            //Debug.Log(tempDist);
            if (tempDist < distToWindows)
                distToWindows = Vector3.Distance(player.transform.position, windows.transform.position);
        }
        //Debug.Log("dist to window" + distToWindows);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("DistanceToOutroWindows", distToWindows);
        goto reTrigger;
    }

    public IEnumerator Queue_Jumpscare()
    {
        yield return new WaitUntil(() => allowTrigger && allowTriggerFMOD);
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/Door_KnockScare", door);
        allowTrigger = false;
    }

    private void OnDestroy()
    {
        //Debug.Log("destroyed");
        doorKnockingInstance.setUserData(IntPtr.Zero);
        timelineHandle.Free();
        doorKnockingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        doorKnockingInstance.release();
        windowEmittersList.Clear();
    } //releases instances on door interactable

    private void OnGUI()
    {
        //GUILayout.Box(String.Format("Current Bar = {0}, Last Marker = {1}", timelineInfo.currentMusicBar, (string)timelineInfo.lastMarker));
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new(instancePtr);
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK) Debug.LogError("Timeline Callback error: " + result);
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = parameter.bar;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}
