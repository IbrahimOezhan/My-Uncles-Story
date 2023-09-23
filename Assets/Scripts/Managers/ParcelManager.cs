using System.Collections.Generic;
using UnityEngine;

public class ParcelManager : MonoBehaviour
{
    private GameManager gameManager;
    private A_Tree_Proximity treeProx;
    private Player_CarKeys playerKeys;
    private List<A_SplineEmitter> splineEmitters = new();

    [SerializeField] private ParcelSpawns[] spawns;
    [SerializeField] private Transform houseParcel;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        treeProx = FindObjectOfType<A_Tree_Proximity>();
        playerKeys = gameManager.player.GetComponent<Player_CarKeys>();
    }

    private void Start()
    {
        AddParcels(0);
        AddParcels(0);
        treeProx.A_TerrainTrees_ApplyCol();
    }

    public void IncreaseTrip()
    {
        gameManager.trip += 1;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TripModulator", gameManager.trip * 10);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Game_Trip", gameManager.trip);

        if (gameManager.isNight)
        {
            if ((gameManager.trip % 2) != 0)
            {
                gameManager.messageState++;
                AddParcels();
                if (MainManager.instance.hardMode) AddParcels();
                for (int o = 0; o < splineEmitters.Count; o++) splineEmitters[o].FMOD_ReTransform_Spline();
            }

            ChangeParcels();
            treeProx.A_TerrainTrees_Reset();
            treeProx.A_TerrainTrees_ApplyCol();
            StartCoroutine(A_SourceLoop_Controller.instance.Wait_FMOD_SL_Instances_To_Reset_Reapply());
        }

        if (gameManager.trip == gameManager.getNightAtTrip) OnLastDayTrip();
        else if (gameManager.trip != gameManager.getNightAtTrip + 1) ClearHouseEvent();
    }

    public void ChangeParcels()
    {
        if (spawns[1].parcels.Count < 4) return;
        for (int s = 0; s < spawns.Length; s++)
        {
            for (int i = 0; i < 20; i++)
            {
                int rdm1 = Random.Range(2, spawns[s].parcels.Count);
                int rdm2 = Random.Range(2, spawns[s].parcels.Count);
                while (rdm2 == rdm1) rdm2 = Random.Range(2, spawns[s].parcels.Count);
                Vector3 one = spawns[s].parcels[rdm1].transform.position;
                Vector3 two = spawns[s].parcels[rdm2].transform.position;
                spawns[s].parcels[rdm1].transform.position = two;
                spawns[s].parcels[rdm2].transform.position = one;
            }
        }
    }

    private void OnLastDayTrip()
    {
        gameManager.house.state = 0;
        gameManager.house.triggersList.Clear();
        gameManager.house.triggersList.Add(Trigger._event);
        gameManager.house.triggersList.Add(Trigger._dialogue);
        gameManager.house.triggersList.Add(Trigger._event);
    }

    private void ClearHouseEvent()
    {
        gameManager.house.state = 0;
        gameManager.house.triggersList.Clear();
        gameManager.house.triggersList.Add(Trigger._event);
    }

    public void AddParcels(int overrideRdm = -1)
    {
        for (int p = 0; p < spawns.Length; p++)
        {
            int rdm = 0;
            for (int i = 100; i > 0; i--) rdm = Random.Range(0, spawns[p].spawnableParcels.Length);
            if (overrideRdm != -1 && p == 1) rdm = overrideRdm;

            Vector3 pos = new(spawns[p].xSpawn, 0, 25 + (spawns[p].parcels.Count * 25));

            spawns[p].parcels.Add(Instantiate(spawns[p].spawnableParcels[rdm], pos, Quaternion.identity));
            spawns[p].parcels[^1].transform.parent = transform;

            A_SplineEmitter a_SplineEmitter = spawns[p].parcels[^1].GetComponentInChildren<A_SplineEmitter>();
            if (a_SplineEmitter != null) splineEmitters.Add(a_SplineEmitter);
        }
        houseParcel.transform.position += new Vector3(0, 0, 25);
    }
}

[System.Serializable]
public class ParcelSpawns
{
    public int xSpawn;
    public GameObject[] spawnableParcels;

    public List<GameObject> parcels = new();
}