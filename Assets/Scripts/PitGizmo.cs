using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    }
}
