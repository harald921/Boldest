using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldCharacter : MonoBehaviour
{

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player")
            col.transform.parent = gameObject.transform;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.transform.tag == "Player")
            col.transform.parent = null;
    }
}