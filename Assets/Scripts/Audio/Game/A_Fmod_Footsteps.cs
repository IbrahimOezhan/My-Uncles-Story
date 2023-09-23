using StarterAssets;
using UnityEngine;

public class A_Fmod_Footsteps : MonoBehaviour
{
    private enum current_terrain
    {
        Grass, Dirt, Wood
    }

    [SerializeField] private current_terrain currentTerrain;
    [SerializeField] private Transform feet;
    private FirstPersonController fps;
    private CharacterController characterController;
    private FMOD.Studio.EventInstance playerFSInstance;
    public float MinFootstepSpeed = 1.9f;
    public float FootstepsFrequencyFactor = 0;
    private float _timeBeforeNextStep = 0f;

    private void Awake()
    {
        fps = GetComponent<FirstPersonController>();
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        FMODSteps_Timer();
    }

    private void CheckTerrain()
    {
        foreach (RaycastHit rayhit in Physics.RaycastAll(transform.position, Vector3.down, 2f))
        {
            bool ret = false;
            switch (LayerMask.LayerToName(rayhit.transform.gameObject.layer))
            {
                case "Dirt":
                    currentTerrain = current_terrain.Dirt;
                    ret = true;
                    break;
                case "Wood":
                    currentTerrain = current_terrain.Wood;
                    ret = true;
                    break;
                case "Grass":
                    currentTerrain = current_terrain.Grass;
                    ret = true;
                    break;
            }
            if (ret) return;
        }
    }

    public void Check_Play_Player_Steps()
    {
        switch (currentTerrain)
        {
            case current_terrain.Grass:
                FMODSteps_Play(0);
                break;
            case current_terrain.Dirt:
                FMODSteps_Play(1);
                break;
            case current_terrain.Wood:
                FMODSteps_Play(2);
                break;
            default:
                FMODSteps_Play(0);
                break;
        }
    }

    private void FMODSteps_Timer()
    {
        var speed = characterController.velocity.magnitude;

        if (speed < MinFootstepSpeed) _timeBeforeNextStep = 0.1F / (MinFootstepSpeed * FootstepsFrequencyFactor);
        else
        {
            if (_timeBeforeNextStep <= 0f && fps.canMove)
            {
                CheckTerrain();
                Check_Play_Player_Steps();
                _timeBeforeNextStep = 1 / (speed * FootstepsFrequencyFactor);
            }
            _timeBeforeNextStep -= Time.deltaTime;
        }
    }

    private void FMODSteps_Play(int terrain)
    {
        if (AudioManager.instance.sceneNbr > 3) return;
        playerFSInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.playerFS);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Terrain", terrain);
        playerFSInstance.start();
        playerFSInstance.release();
    }
}
