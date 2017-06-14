using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    float _originalFOV;

    bool _isShaking = false;

    public void SetShakeDuration(float inShakeDuration, float inShakeIntensity)
    {
        _isShaking = true;
        shakeDuration = inShakeDuration;
        shakeAmount = inShakeIntensity;
    }

    void Awake()
    {
        _originalFOV = Camera.main.fieldOfView;

        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (!_isShaking)
            return;

        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
            _isShaking = false;
        }
    }

    public void DoAimPunch(float inPunchForce, float inPunchSpeed)
    {
        StartCoroutine(AimPunch(inPunchForce, inPunchSpeed));
    }

    IEnumerator AimPunch(float inPunchForce, float inPunchSpeed)
    {
        Debug.Log(_originalFOV);
        float punchedFOV = _originalFOV - inPunchForce;

        while (true)
        {
            punchedFOV += Time.deltaTime * inPunchSpeed;

            Camera.main.fieldOfView = punchedFOV;

            if (punchedFOV > _originalFOV)
            {
                Camera.main.fieldOfView = _originalFOV;
                break;
            }

            yield return null;
        }


        yield return null;
    }
}