using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public Transform rotateTo;
    [SerializeField] private float offset;

    private void Update()
    {
        var heading = rotateTo.position - transform.position;
        var heading2d = new Vector2(heading.x, heading.z).normalized;
        var angle = Mathf.Atan2(heading2d.y, heading2d.x) * -Mathf.Rad2Deg + offset;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}
