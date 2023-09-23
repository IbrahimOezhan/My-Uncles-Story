using UnityEngine;

public class Intro_Parcel : MonoBehaviour
{
    private Vector3 moveTo;
    [SerializeField] private float speed;

    private void Start()
    {
        moveTo = new Vector3(25, 0, 0);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveTo, speed);
        if (transform.position.x >= moveTo.x) transform.position = new Vector3(-25, 0, 0);
    }
}
