using UnityEngine;
using UnityEngine.SceneManagement;

public class LogManager : MonoBehaviour
{
    private GameManager gameManager;
    private ParcelManager parcelManager;
    private float fmod_logs = 0;
    private FMOD.Studio.EventInstance playerLogDropinstance;
    private FMOD.Studio.EventInstance playerLogPUinstance;
    [SerializeField] private GameObject fadeOut;
    [SerializeField] private Transform logContainerCar;
    [SerializeField] private GameObject a_Source;
    [SerializeField] private GameObject a_Source_drop;
    [SerializeField] private Transform logContainerHouse;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        parcelManager = FindObjectOfType<ParcelManager>();
    }

    public void DropLog()
    {
        gameManager.logOnHand.SetActive(false);
        parcelManager.IncreaseTrip();
        EnableLogOnGround();
        A_LogDropSound();
        NoMoreLogsCheck();
    }

    public void PickUpLog(Interactable i)
    {
        A_LogPickUpSound();
        DisableLogOnCar();
        gameManager.ChangeObjective((gameManager.isNight) ? "objectiveRemain" : "objectiveHouse");
        if (gameManager.trip == 6 && !MainManager.instance.hardMode) gameManager.manWalkAway.gameObject.SetActive(true);
        parcelManager.IncreaseTrip();
        gameManager.logOnHand.SetActive(true);
        gameManager.house.gameObject.SetActive(true);
        i.enabled = true;
        gameManager.carLog.gameObject.SetActive(false);
    }

    private void A_LogDropSound()
    {
        fmod_logs++;
        playerLogDropinstance = FMODUnity.RuntimeManager.CreateInstance("event:/Player/playerLogDrop");
        playerLogDropinstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(a_Source_drop.transform.position));
        playerLogDropinstance.setParameterByName("HowManyLogs", fmod_logs);
        playerLogDropinstance.start();
        playerLogDropinstance.release();
    }

    private void A_LogPickUpSound()
    {
        playerLogPUinstance = FMODUnity.RuntimeManager.CreateInstance("event:/Player/playerLogPU");
        playerLogPUinstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(a_Source.transform.position));
        playerLogPUinstance.start();
        playerLogPUinstance.release();
    }

    public void EnableLogOnGround()
    {
        foreach (Transform child in logContainerHouse)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void DisableLogOnCar()
    {
        Destroy(logContainerCar.GetChild(0).gameObject);
    }

    private void NoMoreLogsCheck()
    {
        int numberOfNonActiveChilds = 0;
        foreach (Transform child in logContainerHouse) if (!child.gameObject.activeInHierarchy) numberOfNonActiveChilds++;
        if (numberOfNonActiveChilds == logContainerHouse.childCount - 1)
            AudioManager.instance.ss_ToOutroTRXInstance = FMODUnity.RuntimeManager.CreateInstance("snapshot:/SS_HouseBuild");
        if (numberOfNonActiveChilds == 0)
        {
            Instantiate(fadeOut);
            AudioManager.instance.ss_ToOutroTRXInstance.start(); //Stopped at FMOD CMD House Build Event
            Invoke(nameof(ChangeSceen), 1);
        }
    }

    private void ChangeSceen()
    {
        string time = MainManager.instance.hardMode ? "timeTotalHard" : "timeTotal";
        if (gameManager.speedrunTimer > PlayerPrefs.GetFloat(time)) PlayerPrefs.SetFloat(time, gameManager.speedrunTimer);
        SceneManager.LoadScene(2);
    }
}
