using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private int _ownedByPlayer = -1;

    [SerializeField] private int _startSpeed = 5;
    [SerializeField] private int _maxSpeed = 15;
    [SerializeField] private Vector3 _ballStartPosition = Vector3.zero;
    [SerializeField] private float _minYOffsetVelocity = 0.5f;
    [SerializeField] private float _minBounceAngleDeg = 15;
    [SerializeField] private float _maxBounceAngleDeg = 85;
    [SerializeField] private float _stuckVelocityLimit = 0.01f;
    [SerializeField] private int _stuckCountLimit = 2;

    private Rigidbody2D _myRb;

    private int _speed;
    private Vector3 _minVelocityNorm;
    private Vector3 _maxVelocityNorm;
    private int _stuckCount = 0;
    private bool _preventRespawn = false;
    private int _respawnSpeed;

    public int OwnedByPlayer { get => _ownedByPlayer; }
    public int Speed { get => _speed; }
    public bool PreventRespawn { get => _preventRespawn; set => _preventRespawn = value; }

    private void Awake()
    {
        _respawnSpeed = _speed = _startSpeed; 
        _minVelocityNorm = new Vector3(1, Mathf.Tan(_minBounceAngleDeg * Mathf.Deg2Rad), 0).normalized;
        _maxVelocityNorm = new Vector3(1, Mathf.Tan(_maxBounceAngleDeg * Mathf.Deg2Rad), 0).normalized;
        _myRb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        

        if (_preventRespawn)
        {
            Respawn(false, false);
        }
        else
        {
            Respawn(true);
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log("Magnitude = " + _myRb.velocity.magnitude.ToString());
        //if (_myRb.velocity.magnitude < _speed)
        //{
        _myRb.velocity = _myRb.velocity.normalized * _speed;
        if(_myRb.velocity.magnitude <= _stuckVelocityLimit)
        {
            if(++_stuckCount > _stuckCountLimit)
            {
                //stuck detection and correction
                _myRb.velocity = Random.insideUnitCircle.normalized * _speed;
                _stuckCount = 0;
                Debug.Log("Stuck detected, reset velocity to: " + _myRb.velocity.ToString());
            }
        }
        else
        {
            _stuckCount = 0;
        }
        //_previousVelocity = _myRb.velocity;
        //}
    }

    public void Respawn(bool resetSpeed, bool resetPlayer = true)
    {
        if (_preventRespawn) return;
        if (resetSpeed) _speed = _startSpeed;
        if (resetPlayer) SetPlayerOwnership(-1, Color.white);

        transform.position = _ballStartPosition;
        _myRb.velocity = Random.insideUnitCircle.normalized * _speed;
        if (_myRb.velocity.y > 0)
        {
            //limit to starting in the downwards direction
            _myRb.velocity.Scale(Vector2.down);
        }
        if (Mathf.Abs(_myRb.velocity.normalized.y) < (Mathf.Abs(_myRb.velocity.normalized.x) + _minYOffsetVelocity))
        {
            //limit to angles which are 
            _myRb.velocity = new Vector2(_myRb.velocity.x, -(Mathf.Abs(_myRb.velocity.normalized.x) + _minYOffsetVelocity));
            
        }
    }

    public void IncreaseSpeed(int speedIncrement)
    {
        _speed = Mathf.Min(_speed + speedIncrement, _maxSpeed);
    }

    public void IncreaseRespawnSpeed(int speedIncrement)
    {
        _respawnSpeed = Mathf.Min(_respawnSpeed + speedIncrement, _maxSpeed);
    }

    public void SetPlayerOwnership(int playerID, Color playerColor)
    {
        _ownedByPlayer = playerID;
        
        gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((collision.gameObject.tag == "Player") && (_preventRespawn))
        {
            //this is a new ball due to level completion
            _speed = _respawnSpeed;
        }

        Vector3 newVector3;
        float angle_rad;

        angle_rad = Mathf.Abs(Mathf.Atan(_myRb.velocity.y / _myRb.velocity.x));
        _minVelocityNorm = new Vector3(1, Mathf.Tan(_minBounceAngleDeg * Mathf.Deg2Rad), 0).normalized;
        _maxVelocityNorm = new Vector3(1, Mathf.Tan(_maxBounceAngleDeg * Mathf.Deg2Rad), 0).normalized;
        //limit reflection to outside of 30 degrees to prevent getting stuck in horizontal motion
        //adapted from https://answers.unity.com/questions/1920939/set-bounce-angle-of-an-object.html
        //if (angle_rad == 0)
        //{

        //    _myRb.velocity = _previousVelocity;
        //    Debug.Log("Angle: 0 - Corrected to previous velocity: " + _myRb.velocity.ToString());
        //}
        
        if (angle_rad < (Mathf.Deg2Rad * _minBounceAngleDeg)) 
        {
            newVector3 = _minVelocityNorm;
            newVector3 *= _myRb.velocity.magnitude;
            newVector3.x *= Mathf.Sign(_myRb.velocity.x);
            newVector3.y *= Mathf.Sign(_myRb.velocity.y);
            _myRb.velocity = newVector3;
            Debug.Log("Angle: " + (Mathf.Rad2Deg * angle_rad).ToString() + " - Limited to: " + (Mathf.Rad2Deg * Mathf.Abs(Mathf.Atan(_myRb.velocity.y / _myRb.velocity.x))).ToString("##.#"));
        
        }
        else if(angle_rad > (Mathf.Deg2Rad * _maxBounceAngleDeg))
        {
            newVector3 = _maxVelocityNorm;
            newVector3 *= _myRb.velocity.magnitude;
            newVector3.x *= Mathf.Sign(_myRb.velocity.x);
            newVector3.y *= Mathf.Sign(_myRb.velocity.y);
            _myRb.velocity = newVector3;
            Debug.Log("Angle: " + (Mathf.Rad2Deg * angle_rad).ToString() + " - Limited to: " + (Mathf.Rad2Deg * Mathf.Abs(Mathf.Atan(_myRb.velocity.y / _myRb.velocity.x))).ToString("##.#"));

        }
        else
        {
            //do nothing
            Debug.Log("Angle: " + (Mathf.Rad2Deg * angle_rad).ToString("##.#"));
        }

    }

}
