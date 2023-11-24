using UnityEngine;

public class Player_Interact : MonoBehaviour
{
    [SerializeField] private GameObject textInteract, cameras;
    [HideInInspector] public bool isInteracting;

    private void Update()
    {
        Interactable i = RayCastManager.instance.CastRayInteractable(2.5f, cameras.transform);
        Collider[] coll = Physics.OverlapSphere(transform.position, .7f, 1 << 19);
        if (coll.Length > 0) i = coll[0].gameObject.GetComponent<Interactable>();
        if (i != null && i.enabled && i.gameObject.activeInHierarchy && !isInteracting)
        {
            textInteract.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) i.Interact();
        }
        else textInteract.SetActive(false);
    }
}