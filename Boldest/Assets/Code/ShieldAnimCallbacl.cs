using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimCallbacl : MonoBehaviour
{


    void AnimationDone()
    {
        GetComponent<Animator>().SetBool("Attack", false);
    }

    
}