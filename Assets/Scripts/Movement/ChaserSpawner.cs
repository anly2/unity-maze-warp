using UnityEngine;
using System.Collections;
using System;

public class ChaserSpawner : MonoBehaviour, TurnBased
{
    public GameObject chaserObject;
    public int spawnTurnDelay;
    [HideInInspector]
    public Vector3 spawnPosition;
    public Trajectory targetTrajectory;

    void Start () {
        GameController.instance.AddTurnBasedListener(this);

        spawnPosition = gameObject.transform.position;
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
        GameController.instance.RemoveTurnBasedListener(this);
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
                targetTrajectory = movement.GetTrajectory();
        }
        
        chaserMovement.target = targetTrajectory.GetEnumaration();
    }
}
