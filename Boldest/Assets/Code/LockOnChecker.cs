using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnChecker : MonoBehaviour
{

    Player _player;

    void Start()
    {
        _player = GetComponentInParent<Player>();
    }









    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.gameObject.layer == 8)
            {

                _player._lockables.Add(other);
            }

        }
        
           
    }


    void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.gameObject.layer == 8)
            {
				//if exiting collider is the target then exit target mode and set target to index 0 to avoid out of range indexing
				if (other == _player._lockables[_player._currentLockOnID])
				{
					_player._isLockedOn = false;
					_player._currentLockOnID = 0;
					_player._lockables.Remove(other);
					return;
				}				
																												
				// find if the exiting collider is before or after the target in the list(if before, decrement the current locked on id by 1 )
				for (int i = 0; i < _player._lockables.Count; i++)
				{
					if (other == _player._lockables[i])
					{						
						if (i < _player._currentLockOnID)
							_player._currentLockOnID--;
						
					}
				}									
				_player._lockables.Remove(other);

				

			}
        }
        
           
    }

}