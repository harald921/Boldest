using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterDistortion : MonoBehaviour
{
    public float _distortAmount;
    public float _distortSpeed;
    private float _timer;
    private float _distortion;
    private Material _material;


    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    void Update()
    {

        _timer += Time.deltaTime * _distortSpeed;
        _distortion = Mathf.PerlinNoise(_timer, _timer);
        _distortion = Mathf.Lerp(-1, 1, _distortion);

        _material.SetFloat("_waveValue", _distortion);
        _material.SetFloat("_distortAmount", _distortAmount);

    }

}