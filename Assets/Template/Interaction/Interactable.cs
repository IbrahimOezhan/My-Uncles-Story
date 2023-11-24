using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public List<Trigger> triggersList = new();
    public Dialog dialog;
    [HideInInspector] public int state;
    private Player_Interact player_Interact;
    [SerializeField] private UnityEvent onInteract;

    [Button]
    public void Interact()
    {
        if (player_Interact == null) player_Interact = FindObjectOfType<Player_Interact>();
        if (player_Interact != null) player_Interact.isInteracting = true;
        state = 0;
        SelectInteraction();
    }

    public void SelectInteraction()
    {
        if (state == triggersList.Count)
        {
            if (player_Interact != null) Invoke(nameof(InteractionCooldown), 1);
            return;
        }
        bool doAgain = false;
        switch (triggersList[state])
        {
            case Trigger._event:
                IInteractable uICollision = GetComponent<IInteractable>();
                uICollision?.OnInteract(this);
                doAgain = true;
                break;
            case Trigger._dialogue:
                DialogManager.instance.StartDialog(this);
                break;
            case Trigger._uEvent:
                onInteract.Invoke();
                doAgain = true;
                break;
        }
        state += 1;

        if (doAgain)
        {
            SelectInteraction();
        }
    }

    public void ClearTriggerList(Trigger tr)
    {
        state = 0;
        triggersList.Clear();
        triggersList.Add(tr);
    }

    private void InteractionCooldown()
    {
        player_Interact.isInteracting = false;
    }
}

[System.Serializable]
public class Dialog
{
    public string name;
    public string tag;
    public bool skipable = true;
}

public enum Trigger
{
    _dialogue,
    _event,
    _uEvent
}