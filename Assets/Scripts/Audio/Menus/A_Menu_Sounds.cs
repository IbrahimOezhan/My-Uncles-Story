using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class A_Menu_Sounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool allowClick = false;
    private bool doOnce = true;

    [SerializeField] private CanvasGroup group;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (pointerEventData.pointerEnter.gameObject.GetComponent<Button>() != null && group.interactable)
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Hover");
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (pointerEventData.pointerEnter.gameObject.GetComponent<Button>() != null && group.interactable)
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Hover_Off");
    }

    public void FMOD_Play_UI_ClickOn()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Click_On");
        if (doOnce)
        {
            allowClick = true;
            doOnce = false;
        }
    }
    public void FMOD_Play_UI_ClickBack()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Click_Off");
    }
    public void FMOD_Play_UI_ClickSlider()
    {
        if (allowClick)
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Compass_Click");
    }
}
