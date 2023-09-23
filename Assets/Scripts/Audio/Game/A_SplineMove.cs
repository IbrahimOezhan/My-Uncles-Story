using UnityEngine;

public class A_SplineMove : MonoBehaviour
{
    private Transform followPlayerPos;
    private GameManager gm;
    private bool instanceActive = false;
    private A_Wind_Target_Col windCol;

    [SerializeField] private A_SplineEmitter splineEmitter;
    [SerializeField] private GameObject windTarget;
    [SerializeField] private bool introParcel = false;

    public FMOD.Studio.EventInstance treeWindBrushInstance;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        windCol = FindObjectOfType<A_Wind_Target_Col>();
        followPlayerPos = gm.player;
    }

    private void FixedUpdate()
    {
        if (followPlayerPos != null && windCol.allowSplineFollow)
        {
            transform.position = splineEmitter.WhereOnSpline(followPlayerPos.position);

            if (introParcel && windCol.startParameter)
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Wind_Distance", Vector3.Distance(windTarget.transform.position, followPlayerPos.position));
                //Debug.Log("bool" + windCol.startParameter);
                //Debug.Log(Vector3.Distance(transform.position, followPlayerPos.position)); 
            }
            //if (windCol != null)Debug.Log(windCol.startParameter);
        }

        if (instanceActive && !introParcel)
        {
            treeWindBrushInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            //Debug.Log(Vector3.Distance(followPlayerPos.position, transform.position));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gm.isNight && !introParcel)
        {
            treeWindBrushInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envTreeWindBrushLoop);
            treeWindBrushInstance.start();
            instanceActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && gm.isNight && !introParcel)
        {
            treeWindBrushInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            treeWindBrushInstance.release();
            instanceActive = false;
        }
    }
}
