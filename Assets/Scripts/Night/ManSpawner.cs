using UnityEngine;

public class ManSpawner : MonoBehaviour
{
    private GameManager gameManager;
    private NightManager nightManager;
    private Player_Interact Player_Interact;

    private float time;
    private float timer;

    [SerializeField] private Vector2 spawnRadius;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private float treeCheckRadius;

    private void Awake()
    {
        if (FindObjectsOfType<ManSpawner>().Length > 1) Destroy(gameObject);
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null) Destroy(gameObject);
        else
        {
            if (!gameManager.gameObject.activeInHierarchy) Destroy(gameObject);
            nightManager = gameManager.nightManagerRef;
            time = gameManager.nightManagerRef.waitTime;
            Player_Interact = gameManager.player.GetComponent<Player_Interact>();
        }
    }

    private void FixedUpdate()
    {
        if (!Player_Interact.isInteracting && Player_Interact.transform.position.z < gameManager.manMaxPos.position.z && Player_Interact.transform.position.z > 25)
        {
            timer += 0.02f;
            if (timer > time)
            {
                Man man = Instantiate(Prefab, SpawnPos(), Quaternion.identity).GetComponent<Man>();
                if (nightManager.ratioRandom.Count <= 0) nightManager.GenerateNewList();
                man.state = (State)nightManager.ratioRandom[0];
                nightManager.ratioRandom.RemoveAt(0);
                Destroy(gameObject);
            }
        }
    }

    private Vector3 SpawnPos()
    {
        Vector3 playerPos = gameManager.player.position;
        Vector3 pos = new(Mathf.Clamp(playerPos.x + Random.Range(spawnRadius.x, -spawnRadius.x), -45, 120), 2, Mathf.Clamp(playerPos.z + (((gameManager.trip % 2) != 0) ? spawnRadius.y : -spawnRadius.y), 25, gameManager.manMaxPos.position.z));
        for (int i = 0; Physics.CheckSphere(pos, treeCheckRadius, 15) && i < 50; i++) pos.x = Mathf.Clamp(playerPos.x + Random.Range(spawnRadius.x, -spawnRadius.x), -45, 120);
        pos.y = 0;
        return pos;
    }
}
