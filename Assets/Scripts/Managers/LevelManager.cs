using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance = null;

    [Header("Level properties")]
    public string Name;

    [Header("Level Intro settings")]
    public float durationOfCameraMotionToExit = 0.5f;
    public float durationOfStareAtExit = 1f;
    public float durationOfCameraMotionToSpawn = 1f;

    [Header("Gameplay Settings")]
    public int monkeySpawnTurnDelay = 3;

    [Header("Prefabs to Use")]
    public GameObject monkeyObject;
    public GameObject ghostObject;

    [HideInInspector]
    public Statistics Stats { get; private set; }
    [HideInInspector]
    public Vector3 PlayerSpawn { get { return playerSpawns.Current; } }

    private GameObject player; //for easy access
    private GameObject exit;
    private IEnumerator<Vector3> playerSpawns; //cycling

    private List<Resetable> resetables;
    private List<GhostInfo> ghostData;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        resetables = new List<Resetable>();
        ghostData = new List<GhostInfo>();

        Stats = new Statistics();
    }

    void OnDestroy()
    {
        instance = null;
    }

    void Start()
    {
        new Statistics.DeathCounter().Register();
        new Statistics.TurnCounter().Register();

        player = GameObject.FindWithTag("Player");

        exit = GameObject.FindWithTag("Exit");

        playerSpawns = GetPlayerSpawnsEnumeration();
        playerSpawns.MoveNext();

        Invoke("Intro", 0); //queue the Intro, after all Start()s have been called
    }


    /* Scripted Intro */

    void Intro()
    {
        //Late-bindings
        ButtonListener lCancelListener = null; //later-defined
        CoroutineExtentions.Action lExplore = null; //later-defined
        CoroutineExtentions.Action lFinishIntro = null; //later-defined
        Coroutine lIntro = null; //later-defined, changes multiple times

        Camera camera = Camera.main;
        Vector3 initialPosition = camera.transform.position;
        Vector2 exitLocation = exit.transform.position;

        lExplore = () => {
            Managers.Fog.Explore(new Vector2(0, 0));
            Managers.Fog.Explore(initialPosition);
            Managers.Fog.Explore(exitLocation, (durationOfCameraMotionToExit + durationOfStareAtExit / 2));
        };

        lFinishIntro = () => {
            camera.transform.position = initialPosition;
            Managers.Turn.TurnInProgress = false;
            lCancelListener.Remove();
        };


        lIntro = //carries onto next lines
        Managers.UI.ShowPreScreen()
            .Then(delegate ()
        {
            lExplore();

            lIntro = camera.MotionTo(exitLocation, durationOfCameraMotionToExit)
                .Then(new WaitForSeconds(durationOfStareAtExit))
                .Then(() => lIntro = camera.MotionTo(initialPosition, durationOfCameraMotionToSpawn).Start(this)) //.MotionTo must be eval'd later
                .Then(lFinishIntro)
                .Start(this);

        }).Start(this);

        lCancelListener = this.OnButtonDown("Cancel", () => {
            StopCoroutine(lIntro);
            lExplore();

            if (lFinishIntro != null)
                lFinishIntro();
            else
                Managers.UI.SkipScreen();
        });
    }

   
    /* Inner Classes */

    public class Statistics
    {
        public int DeathCount = 0;
        public int TurnCount = 0;

        private float _startTime = Time.time;
        public float TimeElapsed
        {
            get {
                return Time.time - _startTime;
            }

            internal set {
                _startTime = value;
            }
        }

        public class DeathCounter : Resetable
        {
            public void Reset()
            {
                Managers.Level.Stats.DeathCount++;
            }
        }

        public class TurnCounter : TurnBased
        {
            public void Turn()
            {
                Managers.Level.Stats.TurnCount++;
            }
        }
    }

    private struct GhostInfo
    {
        public int turnDelay;
        public Trajectory trajectory;
        public Vector3 spawnPoint;

        public GhostInfo(int turnDelay, Vector3 spawnPoint, Trajectory trajectory)
        {
            this.turnDelay = turnDelay;
            this.trajectory = trajectory;
            this.spawnPoint = spawnPoint;
        }
    }


    /* Manage Spawn Points */

    IEnumerator<Vector3> GetPlayerSpawnsEnumeration()
    {
        List<Vector3> spawns = GetSpawns();

        if (spawns.Count == 0)
            yield break;


        IEnumerator<Vector3> places = spawns.GetEnumerator();
        
        while (true)
        {
            while (places.MoveNext())
                yield return places.Current;

            places.Reset();
        }
    }

    List<Vector3> GetSpawns()
    {
        List<Vector3> points = new List<Vector3>();
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn Point");
        
        foreach(GameObject spawn in spawns)
        {
            try {
                int i = spawn.GetComponent<Spawn>().spawnIndex;

                i %= spawns.Length;

                if (i < 0)
                    i = spawns.Length - i;

                points.Insert(i, spawn.transform.position);
            }
            catch (Exception) {
                points.Add(spawn.transform.position);
            }
        }
        
        return points;
    }


    /* Manage Subscriptions */

    public void RegisterResetable(Resetable resetable)
    {
        this.resetables.Add(resetable);
    }

    public bool UnregisterResetable(Resetable resetable)
    {
        return this.resetables.Remove(resetable);
    }


    /* Functionality */

    // Player was caught! Wrap the level back to starting positions
    public void Warp()
    {
        Managers.Turn.TurnInProgress = true;

        Managers.UI.Hit();

        AddGhost(Stats.DeathCount);

        playerSpawns.MoveNext();

        StartCoroutine(ResetAll()
            .Then(() => StartGhostSpawners())
            .Then(() => Managers.Turn.TurnInProgress = false));
    }

    // Player finished the level!
    public void Complete()
    {
        Managers.Turn.TurnInProgress = true;
        Managers.UI.ShowPostScreen();
    }


    /* Helper methods */

    void AddGhost(int turnDelay)
    {
        Trajectory targetTrajectory = player.GetComponent<Movement>().trajectory.Clone();

        Vector3 spawnPoint = playerSpawns.Current;

        ghostData.Add(new GhostInfo(turnDelay, spawnPoint, targetTrajectory));
    }

    IEnumerator ResetAll()
    {
        foreach (Resetable thing in resetables)
            thing.Reset();

        yield return new WaitForSeconds(Managers.Game.actorFadeOutDuration);
    }

    void StartGhostSpawners()
    {
        foreach (GhostInfo ghostInfo in ghostData)
        {
            ChaserSpawner spawner = gameObject.AddComponent<ChaserSpawner>() as ChaserSpawner;
            spawner.chaserObject = ghostObject;
            spawner.spawnPosition = ghostInfo.spawnPoint;
            spawner.spawnTurnDelay = ghostInfo.turnDelay;
            spawner.targetTrajectory = ghostInfo.trajectory;
        }
    }
}
