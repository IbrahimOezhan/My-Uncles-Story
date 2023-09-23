using UnityEngine;

public class A_Compass : MonoBehaviour
{
    [SerializeField] private Player_Compass player_compass;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("A_Compass_Letter") && player_compass.broken == false)
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Compass_Click");
    }
}
