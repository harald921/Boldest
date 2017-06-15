using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldCharacter : MonoBehaviour
{

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
            col.transform.parent = gameObject.transform;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
            col.transform.parent = null;
    }
}