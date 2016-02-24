using UnityEngine;
using System.Collections;
using System;

public class SpawnMonkeyChaser : MonoBehaviour, TurnBased
{
    public GameObject monkeyObject;
    public int spawnTurnDelay;

    private Transform spawnPoint;

	void Start () {
        GameController.instance.AddTurnBasedListener(this);

        spawnPoint = gameObject.transform;
	}

    void TurnBased.Turn()
    {
        if (spawnTurnDelay-- > 0)
            return;

        SpawnMonkey();
    }

    public void SpawnMonkey()
    {
        GameObject monkey = Instantiate(monkeyObject, spawnPoint.position, Quaternion.identity) as GameObject;
        monkey.transform.parent = spawnPoint.parent;

        MonkeyMovement monkeyMovement = monkey.GetComponent<MonkeyMovement>();

        if (monkeyMovement == null)
            return;

        Movement movement = GetComponent<Movement>();

        if (movement == null)
            return;

        monkeyMovement.target = movement.GetTrajectory();
    }
}
