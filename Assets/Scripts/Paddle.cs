using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Paddle : MonoBehaviour
{
    public int PlayerID = 0;

    [SerializeField] private float _speed = 200;

    private float _inputDir;
    private Rigidbody2D _myRB;
    private AudioSource _myAS;

    private void Awake()
    {
        _myRB = GetComponent<Rigidbody2D>();
        _myAS = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _inputDir = Input.GetAxisRaw("Horizontal");

        //if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        //{
        //    playerDirection += -1;
        //}

        //if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        //{
        //    playerDirection += 1;
        //}

        
    }

    private void FixedUpdate()
    {
        _myRB.velocity = Vector2.right * _speed * _inputDir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.gameObject.tag == "Ball")
        {
            collision.gameObject.GetComponent<Ball>().SetPlayerOwnership(PlayerID);
            _myAS.Play();
        }
    }

}
