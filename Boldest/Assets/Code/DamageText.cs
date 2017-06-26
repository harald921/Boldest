using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
	[HideInInspector] public Text _text;	
	[HideInInspector] public EnemyBase _enemy;
	[SerializeField] float _offsetSpeed = 1.0f;
	[SerializeField] float _destroyAfter = 1.0f;
	[SerializeField] Vector2 _randomTextOffsetRangeX = Vector2.zero;
	[SerializeField] Vector2 _randomTextOffsetRangeY = Vector2.zero;

	Camera _camera;
	float _offset = 0.0f;
	Vector2 _randomStartOffset;

	private void Start()
	{
		_camera = Camera.main;
		_text = GetComponent<Text>();		
		_randomStartOffset.x = Random.Range(_randomTextOffsetRangeX.x,_randomTextOffsetRangeX.y);
		_randomStartOffset.y = Random.Range(_randomTextOffsetRangeY.x, _randomTextOffsetRangeY.y);

		Destroy(gameObject, _destroyAfter);

	}

	private void Update()
	{
		if (_enemy)
		{
			_offset += _offsetSpeed * Time.deltaTime;
			_text.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _enemy.transform.position);
			_text.rectTransform.position += new Vector3(_randomStartOffset.x,_randomStartOffset.y + _offset, 0);

			return;
		}
		Destroy(gameObject);
		
		
	}
}
