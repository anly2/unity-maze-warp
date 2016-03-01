using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance = null;

    public string Name;
    public List<Transform> playerSpawnPlaces;
    public int monkeySpawnTurnDelay = 3;
    public GameObject monkeyObject;
    public GameObject ghostObject;
    public float actorFadeOutDuration = 1;

    [HideInInspector]
    public Statistics Stats { get; private set; }
    [HideInInspector]
    public Vector3 PlayerSpawn { get { return playerSpawns.Current; } }

    private List<Resetable> resetables;
    private List<GhostInfo> ghostData;
    private IEnumerator<Vector3> playerSpawns; //converted + cycling
    private GameObject player; //for easy access


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

        playerSpawns = GetPlayerSpawnsEnumeration();
        playerSpawns.MoveNext();
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

        Intro();
    }


    /* Scripted Intro */

    void Intro()
    {
        //Late-bindings
        ButtonListener lCancelListener = null; //later-defined
        CoroutineExtentions.Action lExplore = null; //later-defined
        CoroutineExtentions.Action lFinishIntro = null; //later-defined
        Coroutine lIntro = null; //later-defined, changes multiple times

        //Timing settings
        float t0 = 0.5f; //motion to Exit
        float t1 = 1f;   //wait time at Exit
        float t2 = 1f;   //motion back to Player

        Camera camera = Camera.main;
        Vector3 initialPosition = camera.transform.position;
        Vector2 exitLocation = new Vector2(5, -7);

        lExplore = () => {
            Managers.Fog.Explore(new Vector2(0, 0));
            Managers.Fog.Explore(initialPosition);
            Managers.Fog.Explore(exitLocation, t0 + t1 / 2);
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

            lIntro = camera.MotionTo(exitLocation, t0)
                .Then(new WaitForSeconds(t1))
                .Then(() => lIntro = camera.MotionTo(initialPosition, t2).Start(this)) //.MotionTo must not be eval'd immediately!
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
        IEnumerator<Transform> places = playerSpawnPlaces.GetEnumerator();

        while (true)
        {
            while (places.MoveNext())
                yield return places.Current.position;

            places.Reset();
        }
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

        yield return new WaitForSeconds(actorFadeOutDuration);
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
