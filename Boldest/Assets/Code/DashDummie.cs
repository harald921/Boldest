using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDummie : MonoBehaviour
{



	







    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
			
			if (player._isDashing)
			{
				player.visceralAttackWindow(GetComponent<Collider>());
			}
               

        }

    }

}

