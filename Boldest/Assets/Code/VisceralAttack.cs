using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisceralAttack : MonoBehaviour
{
	Animator _anim;
	void Start()
	{
		_anim = GetComponent<Animator>();
		_anim.SetBool("dashAttacking", false);
	}
	public void SetAttackActive()
	{
		_anim.SetBool("dashAttacking", true);

	}

	public void SetAttackInactive()
	{

		_anim.SetBool("dashAttacking", false);
		
	}
	
}
