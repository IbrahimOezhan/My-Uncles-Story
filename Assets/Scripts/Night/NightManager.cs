using FMODUnity;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightManager : MonoBehaviour
{
    private GameManager gameManager;
    private FirstPersonController firstPersonController;
    private List<byte> ratioCopy = new();

    [SerializeField] private Material nightbox;
    [SerializeField] private GameObject blackscreen;
    [SerializeField] private Color color;
    [SerializeField] private float nightIntensity;
    [SerializeField] private Font steveFont;
    [SerializeField] private List<byte> ratio = new();

    public int waitTime;

    [HideInInspector] public List<byte> ratioRandom = new();
    [HideInInspector] public Man man;


    private void Awake()
    {
        PlayerPrefs.SetInt("reachedNight", 1);
        gameManager = FindObjectOfType<GameManager>();
        firstPersonController = gameManager.player.GetComponent<FirstPersonController>();
    }

    public IEnumerator Start()
    {
        gameManager.isNight = true;
        gameManager.messageState++;
        firstPersonController.canMove = false;
        gameManager.ChangeObjective("objectiveRemain");

        gameManager.noteTrans.GetComponent<Text>().font = steveFont;
        gameManager.noteTrans.GetComponent<Text>().fontSize = 11;

        gameManager.carLog.gameObject.SetActive(true);
        gameManager.noteOnCar.SetActive(true);
        gameManager.carDoor.gameObject.SetActive(false);
        gameManager.duck.SetActive(true);

        RuntimeManager.PlayOneShot("event:/Events/TRX_Night");
        AudioManager.instance.dayAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        AudioManager.instance.dayAmbienceInstance.release();

        Destroy(blackscreen, 5);
        VisualChanges();
        yield return new WaitForSeconds(5);

        RuntimeManager.PlayOneShot("event:/Events/TRX_Night_2");
        AudioManager.instance.FMOD_InGame_Night_State();

        firstPersonController.canMove = true;
        if (MainManager.instance.hardMode) Instantiate(gameManager.manSpawner);
    }

    private void VisualChanges()
    {
        gameManager.hand1.intensity = 0.01f;
        gameManager.hand2.intensity = 0.01f;
        Light[] lights = FindObjectsOfType<Light>();
        for (int i = 0; i < lights.Length; i++) if (lights[i].type == LightType.Directional) lights[i].intensity = nightIntensity;
        RenderSettings.fogDensity = 0.1f;
        RenderSettings.fogColor = color;
        RenderSettings.skybox = nightbox;
    }

    public void GenerateNewList()
    {
        ratioCopy.Clear();
        for (int i = 0; i < ratio.Count; i++) ratioCopy.Add(ratio[i]);
        for (int i = 0; i < 10; i++)
        {
            int rdm = Random.Range(0, ratioCopy.Count);
            ratioRandom.Add(ratioCopy[rdm]);
            ratioCopy.RemoveAt(rdm);
        }
    }
}
