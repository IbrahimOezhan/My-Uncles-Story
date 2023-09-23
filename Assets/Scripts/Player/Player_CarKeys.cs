using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_CarKeys : MonoBehaviour
{
    private float charge = 5;
    private bool debug = false;
    private GameManager gameManager;

    [SerializeField] private float defaultCharge;
    [SerializeField] private Image keyCharge;
    [SerializeField] private Animator keyAnim;
    [SerializeField] private Animator carAnim;
    [SerializeField] private GameObject carHorn;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        charge = defaultCharge;
    }

    private void Update()
    {
        if (keyCharge.fillAmount > charge / defaultCharge) keyCharge.fillAmount -= Time.deltaTime;
        RuntimeManager.StudioSystem.setParameterByName("ManDirDynamicReverb_Car", DirectionToCar());

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayCar();

            if (debug)
            {
                Debug.Log("Emitter Distance to Player " + Mathf.Ceil(Vector3.Distance(transform.position, carHorn.transform.position) * 100) / 100);
                Debug.Log("Player Angle Towards Emitter " + DirectionToCar());
            }
        }
    }

    private IEnumerator CooldownEnu()
    {
        gameManager.isUsingKeys = true;
        yield return new WaitForSeconds(5);
        gameManager.isUsingKeys = false;
    }

    public void PlayCar()
    {
        if (charge > 0)
        {
            if (!gameManager.isUsingKeys && gameManager.trip != 0)
            {
                StartCoroutine(CooldownEnu());

                if (gameManager.isNight)
                {
                    Man man = gameManager.nightManagerRef.man;
                    if (man != null) man.Start_Walk_Away();
                }

                keyAnim.Play("Key");
                carAnim.Play("Light_Blink");

                RuntimeManager.PlayOneShotAttached("event:/Player/CarHorns/playerCarHorn" + gameManager.trip.ToString(), carHorn);

                charge--;
                return;
            }
        }
        else gameManager.PopUpText("carKeys");
    }

    private int DirectionToCar()
    {
        var camForwardVector = new Vector3(transform.forward.x, 0f, transform.forward.z);
        float angle = Vector3.SignedAngle((carHorn.transform.position - transform.position).normalized, camForwardVector, Vector3.up);
        int angleInt = (int)Mathf.Round(angle);
        return angleInt;
    }

    //Distance mapping:
    //trips 0-4 - 2 parcels - 110
    //trip 5-6 - 3 parcels - 140
    //trip 7-8 - 160
    //trip 9-10 - 190
    //trip 11-12 - 210
    //trip 13-14 - 240
    //trip 15-16 - 270
    //trip 17 - 300
}
