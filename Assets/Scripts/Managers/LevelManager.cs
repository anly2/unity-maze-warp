using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance = null;

    public string Name;

    [HideInInspector]
    public Statistics Stats { get; private set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Stats = new Statistics();
    }

    void Start() {
        UIManager.instance.ShowPreScreen();
    }

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
    }


    public void Complete()
    {
        UIManager.instance.ShowPostScreen();
    }
}
