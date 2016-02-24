using UnityEngine;
using System.Collections;
using System;

public class SpawnMonkeyChaser : MonoBehaviour, TurnBased
{
    public GameObject monkeyObject;
    public int spawnTurnDelay;

    private Vector3 spawnPosition;

	void Start () {
        GameController.instance.AddTurnBasedListener(this);

        spawnPosition = gameObject.transform.position;
	}

    void TurnBased.Turn()
    {
        if (spawnTurnDelay < 0)
            return;

        if (spawnTurnDelay-- > 0)
            return;

        SpawnMonkey();
    }

    public void SpawnMonkey()
    {
        GameObject monkey = Instantiate(monkeyObject, spawnPosition, Quaternion.identity) as GameObject;
        monkey.transform.parent = gameObject.transform.parent;

        MonkeyMovement monkeyMovement = monkey.GetComponent<MonkeyMovement>();

        if (monkeyMovement == null)
            return;

        Movement movement = GetComponent<Movement>();

        if (movement == null)
            return;

        monkeyMovement.target = movement.GetTrajectory();
    }
}
