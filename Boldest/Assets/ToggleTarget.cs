using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTarget : MonoBehaviour
{
    public bool _isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Arrow")
        {
            _isActive = !_isActive;
        }
    }
}