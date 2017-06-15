using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    //settings for player
    [SerializeField] float _health = 100.0f;
    float _maxHealth;
    [SerializeField] float _moveSpeed = 16.0f;
    [SerializeField] float _turnSpeed = 1.0f;
    [SerializeField] bool _useController = false;
    [SerializeField] float _gravityPower = 0;
    Vector3 _movementVector = Vector3.zero;
    [HideInInspector] public Vector3 _lastMovementVector = Vector3.zero;
    float _acceleration = 0;
    public float _accelerationSpeed = 1;
    Color _defaultColor;
    //settings for dash
    [SerializeField] float _dashDuration;
    [SerializeField] float _dashOffsetDuration;
    [SerializeField] float _dashSpeed;
    [SerializeField] float _dashOffsetSpeed;
    float _dashingSpeed;
    [SerializeField] float _dashCoolDown;
    [HideInInspector] public bool _isDashing = false;
    float _dashingTimer = 0;
    bool _rightTriggerReleased = true;
    bool _inKnockBack = false;
    Animator _dashAnimator;
    Vector3 _dashDir;
    float _dashFraction = 0;
    //settings for visceral attack
    [SerializeField] float _timeWindowToAttack;
    [SerializeField] float _timePreformingAttack;
    [SerializeField] GameObject _visceralAttackParticle;
    [SerializeField] ParticleSystem _dashParticle;
    [SerializeField] float _knockBackForce;
    bool _inVisceralAttack = false;
    Coroutine _visceralCo;
    public GameObject _attackEffect;
    //sword attack stuff
    public float _attackMomentum;
    public float _attackCoolDown;
    float _attackTimer = 0;
    public GameObject _swordSwipe;
    public float _swipeLenght = 0.2f;
    //bow stuff
    [HideInInspector] public bool _isBowing = false;
    [SerializeField] float _speedMultiOnBowing = 1.0f;
    //lock On stuff
    public List<Collider> _lockables;
    [HideInInspector] public int _currentLockOnID = 0;
    [HideInInspector] public bool _isLockedOn = false;
    float _changeTargetTimer = 0;
    [SerializeField] float _changeTargetDelay = 0.3f;
    public Image _bull;
    public Image _aim;
    Vector2 _playerUiPos;
    CameraShake _cameraShaker;
    [HideInInspector] public bool _isVunurable = true;
    [SerializeField] float _invulnerableTime = 0.1f;
    float _invunarableTimer = 0.0f;

    void Start()
    {
        _maxHealth = _health;
        _cameraShaker = Camera.main.GetComponent<CameraShake>();
        _defaultColor = GetComponent<MeshRenderer>().material.color;
        _dashDir = transform.forward;
        _bull.gameObject.SetActive(false);
        _aim.gameObject.SetActive(false);
    }

    private void Update()
    {
        GameObject.Find("Healthbar").transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.InverseLerp(0, _maxHealth, _health);
        _aim.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        LockOnEnemy();
        //can only controll player if not in dash or in the middle of chain attacks
        if (!_isDashing && !_inVisceralAttack && !_inKnockBack && _attackTimer > _attackCoolDown)
        {
            HandleMovement();
            HandleAttackInput();
        }
        HandleAimingOnVisceralAttack();
        _invunarableTimer += Time.deltaTime;
        if (_invunarableTimer > _invulnerableTime)
            _isVunurable = true;
        else
            _isVunurable = false;
        _attackTimer += Time.deltaTime;
        _acceleration += Time.deltaTime * _accelerationSpeed;
        _acceleration = Mathf.Clamp(_acceleration, 0, 1);
    }

    private void FixedUpdate()
    {
        if (!_isDashing && !_inVisceralAttack)
        {
            float speedMulti = 1.0f;
            if (_isBowing)
                speedMulti = _speedMultiOnBowing;
            GetComponent<Rigidbody>().AddForce(new Vector3(((_movementVector.normalized.x * _moveSpeed) * _acceleration) * speedMulti, -_gravityPower, ((_movementVector.normalized.z * _moveSpeed) * _acceleration) * speedMulti));
            _movementVector = Vector3.zero;
        }

        HandleDash2();
    }

    void HandleMovement()
    {
        _movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (Mathf.Abs(_movementVector.x) > 0 || Mathf.Abs(_movementVector.z) > 0)
            _lastMovementVector = _movementVector;

        else
            _acceleration = 0;

        if (_isLockedOn && _lockables.Count > 0)
        {
            Vector3 lookVector = _lockables[_currentLockOnID].transform.position - transform.position;
            lookVector.Normalize();
            _lastMovementVector = lookVector;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lastMovementVector), _turnSpeed * Time.deltaTime);
    }

    void HandleAimingOnVisceralAttack()
    {
        if (_inVisceralAttack)
        {
            Vector2 stick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Mathf.Abs(stick.x) > 0 || Mathf.Abs(stick.y) > 0)
                _aim.gameObject.SetActive(true);
            else
                _aim.gameObject.SetActive(false);
            _aim.transform.right = stick;
        }

        else
            _aim.gameObject.SetActive(false);
    }

    void HandleAttackInput()
    {
        if (Input.GetButtonDown("RightHandButton"))
            if (_attackTimer >= _attackCoolDown)
            {
                _attackTimer = 0;
                GameObject swipe = Instantiate(_swordSwipe, transform.position + (transform.forward * 1.5f), transform.rotation * _swordSwipe.transform.rotation);
                swipe.transform.parent = transform;
                Destroy(swipe.gameObject, _swipeLenght);
                AttackForce();
            }

        if (Input.GetButtonDown("BowButton"))
            transform.GetComponentInChildren<Bow>().DrawBow();
    }

    void HandleDash2()
    {
        _dashingTimer -= Time.deltaTime;

        if (Input.GetAxisRaw("RightHandTrigger") > 0 && _dashingTimer + _dashCoolDown < 0 && _rightTriggerReleased && !_inVisceralAttack)
        {
            _dashingTimer = _dashDuration;
            _rightTriggerReleased = false;
            // start dashparticle
            _dashParticle.gameObject.SetActive(true);
            _dashParticle.Play();

            if (_isLockedOn)
            {
                Vector3 playerToTarget = _lockables[_currentLockOnID].transform.position - transform.position;
                playerToTarget.Normalize();
                Vector3 leftStickVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                leftStickVector.Normalize();
                float dot = Vector3.Dot(playerToTarget, leftStickVector);

                if (dot < 0.8f && dot != 0)
                {
                    _dashDir = leftStickVector;
                    transform.forward = leftStickVector;
                    _dashingTimer = _dashOffsetDuration;
                    _dashingSpeed = _dashOffsetSpeed;
                }

                else
                {
                    _dashDir = transform.forward;
                    _dashingTimer = _dashDuration;
                    _dashingSpeed = _dashSpeed;
                }
            }

            else
            {
                _dashDir = transform.forward;
                _dashingTimer = _dashDuration;
                _dashingSpeed = _dashSpeed;
            }
        }

        else if (Input.GetAxisRaw("RightHandTrigger") == 0)
            _rightTriggerReleased = true;

        if (_dashingTimer > 0 && !_inVisceralAttack)
        {
            _dashFraction = Mathf.InverseLerp(_dashDuration, 0, _dashingTimer);
            float v = Mathf.Sin(_dashFraction * Mathf.PI);
            _isDashing = true;
            GetComponent<Rigidbody>().AddForce(_dashDir * (_dashingSpeed * v));
        }

        else
        {
            _isDashing = false;
        }
    }
    public void VisceralAttackWindow(Collider enemyCollider)
    {
        Time.timeScale = 0.1f;
        _visceralCo = StartCoroutine(HandleVisceralAttackWindow(enemyCollider));
    }

    void FailedVisceralAttack(Collider enemyCollider)
    {
        //if visceral attack failed, re-enable collision between enemy and player
        Physics.IgnoreCollision(enemyCollider, GetComponent<Collider>(), false);
        //StartCoroutine(KnockBack(-transform.forward));
        _inVisceralAttack = false;
    }

    IEnumerator HandleVisceralAttackWindow(Collider enemyCollider)
    {
        float timeToAttack = _timeWindowToAttack;
        bool failedAttack = true;
        EnemyBase enemy = enemyCollider.GetComponent<EnemyBase>();

        while (timeToAttack > 0)
        {
            timeToAttack -= Time.deltaTime;
            if (Input.GetButtonDown("RightHandButton") && enemy._isVunurable)
            {
                if (enemyCollider.tag == "Shielder")
                    enemyCollider.GetComponent<Shielder>().GettingDashed();

                if (enemyCollider.tag == "Bird")
                    enemyCollider.GetComponentInParent<Bird>()._move = false;

                GameObject effect = Instantiate(_attackEffect, enemyCollider.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                Destroy(effect, _timePreformingAttack);
                failedAttack = false;
                _inVisceralAttack = true;
                transform.LookAt(enemyCollider.transform);
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                StartCoroutine(PreformVisceralAttack(enemyCollider));
                Time.timeScale = 1.0f;
                StopCoroutine(_visceralCo);
            }
            yield return null;
        }

        if (failedAttack)// for some reason StopCoroutine seems to not be 100% reliable, added bool if this line gets read even on succsesful attack                        
        {
            FailedVisceralAttack(enemyCollider);
            Time.timeScale = 1.0f;
        }
    }

    IEnumerator PreformVisceralAttack(Collider enemyCollider)
    {
        _invunarableTimer = 0 - (_timePreformingAttack + 0.5f);
        yield return new WaitForSeconds(_timePreformingAttack); // wait to finish attack, will be timed on attack animation	
        RemoveEnemyFromList(enemyCollider);
        Destroy(enemyCollider.gameObject); //destroy enemy you attacked (maybe can do different things depending on the type of enemy and tag later on)			
        GameObject deathParticle = Instantiate(_visceralAttackParticle, enemyCollider.transform.position, _visceralAttackParticle.transform.rotation);
        Destroy(deathParticle.gameObject, 3);
        //automaticly set up new dash after finishing attack	       		
        bool haveDashDirection = true;
        Vector3 newDashDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        newDashDir.Normalize();

        if (newDashDir.x == 0 && newDashDir.z == 0)
            haveDashDirection = false;

        if (haveDashDirection)
        {
            transform.forward = newDashDir;
            _dashDir = newDashDir;
            _dashingTimer = _dashDuration;
            _isDashing = true;
            _dashParticle.Play();
        }

        _inVisceralAttack = false;
        _cameraShaker.DoAimPunch(5, 60.0f);
    }

    public IEnumerator KnockBack(Vector3 inDirection)
    {
        _inKnockBack = true;
        GetComponent<Rigidbody>().AddForce(inDirection * _knockBackForce);
        yield return new WaitForSeconds(0.5f);
        _inKnockBack = false;
    }

    public void CancelDash()
    {
        _dashingTimer = 0;
    }

    void AttackForce()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * _attackMomentum);
    }

    void LockOnEnemy()
    {
        if (_lockables.Count == 0)
            _isLockedOn = false;

        if (Input.GetButtonDown("R3") && !_inVisceralAttack)
        {
            if (_lockables.Count > 0)
                _isLockedOn = !_isLockedOn;
            else
                _isLockedOn = false;
            if (_isLockedOn)
                _currentLockOnID = 0;
        }
        _changeTargetTimer += Time.deltaTime;

        if (_isLockedOn)
        {
            _bull.gameObject.SetActive(true);
            if (Input.GetAxisRaw("RightStickHorizontal") > 0 && _currentLockOnID < _lockables.Count - 1 && _changeTargetTimer > _changeTargetDelay)
            {
                _currentLockOnID++;
                _changeTargetTimer = 0;
            }

            if (Input.GetAxisRaw("RightStickHorizontal") < 0 && _currentLockOnID > 0 && _changeTargetTimer > _changeTargetDelay)
            {
                _currentLockOnID--;
                _changeTargetTimer = 0;
            }

            if (_lockables.Count > 0)
                for (int i = 0; i < _lockables.Count; i++)
                    if (i == _currentLockOnID)
                        _bull.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _lockables[i].transform.position);
        }

        else
            _bull.gameObject.SetActive(false);
    }

    public void RemoveEnemyFromList(Collider other)
    {
        //if exiting collider is the target then exit target mode and set target to index 0 to avoid out of range indexing
        if (other == _lockables[_currentLockOnID])
        {
            _isLockedOn = false;
            _currentLockOnID = 0;
            _lockables.Remove(other);
            return;
        }

        // find if the exiting collider is before or after the target in the list(if before, decrement the current locked on id by 1 )
        for (int i = 0; i < _lockables.Count; i++)
            if (other == _lockables[i])
                if (i < _currentLockOnID)
                    _currentLockOnID--;

        _lockables.Remove(other);
    }

    public void ModifyHealth(float inHealthModifier)
    {
        _health += inHealthModifier;
        _invunarableTimer = 0;
        if (_health <= 0)
        {
            transform.position = new Vector3(0, 0, 0);
            _health = 100;
        }
        if (inHealthModifier < 0)
        {
            _cameraShaker.SetShakeDuration(0.1f, 0.2f);
            StartCoroutine(DamageFlash());
        }
    }

    IEnumerator DamageFlash()
    {
        float flashDuration = 0.13f;
        float timer = flashDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float flashLerpValue = Mathf.InverseLerp(0, flashDuration, timer);
            GetComponent<MeshRenderer>().material.color = Color.Lerp(_defaultColor, Color.red, flashLerpValue);
            yield return null;
        }
    }
}