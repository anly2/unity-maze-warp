using UnityEngine;
using System.Collections;
using System;

public class ChaserSpawner : MonoBehaviour, TurnBased
{
    public GameObject chaserObject;
    public int spawnTurnDelay;

    [HideInInspector]
    public Vector3 spawnPosition;
    [HideInInspector]
    public ActionHistory actionHistory;


    void Awake ()
    {
        spawnPosition = gameObject.transform.position;
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
        Destroy(this);
    }

    public void SpawnChaser()
    {
        GameObject chaser = Instantiate(chaserObject, spawnPosition, Quaternion.identity) as GameObject;
        chaser.transform.parent = gameObject.transform.parent;

        ChaserMovement chaserMovement = chaser.GetComponent<ChaserMovement>();

        if (chaserMovement == null)
            return;

        if (actionHistory == null)
        {
            Movement movement = gameObject.GetComponent<Movement>();

            if (movement != null)
                actionHistory = movement.actionHistory;
        }
        
        chaserMovement.targetActions = actionHistory.GetEnumaration();
    }
}
