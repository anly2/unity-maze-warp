using UnityEngine;
using System;

public class KillByContact : MonoBehaviour {
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Managers.Level.Warp();
        }
        else if (other.gameObject.tag == "Ghost")
        {
            Resetable[] resetables = other.gameObject.GetComponents<Resetable>();
            foreach (Resetable resetable in resetables)
                resetable.Reset();


            //if this is monkey
            //chase the player now
            try {
                GetComponent<MonkeyMovement>().target = GameObject.FindWithTag("Player");
            }
            catch (NullReferenceException) { /* ignore */ }
        }
        //Banana is handled in MonkeyMovement
    }
}
