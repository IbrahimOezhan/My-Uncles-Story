using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    private Interactable _i;
    private bool cooldownBool;
    [SerializeField] private float delay;
    public Text textDialog, dialogName;
    [HideInInspector] public FirstPersonController pc;
    public Text textDismiss;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pc == null) pc = FindObjectOfType<FirstPersonController>();
        StartCoroutine(Cooldown());
        if (_i)
        {
            _i.enabled = true;
            _i.SelectInteraction();
            _i = null;
        }
        if (pc != null) pc.canMove = true;
        textDialog.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !cooldownBool && _i != null && _i.dialog.skipable)
        {
            if (textDialog.text != TranslationManager.instance.GetText(_i.dialog.tag))
            {
                StartCoroutine(Cooldown());
                textDialog.GetComponent<DialogAnimation>().StopAllCoroutines();
                textDialog.text = TranslationManager.instance.GetText(_i.dialog.tag);
                textDismiss.enabled = true;
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Click_E");
                OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
                Invoke(nameof(PlayerCooldown), 1);
            }
        }
    }

    private void PlayerCooldown()
    {
        if (pc != null) pc.GetComponent<Player_Interact>().isInteracting = false;
    }

    private IEnumerator Cooldown()
    {
        cooldownBool = true;
        yield return new WaitForSeconds(.5f);
        cooldownBool = false;
    }

    public void StartDialog(Interactable i)
    {
        _i = i;
        if (pc == null) pc = FindObjectOfType<FirstPersonController>();
        if (pc != null) pc.canMove = false;

        textDismiss.enabled = false;
        textDialog.GetComponent<DialogAnimation>().i = _i;
        textDialog.GetComponent<DialogAnimation>().delay = delay;
        textDialog.text = TranslationManager.instance.GetText(_i.dialog.tag);
        if (_i.dialog.name != "") dialogName.text = _i.dialog.name;
        _i.enabled = false;

        textDialog.gameObject.SetActive(false);
        textDialog.gameObject.SetActive(true);
    }
}