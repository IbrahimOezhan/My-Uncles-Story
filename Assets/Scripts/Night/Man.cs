using Sirenix.OdinInspector;
using UnityEngine;

public class Man : MonoBehaviour
{
    private NightManager nightManager;
    private GameManager gameManager;
    private A_Man_Audio_States aStates;
    private Player_Interact playerInteract;
    private Transform player;
    private readonly Vector3[] offsets = { new(0, 0, 0), new(0, 1, 0), new(0, 0.5f, 0), new(0, 1.5f, 0), new(.5f, 0, 0), new(0, 0, .5f), new(0.5f, 0.5f, 0.5f), new(.5f, 1.5f, .5f), new(0.25f, 0.25f, 0.25f), new(0.25f, 0.25f, 0), new(0, 0.25f, 0.25f) };
    private Vector3 movePos;
    private float lookTimer;
    private float totalLookTimer;
    private float randomStareTime;
    private float randomChaseTimer;
    private float speedMultiplier = 1;
    private float walkAwayFromSafePointTimer;
    private int diff;
    private bool hasSeen;
    private bool chaseTriggered;

    [SerializeField] private GameObject deathcamera;
    [SerializeField] private float visibleDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float deathTimer;
    [SerializeField] private Transform behind;
    [SerializeField] private Renderer ManRenderer;
    [SerializeField] private LayerMask treeLayer;
    [SerializeField] private ManValues[] values;
    [SerializeField] private bool destroyOnTimeDistance;

    [SerializeField] private bool overWriteState;
    [ShowIf("overWriteState")] public State overwriteStateWith;

    [SerializeField] private bool showStates;
    [ShowIf("showStates")][ReadOnly] public State state;

    [HideInInspector] public Animator animator;
    [HideInInspector] public bool isInTree;

    private bool CanTriggerDeathByDist() { return Vector3.Distance(transform.position, player.position) > maxDistance && state != State.Chase_Death; }
    private bool CanTriggerPlayerDeath() { return (state == State.Chase_Base && hasSeen || state == State.Chase_Death) && DistanceToTarget() < 1f; }
    private bool CanTriggerDeathByTime() { return deathTimer <= 0.00f && state != State.Chase_Death; }
    private bool CanTriggerWaitAndWalk() { return Vector3.Distance(transform.position, gameManager.carLog.transform.position) < 10; }
    private bool CanTriggerChaseNoEscape() { return DistanceToTarget() < 2.5f && !(state == State.Chase_Base && hasSeen); }
    public float DistanceToTarget() { return Vector3.Distance(transform.position, movePos); }
    public bool CanSwitchState() { return !(state == State.Walk_Away || state == State.Chase_Death); }
    public bool IsBeingSeen(float distance = Mathf.Infinity) { return ManRenderer.isVisible && RayCastManager.instance.CastRays(transform, player, 8, offsets, distance); }

    private void Awake()
    {
        if (MainManager.instance.hardMode) diff = 1;

        gameManager = FindObjectOfType<GameManager>();
        aStates = GetComponent<A_Man_Audio_States>();
        animator = GetComponentInChildren<Animator>();
        player = gameManager.player;
        playerInteract = player.GetComponent<Player_Interact>();
        nightManager = gameManager.nightManagerRef;
        nightManager.man = this;

        movePos = player.position;
        randomStareTime = Random.Range(values[diff].randomTrxTime.x, values[diff].randomTrxTime.y);
        randomChaseTimer = Random.Range(values[diff].chaseLifeTimeRange.x, values[diff].chaseLifeTimeRange.y);

        if (A_RandomEmitters.rm != null) A_RandomEmitters.rm.man = gameObject;
        if (overWriteState) state = overwriteStateWith;

        Debug.Log("spawn dist" + Vector3.Distance(gameObject.transform.position, player.position));
    }

    private void Update()
    {
        BehaiviorExecution();
    }

    private void FixedUpdate()
    {
        if (ManRenderer.material.color.a < 1.00f)
        {
            float newAlpha = Mathf.Clamp01(ManRenderer.material.color.a + Time.deltaTime);
            ManRenderer.material.color = new Color(ManRenderer.material.color.r, ManRenderer.material.color.g, ManRenderer.material.color.b, newAlpha);
        }

        if (CanSwitchState())
        {
            if (CanTriggerWaitAndWalk()) Start_Wait_Walk_Away();
            if (CanTriggerChaseNoEscape()) Start_Chase_Death();

            if (IsBeingSeen(visibleDistance))
            {
                totalLookTimer += Time.deltaTime;
                lookTimer += Time.deltaTime;

                float chaseStartTime = values[diff].chaseLookTimeMultiplier * DistanceToTarget();
                Mathf.Clamp(chaseStartTime, values[diff].chaseTriggerTimeClamp.x, values[diff].chaseTriggerTimeClamp.y);
                if (lookTimer > chaseStartTime && !gameManager.PlayerInSafeZone()) hasSeen = true;
            }
            else lookTimer = 0;

            movePos = player.position;
        }
        if (CanTriggerPlayerDeath()) PlayerRestart();

        if (destroyOnTimeDistance)
        {
            if (!IsBeingSeen(visibleDistance))
            {
                if (CanTriggerDeathByTime() || CanTriggerDeathByDist()) GetRidOfMan(true, true);
                if (!hasSeen)
                {
                    if (gameManager.trip % 2 == 0 && player.position.z < transform.position.z - 7) GetRidOfMan(true, true);
                    else if (gameManager.trip % 2 != 0 && player.position.z > transform.position.z + 7) GetRidOfMan(true, true);
                }
            }

            deathTimer -= Time.deltaTime * (playerInteract.isInteracting ? 0 : 1);
        }

        Rotation();
    }

    private float NewSpawnTime(float maxTime, float currTime)
    {
        if (hasSeen)
        {
            if (state == State.Chase_Death) return maxTime;
            else if (chaseTriggered) return currTime * values[diff].SpawnTimeChaseEndMultiplier;
            else return currTime * values[diff].SpawnTimeSeenMultiplier;
        }
        else return currTime * values[diff].SpawnTimeNotSeenMultiplier;
    }

    private void PlayerRestart()
    {
        GetRidOfMan(false, false);
        aStates.A_Play_Death_SFX();
        MainManager.instance.Death(gameObject);
    }

    private void GetRidOfMan(bool spawnNewMan, bool destroy)
    {
        aStates.A_Chase_Reset();
        gameManager.playerInChase = false;

        float maxTime = values[diff].maxTimes[Mathf.Clamp(gameManager.trip - 8, 0, values[diff].maxTimes.Length - 1)];
        nightManager.waitTime = Mathf.RoundToInt(NewSpawnTime(maxTime, nightManager.waitTime));

        if (spawnNewMan) Instantiate(gameManager.manSpawner);

        Debug.Log("spawn destroy" + Vector3.Distance(gameObject.transform.position, player.position));
        GetComponentInChildren<A_Man_Movement>().Stop_Release_IdleAura();
        if (destroy) Destroy(gameObject);
    }

    private void BehaiviorExecution()
    {
        switch (state)
        {
            case State.Follow_Not_Look:
                Follow_Not_Look();
                break;
            case State.Chase_Base:
                Chase();
                break;
            case State.Chase_Death:
                Chase_Death();
                break;
            case State.Walk_Away:
                Walk_Away();
                break;
            case State.Walk_Away_Wait:
                Wait_Walk_Away();
                break;
        }
    }

    private void Start_Follow_Not_Look()
    {
        aStates.A_Chase_Reset();
        state = State.Follow_Not_Look;
        deathTimer = 15;
    }

    private void Follow_Not_Look()
    {
        if (hasSeen && !IsBeingSeen(visibleDistance))
        {
            float speed = Mathf.Clamp(DistanceToTarget() - 1, values[diff].speedPassive.x, values[diff].speedPassive.y);
            Movement(speed, "Walk");
        }
        else animator.Play("Idle");
        if (totalLookTimer > randomStareTime && !gameManager.PlayerInSafeZone() && !chaseTriggered) state = State.Chase_Base;
    }

    private void Start_Chase_Death()
    {
        aStates.A_Play_NoChase_Jumpscare();
        hasSeen = true;
        deathcamera.SetActive(true);
        state = State.Chase_Death;
    }

    private void Chase_Death()
    {
        gameManager.playerInChase = true;
        Movement(values[diff].speedChasingLook, "Run");
    }

    public void Start_Walk_Away()
    {
        aStates.A_Chase_Reset();
        destroyOnTimeDistance = true;
        deathTimer = 10;
        state = State.Walk_Away;
        aStates.allowManChaseVoxTimer = false;
        movePos = behind.position;
    }

    private void Walk_Away()
    {
        Movement(values[diff].speedWalkAway, "Walk");
        if (!IsBeingSeen(20)) deathTimer = 0;
    }

    private void Start_Wait_Walk_Away()
    {
        aStates.A_Chase_Reset();
        state = State.Walk_Away_Wait;
        deathTimer = 5;
    }

    private void Wait_Walk_Away()
    {
        animator.Play("Idle");
        walkAwayFromSafePointTimer += Time.deltaTime;
        if (walkAwayFromSafePointTimer > values[diff].waitBeforeWalkAwayTimer) Start_Walk_Away();
    }

    private void Start_Chase()
    {
        deathTimer = randomChaseTimer;
        chaseTriggered = true;
        aStates.A_Chase_Start();
        gameManager.playerInChase = true;
    }

    private void Chase()
    {
        if (!hasSeen) animator.Play("Idle");
        else
        {
            if (!chaseTriggered) Start_Chase();
            float speed;
            speed = IsBeingSeen() ? values[diff].speedChasingLook : Mathf.Clamp(DistanceToTarget() - 1, values[diff].speedChasingNotLook.x, values[diff].speedChasingNotLook.y);
            Movement(speed, "Run");
            if (aStates.CD_Man_DuringChase_Vox <= 1 && IsBeingSeen(visibleDistance)) aStates.A_Play_DuringChaseManVox_SFX();
        }
    }

    private void Movement(float speed, string anim)
    {
        animator.Play(anim);
        Vector3 movePosTemp = movePos;
        movePosTemp.y = .2f;
        speedMultiplier = playerInteract.isInteracting ? 0 : isInTree ? values[diff].speedTreeMultiplier : 1;
        transform.position = Vector3.MoveTowards(transform.position, movePosTemp, (speed * speedMultiplier) * Time.deltaTime);
        if (movePos == player.position) FMODUnity.RuntimeManager.StudioSystem.setParameterByName("DistToMan", DistanceToTarget());
    }

    private void Rotation()
    {
        var heading = movePos - transform.position;
        var heading2d = new Vector2(heading.x, heading.z).normalized;
        var angle = Mathf.Atan2(heading2d.y, heading2d.x) * -Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}

[System.Serializable]
public class ManValues
{
    [Header("SpawnTimes")]
    public float[] maxTimes;
    public float SpawnTimeChaseEndMultiplier;
    public float SpawnTimeNotSeenMultiplier;
    public float SpawnTimeSeenMultiplier;

    [Header("Speeds")]
    public float speedTreeMultiplier;
    public float speedWalkAway;
    public float speedChasingLook;
    public Vector2 speedChasingNotLook;
    public Vector2 speedPassive;

    [Header("Others")]
    public float waitBeforeWalkAwayTimer;
    public float chaseLookTimeMultiplier;

    public Vector2 chaseTriggerTimeClamp;
    public Vector2 randomTrxTime;
    public Vector2 chaseLifeTimeRange;
}

public enum State
{
    Follow_Not_Look,
    Chase_Base,
    Chase_Death,
    Walk_Away,
    Walk_Away_Wait,
    Nothing,
}