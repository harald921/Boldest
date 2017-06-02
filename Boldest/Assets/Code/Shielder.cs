using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Shielder : MonoBehaviour
{
    Player _player;
    Animator _shieldAnim;
    Animator _swordAnim;

    float _stateChange = 0;
    

    void Start()
    {
        _player = FindObjectOfType<Player>();
        _shieldAnim = transform.GetChild(0).GetComponent<Animator>();
        _swordAnim = transform.GetChild(1).GetComponent<Animator>();
    }


    void Update()
    {
        Vector3 look = new Vector3( _player.transform.position.x, transform.position.y, _player.transform.position.z) - transform.position;
        look.Normalize();
        transform.forward = look;

        _stateChange += Time.deltaTime;

        if(_stateChange > 7)
        {

            _shieldAnim.SetBool("Attack", true);
            _swordAnim.SetBool("Attack", true);
            _stateChange = 0;
        }
        
    }
}