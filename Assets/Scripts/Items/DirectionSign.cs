using UnityEngine;
using System.Collections;

public class DirectionSign : MonoBehaviour {

    public GameObject signObject;

    private Transform player;
    private bool firstTime;
    private GameObject signClone = null;

    void Start()
    {
        firstTime = true;
    }

    void Update()
    {
        player = GameObject.Find("Player").transform;

        if (signObject == null)
            return;

        StartCoroutine(InstantiateSign());

        if (signClone != null && player.position == signClone.transform.position)
            Destroy(signClone);
   
    }

    IEnumerator InstantiateSign()
    {
        if (firstTime)
        {
            Vector3 signPos = new Vector3(player.position.x, player.position.y - 1, player.position.z);
            signClone = Instantiate(signObject, signPos, Quaternion.identity) as GameObject;
            firstTime = false;
        }
        else
        {
            yield return null;
        }

    }
}
