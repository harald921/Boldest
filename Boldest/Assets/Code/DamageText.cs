using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
	[HideInInspector] public Text _text;	
	[HideInInspector] public Vector3 _posInWorld; // set when dealing dmg to enemy
	[HideInInspector] public float _dmg; // set when dealing dmg to enemy
	[SerializeField] float _offsetSpeed = 25.0f; // speed for moving text up
	[SerializeField] float _lifeTimeNotIncludingFades = 1.0f; // time existing with 100% alpha
	[SerializeField] float _fadeOutTime = 0.2f; // time fading out after lifetime
	[SerializeField] float _fadeInTime = 0.2f; // time fading in before lifetime starts

	[SerializeField] Vector2 _randomTextOffsetRangeX = Vector2.zero; // spawn range on x axis
	[SerializeField] Vector2 _randomTextOffsetRangeY = Vector2.zero; // spawn range on y axis

	Camera _camera;
	
	//offsets for moving the text
	Vector2 _randomStartOffset;
	float _offset = 0.0f;

	//variables for controlling alpha
	float _lifeTimer = 0.0f;
	float _alpha = 1.0f;
	Color _startColor;

	private void Start()
	{
		_camera = Camera.main;
		_text = GetComponent<Text>();
		_startColor = _text.color;
		_text.text = _dmg.ToString();
	
		//set the start offset position from enemy based on the ranged values
		_randomStartOffset.x = Random.Range(_randomTextOffsetRangeX.x,_randomTextOffsetRangeX.y);
		_randomStartOffset.y = Random.Range(_randomTextOffsetRangeY.x, _randomTextOffsetRangeY.y);

		// set destroy right away with lifetime including fadeing times
		Destroy(gameObject, _lifeTimeNotIncludingFades + _fadeOutTime + _fadeInTime);		
	}

	private void Update()
	{
		_lifeTimer += Time.deltaTime;

		//update position 
		UpdateScreenPosition();

		//fade in text if fade in time is not set to 0
		if (_lifeTimer < _fadeInTime && _fadeInTime != 0)
			FadeInText();

		//fade out text after lifetime counting fade in time	
		if (_lifeTimer >= _lifeTimeNotIncludingFades + _fadeInTime)
			FadeOutText();								
	}

	void UpdateScreenPosition()
	{
		// update screen pos so text wont follow moving screen
		_offset += _offsetSpeed * Time.deltaTime;
		_text.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _posInWorld); // base pos on whatever pos the enemy had when hit
		_text.rectTransform.position += new Vector3(_randomStartOffset.x, _randomStartOffset.y + _offset, 0);

	}

	void FadeInText()
	{
		//get value from 0-1 during the fadeintime
		_alpha = _lifeTimer / _fadeInTime;
		Color color = _startColor;
		color.a = _alpha;
		_text.color = color;

	}

	void FadeOutText()
	{
		//get value from 1-0 during fadeOutTime
		_alpha -= Time.deltaTime * (1 / _fadeOutTime);
		Color color = _startColor;
		color.a = _alpha;
		_text.color = color;
	}
}
