using System.Collections;
using UnityEngine;

public class A_RandomEmitterObj : MonoBehaviour
{
    private GameObject player;
    private bool insideTree;
    private FMOD.Studio.EventInstance owlChirpInstance;
    private FMOD.Studio.EventInstance treeSticksInstance;

    private void Start()
    {
        player = FindObjectOfType<Player_Interact>().gameObject;
        StartCoroutine(A_Check_Col());
    }

    private void Update()
    {
        owlChirpInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE owlState);
        if (owlState == FMOD.Studio.PLAYBACK_STATE.STOPPING)
        {
            //Debug.Log("owl Stopping");
            A_RandomEmitters.rm.farEmittersList.Remove(transform);
            Destroy(gameObject);
        }

        treeSticksInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE sticksState);
        if (sticksState == FMOD.Studio.PLAYBACK_STATE.STOPPING)
        {
            //Debug.Log("sticks Stopping");
            A_RandomEmitters.rm.farEmittersList.Remove(transform);
            Destroy(gameObject);
        }
    }

    public void Check_Collisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);
        //Debug.Log("How many Colliders Start " + hitColliders.Length);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.layer == 15)
            {
                insideTree = true;
                break;
            }
        }

        if (insideTree == true)
        {
            //Debug.Log("FMOD PLAY");
            int randomEmitterChance = Random.Range(0, 10);
            A_RandomEmitters.rm.farEmittersList.Add(gameObject.transform);

            if (randomEmitterChance >= 3)
            {
                //Debug.Log("play owl chirp dynamic loop");
                owlChirpInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envOwlChirpLoop);
                owlChirpInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                owlChirpInstance.start();

                if (AudioManager.instance.gm.trip >= 9)
                {
                    Debug.Log("manTreeStepsCMD 3-10");
                    if (A_RandomEmitters.rm.man != null && !A_RandomEmitters.rm.man.GetComponent<Man>().IsBeingSeen()) return;
                    FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/manTreeSteps_Cmd", gameObject);
                }

                A_RandomEmitters.rm.forceSticks++;
            }
            else if (randomEmitterChance < 3 || A_RandomEmitters.rm.forceSticks == Random.Range(2, 6))
            {
                if (Vector3.Distance(transform.position, player.transform.position) >= 7 &&
                    A_RandomEmitters.rm.forceCloseSticks < Random.Range(2, 6))
                {
                    if (A_RandomEmitters.rm.man != null && !A_RandomEmitters.rm.man.GetComponent<Man>().IsBeingSeen()) return;
                    Debug.Log("sticks play far");
                    TreeStickInstance_Start();

                    int random = Random.Range(0, 10);
                    if (random >= 5)
                    {
                        if (A_RandomEmitters.rm.man != null) return;
                        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/manTreeSteps_Cmd", gameObject);
                    }

                    A_RandomEmitters.rm.forceSticks = 0;
                    A_RandomEmitters.rm.forceCloseSticks++;
                }
                else
                {
                    if (A_RandomEmitters.rm.man != null && !A_RandomEmitters.rm.man.GetComponent<Man>().IsBeingSeen()) return;
                    Debug.Log("sticks play close");

                    transform.position = player.transform.position + Random.insideUnitSphere * 10;
                    transform.position = new Vector3(transform.position.x, 5, transform.position.z);

                    TreeStickInstance_Start();

                    if (AudioManager.instance.gm.trip >= 10)
                    {
                        Debug.Log("manTreeStepsCMD close");
                        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/manTreeSteps_Cmd", gameObject);
                    }

                    //random master event that plays 2 scatterer. chance to trigger with other emitters

                    A_RandomEmitters.rm.forceCloseSticks = 0;
                    A_RandomEmitters.rm.forceSticks = 0;
                }
            }
        }

        if (insideTree == false)
        {
            hitColliders = null;
            if (hitColliders != null) Debug.Log("How many Colliders End " + hitColliders.Length);
            transform.position = player.transform.position + Random.insideUnitSphere * 20;
            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
            //Debug.Log("Rerolled");
        }
    }

    private void TreeStickInstance_Start()
    {
        treeSticksInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.envTreeSticks);
        treeSticksInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        treeSticksInstance.start();
    }

    private IEnumerator A_Check_Col()
    {
        while (insideTree == false)
        {
            yield return new WaitForSeconds(0.1f);
            Check_Collisions();
        }
    }
}
