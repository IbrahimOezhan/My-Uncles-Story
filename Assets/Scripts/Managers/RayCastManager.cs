using UnityEngine;

public class RayCastManager : MonoBehaviour
{

    public static RayCastManager instance;

    private void Start()
    {
        instance = this;
    }

    public bool CastRay(Transform originTransform, Transform directionTransform, int directionLayer, Vector3 offset, float lenght)
    {
        RaycastHit hit;
        Vector3 origin = originTransform.position + offset;
        Vector3 directionOr = directionTransform.position + new Vector3(0, 1.5f, 0);
        Vector3 direction = new(directionOr.x - origin.x, directionOr.y - origin.y, directionOr.z - origin.z);

        bool hitB = Physics.Raycast(origin, direction, out hit, lenght) && hit.transform.gameObject?.layer == directionLayer;

        if (hitB) Debug.DrawRay(origin, direction, Color.white);
        else Debug.DrawRay(origin, direction, Color.black);
        return hitB;
    }

    public bool CastRays(Transform originTransform, Transform directionTransform, int directionLayer, Vector3[] offsets, float lenght)
    {
        for (int i = 0; i < offsets.Length; i++) if (CastRay(originTransform, directionTransform, directionLayer, offsets[i], lenght)) return true;
        return false;
    }

    public Interactable CastRayInteractable(float lenght, Transform origin)
    {
        RaycastHit hit;
        Debug.DrawRay(origin.position, origin.TransformDirection(Vector3.forward), Color.cyan);
        if (Physics.Raycast(origin.position, origin.TransformDirection(Vector3.forward), out hit, lenght))
        {
            GameObject temp = hit.transform.gameObject;
            if (temp.GetComponent<Interactable>() && !temp.CompareTag("Interactable")) return temp.GetComponent<Interactable>();
        }
        return null;
    }
}
