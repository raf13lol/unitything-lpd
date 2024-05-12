using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class dontdielol_thatkillsyou : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find(gameObject.name) != transform.gameObject)
        {
            Destroy(transform.gameObject);
        }
        else
            DontDestroyOnLoad(transform.gameObject);
    }
}
