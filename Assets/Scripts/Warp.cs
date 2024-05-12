using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public int warpNum;

    public bool forceWarp;
    public string warpTo;
    public int warpNumTo;

    float warpTimerDelay;

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerMomento>().movedYet)
            warpTimerDelay += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (!forceWarp || warpTimerDelay < 0.5f)
            return;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (Vector3.Distance(obj.transform.position, transform.position) <= 1.05f)
            {
                var gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
                gc.whereWarpForced = warpTo;
                gc.isWarpForced = true;
                gc.whereWarpNumForced = warpNumTo;
                gc.OhShitWereWarping();
                break;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (forceWarp)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    }
}
