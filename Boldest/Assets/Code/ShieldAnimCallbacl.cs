using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimCallbacl : MonoBehaviour
{

    Shielder _parent;


    void Start()
    {
        _parent = GetComponentInParent<Shielder>();
    }

    void AnimationDone()
    {
        GetComponent<Animator>().SetBool("Attack", false);
        _parent._isAttacking = false;

    }

    
}