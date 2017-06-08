using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttackCheck : MonoBehaviour
{

	public Collider _hardCollider;

	







    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
			
			if (player._isDashing)
			{
				Physics.IgnoreCollision(other, _hardCollider, true);
				player.VisceralAttackWindow(GetComponent<Collider>());
			}
               

        }

    }

}

