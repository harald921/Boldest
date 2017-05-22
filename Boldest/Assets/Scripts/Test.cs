using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 rot;

    void Start()
    {
        Debug.Log(rot);    
    }


    void Update()
    {

        transform.Rotate(new Vector3(5, 10, 5));

        
       

         

    }
       
        
}