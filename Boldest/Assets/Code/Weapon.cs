using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public void OnAttackDone()
    {
        GetComponent<Animator>().SetBool("attack", false);   
        Debug.Log("Attack done");
    }

    public void TryAttack()
    {
        GetComponent<Animator>().SetBool("attack", true);
        Debug.Log("Attack start");
    }

    private void Start()
    {

    }
}