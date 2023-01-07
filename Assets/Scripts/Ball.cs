using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private int _ownedByPlayer = -1;

    [SerializeField] private int _startSpeed = 1;
    [SerializeField] private int _maxSpeed = 15;
    [SerializeField] private Vector3 _ballStartPosition= Vector3.zero;
    [SerializeField] private float _minYOffsetVelocity = 0.5f;
    private Rigidbody2D _myRb;

    private int _speed;

    public int OwnedByPlayer { get => _ownedByPlayer; }
    public int Speed { get => _speed; }

    private void Awake()
    {
        _myRb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        Respawn(true);
    }

    private void FixedUpdate()
    {
        Debug.Log("Magnitude = " + _myRb.velocity.magnitude.ToString());
        //if (_myRb.velocity.magnitude < _speed)
        //{
            _myRb.velocity = _myRb.velocity.normalized * _speed;
        //}
    }

    public void Respawn(bool resetSpeed, bool resetPlayer = true)
    {
        if (resetSpeed) _speed = _startSpeed;
        if (resetPlayer) SetPlayerOwnership(-1);

        transform.position = _ballStartPosition;
        _myRb.velocity = Random.insideUnitCircle.normalized * _speed;
        if(_myRb.velocity.y > 0)
        {
            _myRb.velocity.Scale(Vector2.down);
        }
        if(Mathf.Abs(_myRb.velocity.normalized.y) < (Mathf.Abs(_myRb.velocity.normalized.x) + _minYOffsetVelocity))
        {
            _myRb.velocity = new Vector2(_myRb.velocity.x, -(Mathf.Abs(_myRb.velocity.normalized.x) + _minYOffsetVelocity));
        }
    }

    public void IncreaseSpeed(int speedIncrement)
    {
        _speed = Mathf.Min(_speed + speedIncrement, _maxSpeed);
    }

    public void SetPlayerOwnership(int playerID)
    {
        _ownedByPlayer = playerID;
        PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(playerID);
        if (playerInfo != null)
        {
            gameObject.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetPlayerInfo(playerID).Color;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
