using UnityEngine;

public class A_Wind_Target_Col : MonoBehaviour
{
    public bool startParameter;
    public bool allowSplineFollow;

    private void Start()
    {
        startParameter = false;
        allowSplineFollow = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startParameter = true;
            allowSplineFollow = true;
            //Debug.Log("true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startParameter = false;
            //Debug.Log("false");
        }
    }
}
