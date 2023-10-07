using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private FirstPersonController fps;
    private StarterAssetsInputs sai;
    private Player_Compass playerCompass;
    private Player_Interact player_Interact;
    private LogManager logManager;
    private Interactable note;

    private string lastMessage;
    private bool once = false;
    private int restarts = 0;
    private float noMoveTimer;

    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private Interactable restartDialog;
    [SerializeField] private Interactable firstDialog;
    [SerializeField] private Interactable dialogTooFar;
    [SerializeField] private GameObject popText;

    public GameObject toolsUI;
    public GameObject logOnHand;
    public GameObject noteOnCar;
    public GameObject noteOnHand;
    public GameObject nightManager;
    public GameObject manSpawner;
    public GameObject duck;
    public Transform player;
    public Transform manMaxPos;
    public Man manWalkAway;
    public Text objectiveText;
    public Translater noteTrans;
    public Interactable carLog;
    public Interactable house;
    public Interactable carDoor;
    public int getNightAtTrip;

    [HideInInspector] public NightManager nightManagerRef;
    [HideInInspector] public bool isUsingKeys;
    [HideInInspector] public bool isNight;
    [HideInInspector] public bool playerInChase;
    [HideInInspector] public int trip;
    [HideInInspector] public int messageState = 0;
    [HideInInspector] public float speedrunTimer;

    private void Awake()
    {
        logManager = FindObjectOfType<LogManager>();
        fps = player.GetComponent<FirstPersonController>();
        sai = player.GetComponent<StarterAssetsInputs>();
        playerCompass = fps.GetComponent<Player_Compass>();
        player_Interact = fps.GetComponent<Player_Interact>();

        AudioManager.instance.gm = this;
        AudioManager.instance.player = player.gameObject;
        AudioManager.instance.car = GameObject.FindGameObjectWithTag("A_Car");
        AudioManager.instance.firstPersonController = fps;
    }

    private void Start()
    {
        if (MainManager.instance.skipDay) StartCoroutine(SkipDay());
        else firstDialog.Interact();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && noteOnHand.activeInHierarchy) DisableNote();
        if (Input.GetKeyDown(KeyCode.N) && fps.canMove && lastMessage != null) DisplayNote(null, lastMessage);
    }

    private void FixedUpdate()
    {
        if ((sai.move == Vector2.zero || !fps.canMove || !isNight || playerCompass.compassRepairText.enabled || isUsingKeys) && !player_Interact.isInteracting)
        {
            noMoveTimer += Time.deltaTime;
            if (noMoveTimer > 1f || isUsingKeys) canvas.alpha += Time.deltaTime * (isUsingKeys ? 4 : 2);
        }
        else
        {
            noMoveTimer = 0;
            canvas.alpha -= Time.deltaTime * 2;
        }

        if (!once && trip == 0 && Vector3.Distance(player.position, carLog.transform.position) > 20)
        {
            once = true;
            dialogTooFar.Interact();
        }

        if (isNight) speedrunTimer += 0.02f;
    }

    public void OnRestart()
    {
        AudioManager.instance.FMOD_InGame_Night_State();
        Instantiate(manSpawner);
        playerCompass.RepairCompass();
        if (restarts == 0) restartDialog.Interact();
        else fps.canMove = true;
        restarts++;
    }

    private IEnumerator SkipDay()
    {
        MainManager.instance.skipDay = false;
        nightManagerRef = Instantiate(nightManager).GetComponent<NightManager>();
        logManager.DisableLogOnCar();
        yield return new WaitForSeconds(0.1f);
        logManager.EnableLogOnGround();
        yield return new WaitForSeconds(0.1f);
        logManager.DisableLogOnCar();
        yield return new WaitForSeconds(0.1f);
        logManager.EnableLogOnGround();
        trip = 4;
        toolsUI.SetActive(true);
    }

    public void ChangeObjective(string tag)
    {
        objectiveText.text = TranslationManager.instance.GetText(tag);
    }

    public void DisplayNote(Interactable i = null, string overwriteMsg = null)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_LetterOpen");

        if (i != null)
        {
            note = i;
            noteOnCar.SetActive(false);
            i.enabled = false;
        }

        fps.canMove = false;
        if (playerCompass.compassEnabled) playerCompass.Compass();
        noteTrans.tagName = overwriteMsg != null ? overwriteMsg : lastMessage = "message" + (MainManager.instance.hardMode ? "Hard" : "") + messageState;
        noteOnHand.SetActive(true);
        noteTrans.gameObject.SetActive(true);
        TranslationManager.instance.UpdateLanguage();
    }

    public void DisableNote()
    {
        fps.canMove = true;
        player_Interact.isInteracting = false;

        if (note != null)
        {
            logManager.PickUpLog(carLog);
            note = null;
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_LetterClose");

        toolsUI.SetActive(trip > 0);
        noteOnHand.SetActive(false);
        noteTrans.gameObject.SetActive(false);

    }

    public void PopUpText(string _tag)
    {
        Instantiate(popText).GetComponentInChildren<Translater>().tagName = _tag;
    }

    public bool PlayerInSafeZone()
    {
        return Vector3.Distance(player.position, carLog.transform.position) < 12;
    }
}