using System.Collections.Generic;
using UnityEngine;

public class A_Tree_Proximity : MonoBehaviour
{
    private CharacterController characterController;
    private List<TreeInstance> treeInstancesList = new();
    private List<GameObject> emptyTreeColList = new(); //counts empty tree collider objs to destroy 
    private List<GameObject> closestTreeObjectsList = new();

    private GameObject closestTreeEmpty;
    private GameObject treeColGameObj;
    private Vector3 lastTriggerPosition;
    private Vector3 closestTreeTransf;
    private float distWalkedFromLastTrigger;
    private float distMinToReTrigger;
    private bool doOnce = true;

    [SerializeField] private float distanceToClosestTree;

    private void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        distWalkedFromLastTrigger = 0f;
        lastTriggerPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (closestTreeEmpty != null)
        {
            distanceToClosestTree = Mathf.Round(Vector3.Distance(transform.position, closestTreeTransf) * 100f) / 100f;
            A_Play_Soft_Tree_Foliage();
            distWalkedFromLastTrigger = Vector3.Distance(transform.position, lastTriggerPosition);
            if (characterController.velocity != Vector3.zero) A_Play_Hard_Tree_Foliage();
        }

        //Debug.DrawRay(this.transform.position, closestTreeEmpty.transform.position - this.transform.position, Color.blue);
        //Debug.DrawRay(this.transform.position, closestTreeTransf - this.transform.position, Color.red);
    }

    public void A_TerrainTrees_ApplyCol()
    {
        foreach (Terrain terrain in FindObjectsOfType<Terrain>())
        {
            if (terrain.CompareTag("A_Terrain"))
            {
                TerrainData terrainData = terrain.terrainData;

                foreach (TreeInstance treeInstance in terrainData.treeInstances)
                {
                    treeInstancesList.Add(treeInstance);
                    Vector3 position = Vector3.Scale(treeInstance.position, terrainData.size) + terrain.transform.position;
                    position.y = terrain.SampleHeight(position);

                    treeColGameObj = new GameObject
                    {
                        tag = "A_TreeCol",
                        name = "A_TreeColGameObject",
                        layer = 15
                    };
                    treeColGameObj.transform.position = position;
                    treeColGameObj.transform.parent = terrain.transform;

                    CapsuleCollider capCol = treeColGameObj.AddComponent<CapsuleCollider>();
                    capCol.isTrigger = true;
                    capCol.center = new Vector3(0, 2, 0);
                    capCol.radius = 1.5f;
                    capCol.height = 6.8f;

                    emptyTreeColList.Add(treeColGameObj);

                    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //cube.transform.position = position;
                    //cube.transform.SetParent(TreeObjectsParent.transform);
                }
            }
        }
        //Debug.Log("Counted and added Tree Cols");
        //Debug.Log("how many tree atm" + treeInstancesList.Count);
    }

    public void A_TerrainTrees_Reset()
    {
        foreach (GameObject empty in emptyTreeColList) if (empty != null) Destroy(empty);
        emptyTreeColList.Clear();
        treeInstancesList.Clear();
        closestTreeObjectsList.Clear();
        //Debug.Log("Terrain Trees Lists Cleared and destroyed");
    }

    private void A_Play_Soft_Tree_Foliage()
    {
        if (distanceToClosestTree <= 2.4f && doOnce == true)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Environment/Tree_Collision", closestTreeEmpty);
            doOnce = false;
        }
        else if (distanceToClosestTree > 2.4f) doOnce = true;
    }

    private void A_Play_Hard_Tree_Foliage()
    {
        if (distanceToClosestTree <= 2.7f && A_RandomEmitters.rm != null && AudioManager.instance.gm.trip >= 8) //delay tree scares
        {
            if (distWalkedFromLastTrigger > 10)
            {
                if (distWalkedFromLastTrigger >= distMinToReTrigger || A_RandomEmitters.rm.allowCloseEmitter == true)
                {
                    if (characterController.velocity != Vector3.zero)
                    {
                        Debug.Log("Close Tree Emitter Play");
                        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Events/Tree_Foliage", closestTreeEmpty);

                        lastTriggerPosition = transform.position; //reset trigger position to the closest object
                        distMinToReTrigger = Random.Range(150f, 300f); //sets a new random distance to retrigger close trigger original 200 300
                        distWalkedFromLastTrigger = 0f; //reset distance walked to the next trigger

                        A_RandomEmitters.rm.CDTimerCloseEmitter = Random.Range(150f, 300f);
                        A_RandomEmitters.rm.allowCloseEmitter = false;
                    }

                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("A_TreeCol")) closestTreeObjectsList.Add(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("A_TreeCol"))
        {
            //Debug.Log(closestTreeObjectsList.Count);
            if (closestTreeObjectsList.Count > 0)
            {
                Collider closestCollider = closestTreeObjectsList[0].GetComponent<Collider>();
                Vector3 closestPointB = closestCollider.ClosestPointOnBounds(transform.position);
                float distanceB = Vector3.Distance(closestPointB, transform.position);

                foreach (GameObject gameObject in closestTreeObjectsList)
                {
                    Vector3 closestPointA = gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    float distanceA = Vector3.Distance(closestPointA, transform.position);

                    if (distanceA < distanceB)
                    {
                        closestCollider = gameObject.GetComponent<Collider>();
                        distanceB = distanceA;
                        closestTreeEmpty = gameObject;
                    }
                    //Vector3 closestPoint = closestCollider.ClosestPointOnBounds(this.transform.position);
                }
                closestTreeTransf = closestCollider.GetComponentInParent<Transform>().transform.position;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("A_TreeCol")) closestTreeObjectsList.Remove(other.gameObject);
    }

    /*
    //public Terrain currentTerrain;
    //public TerrainData currentTerrainData;
    //public TerrainCollider currentTerrainDataCollider;
     *     private Terrain currentTerrain;
    private int[,] map;
     * public void A_ReOrder_TerrainTrees() //pointless this way, find more optimized way or leave it to reset everything and reapply
    {
        foreach (Terrain terrain in currentTerrainsList)
        {
            TerrainData terrainData = terrain.terrainData;
            Debug.Log("foreach terrain in re order");
            foreach (TreeInstance treeInstance in terrainData.treeInstances)
            {
                Vector3 newTreePos = Vector3.Scale(treeInstance.position, terrainData.size) + terrain.transform.position;
                treeColGameObj.transform.position = newTreePos;
                Debug.Log("moved tree col");
            }
        }
        //Debug.Log("Terrain Trees List position updated");
    }
    private void A_Get_Grass()
    {
        currentTerrain = FindObjectOfType<Terrain>();
        Vector3 worldPos = transform.position;
        Vector3 terrainLocalPos = worldPos - currentTerrain.transform.position;
        Vector2 normalizedPos = new(Mathf.InverseLerp(0.0f, currentTerrain.terrainData.size.x, terrainLocalPos.x),
            Mathf.InverseLerp(0.0f, currentTerrain.terrainData.size.z, terrainLocalPos.z));
        map = currentTerrain.terrainData.GetDetailLayer
            (0, 0, currentTerrain.terrainData.detailWidth, currentTerrain.terrainData.detailHeight, 0);
        for (int x = 0; x < currentTerrain.terrainData.detailWidth; x++)
        {
            for (int y = 0; y < currentTerrain.terrainData.detailHeight; y++)
            {
                if (map[x, y] > 1)
                {
                    normalizedPos.x *= currentTerrain.terrainData.heightmapResolution;
                    normalizedPos.y *= currentTerrain.terrainData.heightmapResolution;
                    Debug.Log(x + " " + y);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(normalizedPos.x, normalizedPos.y);
                }
            }
        }
    }
     * 
     * 
     * private Vector3 ConvertWorldCord2TerrCord(Vector3 wordCor)
{
    Vector3 vecRet = new Vector3();
    Terrain ter = currentTerrain;
    Vector3 terPosition = ter.transform.position;
    vecRet.x = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
    vecRet.z = ((wordCor.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
    return vecRet;
}*/

    /*void save old code()
    {
        //currentTerrain = other.GetComponentInParent<Terrain>();
        //currentTerrainDataCollider = currentTerrain.gameObject.GetComponent<TerrainCollider>();
        //currentTerrainData = currentTerrain.terrainData;

        /*if (currentTerrainData != null)
        {
            for (int t = 0; t < currentTerrainData.treeInstanceCount; t++)
            {
                //Vector3 treePos = currentTerrainData.GetTreeInstance(t).position;
                //Debug.Log(treePos);
            }
            int tt = 0;
            foreach (TreeInstance treeInstance in currentTerrainData.treeInstances)//adds tree instances to list
            {
                var treeToAdd = treeInstance;
                treeToAdd = currentTerrainData.GetTreeInstance(tt);
                treeInstancesList.Add(treeToAdd);
                tt++;
            }
        }
        if (treeInstancesList.Count > 0)
        {
            TreeInstance HolderClosestTree = treeInstancesList[0];
            closestTreeInstance = Vector3.Scale(HolderClosestTree.position, currentTerrainData.size) + currentTerrain.transform.position;
            foreach (TreeInstance Location in treeInstancesList) //multiplies all tree instance in the list
            {
                Vector3 position = Vector3.Scale(Location.position, currentTerrainData.size) + currentTerrain.transform.position;
                if (Vector3.Distance(position, this.gameObject.transform.position) < Vector3.Distance(closestTreeInstance, this.gameObject.transform.position))
                {
                    //if distance between holder tree position greater than closest tree; set new closest
                    HolderClosestTree = Location;
                    closestTreeInstance = position;
                }
            }
            closestTreeInstance.y = currentTerrain.SampleHeight(closestTreeInstance);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = closestTreeInstance;


            playerTerrainPos = Vector3.Scale(this.transform.position, currentTerrainData.size) + currentTerrain.transform.position;
            //Debug.Log("player terrain position " + playerTerrainPos);
            //Debug.Log(closestTreeInstance);
            Debug.Log("player dist to tree " + Vector3.Distance(playerTerrainPos, closestTreeInstance));
        }
    }*/
}
