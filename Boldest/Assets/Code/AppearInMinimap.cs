using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearInMinimap : MonoBehaviour
{
	[SerializeField]
	Image _icon;
	
	void Start ()
	{
		MinimapManager.AddIcon(gameObject,_icon);
	}
	
	


}
