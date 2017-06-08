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
    [SerializeField] float _aimSpeed                 = 1.0f;

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
        GetComponentInParent<Player>()._isBowing = false;

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
        GetComponentInParent<Player>()._isBowing = true;

        while (_isDrawingBow)
        {
			
				_currentDraw += _drawSpeed * Time.deltaTime;

				if (_currentDraw > _maxDraw)
					_currentDraw = _maxDraw;

            if (!GetComponentInParent<Player>()._isLockedOn)
            {
                //aim bow, if stick input set _lastMovementVector in player to the stick input(avoids snapping back to last known direction after fire) and move the player rotation accordingly
                Vector3 leftStick = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                if (Mathf.Abs(leftStick.x) > 0 || Mathf.Abs(leftStick.z) > 0)
                    GetComponentInParent<Player>()._lastMovementVector = leftStick;

                transform.parent.forward = GetComponentInParent<Player>()._lastMovementVector;
            }
				

				if (Input.GetButtonUp("BowButton"))
					ReleaseString();

				yield return null;						

		}

        FireProjectile();
    }


    void SetBowVisibility(bool inVisibility)
    {
        GetComponent<MeshRenderer>().enabled = inVisibility;
    }

}
