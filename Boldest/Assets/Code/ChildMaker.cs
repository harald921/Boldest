using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMaker : MonoBehaviour
{


    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            other.transform.parent = transform;
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            other.transform.parent = other.transform;
        }
    }

}
