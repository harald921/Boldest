using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public void OnAttackDone()
    {
        GetComponent<Animator>().SetBool("IsAttacking", false);   
        Debug.Log("Attack done");
    }

    public void TryAttack()
    {
        GetComponent<Animator>().SetBool("IsAttacking", true);
        Debug.Log("Attack start");
    }
}