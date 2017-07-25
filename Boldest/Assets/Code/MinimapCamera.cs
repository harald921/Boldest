using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
	[SerializeField]
	Transform _playerTransform;

	[SerializeField]
	Shader _shader;

	[SerializeField]
	bool _useCustomShader = false;
	
	void Start ()
	{
		if(_useCustomShader)
			GetComponent<Camera>().SetReplacementShader(_shader, "");
    }
	
	
	void Update ()
	{
		transform.position = _playerTransform.position + new Vector3(0, 100, 0);
	}
}
