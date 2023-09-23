using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_Compass : MonoBehaviour
{
    private FMOD.Studio.EventInstance compassTurnInstance;
    private FirstPersonController fps;
    private GameManager gameManager;

    private float preRot;
    private float posRot;
    private float timer;
    private float rotateTimer;
    private float fadeSpeed = -0.02f;

    [SerializeField] private Transform compass, rotationToNorth;
    [SerializeField] private int offset;
    [SerializeField] private GameObject compassObject;
    [SerializeField] private Animator compassIcon;
    [SerializeField] private CanvasGroup group;

    public int compassHealth;
    public Text compassRepairText;

    [HideInInspector] public float brokenValue;
    [HideInInspector] public bool compassEnabled;
    [HideInInspector] public bool broken;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        fps = GetComponent<FirstPersonController>();
        preRot = fps.gameObject.transform.rotation.y;
        if (MainManager.instance.hardMode) compassHealth = 50;
    }

    private void FixedUpdate()
    {
        group.alpha += (fadeSpeed);

        if (compassEnabled)
        {
            posRot = fps.gameObject.transform.rotation.y;

            fps.MoveSpeed = 1.5f;

            if (!broken)
            {
                if (!gameManager.playerInChase)
                {
                    timer += Time.deltaTime;
                    if (timer > 2 && CanAddBrokenValue())
                    {
                        brokenValue += 1;
                        timer = 0;
                    }
                    if (brokenValue >= compassHealth) BrokenCompass();
                    compass.transform.localEulerAngles = new((rotationToNorth.localEulerAngles.y + offset) * (-1), 0, 0);
                }
                else
                {
                    rotateTimer += 0.02f;
                    if (rotateTimer == 0.03f)
                    {
                        rotateTimer = 0;
                        compass.transform.localEulerAngles = new(Random.rotation.x * -1, 0, 0);
                    }
                }
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("CompassTimer", brokenValue);

                if (posRot != preRot)
                {
                    compassTurnInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE compassState);
                    if (compassState != FMOD.Studio.PLAYBACK_STATE.STARTING && compassState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                    {
                        compassTurnInstance = FMODUnity.RuntimeManager.CreateInstance(AudioManager.instance.compassTurn);
                        compassTurnInstance.start();
                    }
                    preRot = posRot;
                }
                else StopCompassTurn();
            }
            else StopCompassTurn();
        }
        else
        {
            fps.MoveSpeed = 4.5f;
            StopCompassTurn();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !gameManager.noteOnHand.activeInHierarchy) Compass();
    }

    private void OnDisable()
    {
        StopCompassTurn();
    }

    public void Compass()
    {
        compassIcon.Play("Compass");
        compassObject.SetActive(!compassEnabled);
        compassEnabled = !compassEnabled;
        if (compassEnabled)
        {
            if (CanAddBrokenValue()) brokenValue += Random.Range(5, 10);
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_CompassOn");
        }
        else FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_CompassOff");
    }

    public void RepairCompass()
    {
        brokenValue = 0;
        broken = false;
        compassRepairText.enabled = false;
    }

    public IEnumerator RepairAnim(Interactable i)
    {
        fps.GetComponent<Player_Interact>().enabled = false;
        fps.canMove = false;
        fadeSpeed = 0.08f;
        yield return new WaitForSeconds(0.2f);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Compass_Fix");
        yield return new WaitForSeconds(1.4f);
        RepairCompass();
        gameManager.PopUpText("repaired");
        yield return new WaitForSeconds(1);
        fadeSpeed = -0.06f;
        fps.canMove = true;
        fps.GetComponent<Player_Interact>().enabled = true;
    }

    public void BrokenCompass()
    {
        broken = true;
        compassRepairText.enabled = true;
        gameManager.PopUpText("broken");
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Compass_Break");
        StopCompassTurn();
        brokenValue = 0;
    }

    private void StopCompassTurn()
    {
        compassTurnInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE compassState);
        if (compassState == FMOD.Studio.PLAYBACK_STATE.STARTING || compassState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            compassTurnInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            compassTurnInstance.release();
        }
    }

    private bool CanAddBrokenValue()
    {
        return !(gameManager.playerInChase || gameManager.PlayerInSafeZone());
    }
}
