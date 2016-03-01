using UnityEngine;
using System.Collections;
using System;

public class ChaserSpawner : MonoBehaviour, TurnBased
{
    public GameObject chaserObject;
    public int spawnTurnDelay;
    // Added by Lewis 
    private Vector3 targetPosition;
    [HideInInspector]
    public Vector3 spawnPosition;
    public Trajectory targetTrajectory;

    void Awake ()
    {
        spawnPosition = gameObject.transform.position;      
    }

    void Update()
    {
        //Added by Lewis. quite tricky. hope you can improve it. 
        //for the effect of smoothmovement, the position will be (1.1, -3, 0) rather than (2, -3, 0)
        float x = Mathf.Ceil(GameObject.Find("Player").transform.position.x);
        float y = Mathf.Ceil(GameObject.Find("Player").transform.position.y);
        targetPosition = new Vector3(x, y, 0f);
    }
    
    void Start()
    {
        this.Register();
    }

    void OnDestroy()
    {
        this.Unregister();
    }


    void TurnBased.Turn()
    {
        if (spawnTurnDelay-- > 0)
            return;
        
        SpawnChaser();

        Invoke("Dispose", 0f);
    }

    void Dispose()
    {
        //Managers.Turn.RemoveTurnBasedListener(this);
        Destroy(this);
    }

    public void SpawnChaser()
    {
        GameObject chaser = Instantiate(chaserObject, spawnPosition, Quaternion.identity) as GameObject;
        chaser.transform.parent = gameObject.transform.parent;

        ChaserMovement chaserMovement = chaser.GetComponent<ChaserMovement>();

        if (chaserMovement == null)
            return;

        if (targetTrajectory == null)
        {
            Movement movement = gameObject.GetComponent<Movement>();

            if (movement != null)
                targetTrajectory = movement.trajectory;
        }
        
        chaserMovement.target = targetTrajectory.GetEnumaration();

        // Added by Lewis 
        chaserMovement.target = PathFinding.FindPath(chaserMovement, targetPosition);

    }
}
