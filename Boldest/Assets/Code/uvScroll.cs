using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uvScroll : MonoBehaviour
{

    float _xScroll;
    float _yScroll;
    public float _scrollSpeedOnX;
    public float _scrollSpeedOnY;
    public bool _scrollX;
    public bool _scrollY;
    Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }


    void Update()
    {
    //tr
        if(_scrollX)
        {
            _xScroll += _scrollSpeedOnX * Time.deltaTime;
            _material.SetTextureOffset("_MainTex", new Vector2(_xScroll, _material.GetTextureOffset("_MainTex").y));

        }

        if (_scrollY)
        {
            _yScroll += _scrollSpeedOnY * Time.deltaTime;
            _material.SetTextureOffset("_MainTex", new Vector2(_material.GetTextureOffset("_MainTex").x, _yScroll));

        }
    }









}