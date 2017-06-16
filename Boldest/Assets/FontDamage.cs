using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontDamage : MonoBehaviour
{
    public float _floatSpeed = 10;
    private void Update()
    {
        transform.position += new Vector3(0, 1, 0) * _floatSpeed * Time.deltaTime;
    }



}