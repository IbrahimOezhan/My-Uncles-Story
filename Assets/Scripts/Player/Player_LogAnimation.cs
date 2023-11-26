using UnityEngine;

public class Player_LogAnimation : MonoBehaviour
{
    private CharacterController characterController;
    private Player_Compass playerCompass;
    [SerializeField] private Animator log;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCompass = GetComponent<Player_Compass>();
    }

    private void FixedUpdate()
    {
        if (characterController.velocity.magnitude > 0.1 && !playerCompass.compassEnabled)
        {
            log.SetFloat("state", 1);
        }
        else log.SetFloat("state", 0);
    }
}
