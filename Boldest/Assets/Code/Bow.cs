using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] GameObject _projectile;

    [SerializeField] float _maxDraw                  = 1.0f;
    [SerializeField] float _drawSpeed                = 1.0f;
                     float _currentDraw              = 0.0f;
    [SerializeField] float _drawToVelocityMultiplier = 1.0f;

    bool _isDrawingBow = false;

    public void DrawBow()
    {
        if (!_isDrawingBow)
            StartCoroutine(WaitForRelease());

    }

    public void ReleaseString()
    {
        _isDrawingBow = false;
        SetBowVisibility(false);
    }

    void FireProjectile()
    {

        GameObject firedProjectile = Instantiate(_projectile, transform.position, Quaternion.identity);

        firedProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * _currentDraw * _drawToVelocityMultiplier);
        firedProjectile.transform.forward = transform.forward;

        _currentDraw = 0.0f;
    }

    IEnumerator WaitForRelease()
    {
        SetBowVisibility(true);
        _isDrawingBow = true;

        while (_isDrawingBow)
        {
            _currentDraw += _drawSpeed * Time.deltaTime;

            if (_currentDraw > _maxDraw)
                _currentDraw = _maxDraw;

            yield return null;
        }

        FireProjectile();
    }


    void SetBowVisibility(bool inVisibility)
    {
        GetComponent<MeshRenderer>().enabled = inVisibility;
    }

}
