using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    private GameObject gameIn, restartIn;
    private GameManager gameManager;
    private ParcelManager parcelManager;

    private bool allowSFX = true;
    private float closeTimer;

    [SerializeField] private GameObject restart;
    [SerializeField] private GameObject game;
    [SerializeField] private GameObject fadeOut;
    [SerializeField] private Text closeText;

    public int whichEnding;
    public bool skipDay;
    public bool hardMode;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else
        {
            SceneManager.sceneLoaded += OnScene;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnScene;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            closeTimer += Time.deltaTime;
            closeText.enabled = true;

            if (closeTimer > 1)
            {
                if (allowSFX)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Pause_Riser");
                    allowSFX = false;
                }

                if (closeTimer >= 4)
                {
                    allowSFX = false;
                    SceneManager.LoadScene(0);
                }
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Pause_Riser_Stop");
            allowSFX = true;
            closeTimer = 0;
            closeText.enabled = false;
        }
    }

    private void OnScene(Scene scene, LoadSceneMode mode)
    {
        Instantiate(fadeOut);
        switch (scene.name)
        {
            case "Menu":
                PlayerPrefs.SetInt("hasDuck", 0);
                PlayerPrefs.SetInt("hasKnife", 0);
                skipDay = false;
                hardMode = false;
                break;
            case "Game":
                gameIn = Instantiate(game);
                parcelManager = FindAnyObjectByType<ParcelManager>();
                gameManager = gameIn.GetComponent<GameManager>();
                break;
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("hasDuck", 0);
        PlayerPrefs.SetInt("hasKnife", 0);
    }

    public void Death(GameObject man)
    {
        if (hardMode)
        {
            skipDay = true;
            SceneManager.LoadScene(1);
        }
        else
        {
            AudioManager.instance.FMOD_InGame_Dead_State(); //resets chase music state to off - 0 
            parcelManager.ChangeParcels();

            gameManager.player.GetComponent<FirstPersonController>().canMove = false;
            gameManager.player.GetComponent<StarterAssetsInputs>().move = new(0, 0);
            gameManager.player.GetComponent<StarterAssetsInputs>().look = new(0, 0);

            restartIn = Instantiate(restart);
            Destroy(man);
            gameIn.SetActive(false);

            gameManager.player.GetComponent<FirstPersonController>().canMove = false;
            gameManager.player.GetComponent<StarterAssetsInputs>().move = new(0, 0);
            gameManager.player.GetComponent<StarterAssetsInputs>().look = new(0, 0);

            Vector3 newPos = ((gameManager.trip % 2) == 0) ? gameManager.house.transform.position : gameManager.carDoor.transform.position;
            newPos.y = 0.08999985f;
            gameManager.player.position = newPos;
        }
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("hasDied", 1);
        StartCoroutine(RestartIE());
    }

    private IEnumerator RestartIE()
    {
        AudioManager.instance.ss_RestartDroneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        AudioManager.instance.ss_RestartDroneInstance.release();
        Destroy(restartIn, 0.5f);
        yield return new WaitForSeconds(.5f);
        gameIn.SetActive(true);
        gameManager.OnRestart();
    }
}
