using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : Movement, Resetable{

    void Start()
    {
        (this as Resetable).Register();
        Spawn();
    }

    void OnDestroy()
    {
        (this as Resetable).Unregister();
    }


    void Resetable.Reset()
    {
        float opacity = gameObject.GetOpacity();
        trajectory.Clear();

        StartCoroutine(
            gameObject.FadeOut()
            .Then(() => Spawn())
            .Then(() => gameObject.SetOpacity(opacity))
        );
    }

    void Spawn()
    {
        if (Managers.Level == null)
            return;

        //position at spawn point
        gameObject.transform.position = Managers.Level.PlayerSpawn;

        //add chaser spawner
        ChaserSpawner spawner = gameObject.AddComponent<ChaserSpawner>() as ChaserSpawner;
        spawner.chaserObject = Managers.Level.monkeyObject;
        spawner.spawnTurnDelay = Managers.Level.monkeySpawnTurnDelay;
    }


	void Update () {
        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;
        

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 dest = gameObject.transform.position + new Vector3(horizontal, vertical);
            
            if (Managers.Turn.TurnInProgress)
                return;
            
            if (!CanMove(dest))
                return;

            Move(dest);
            Managers.Turn.TakeTurn();
        }
    }
}
