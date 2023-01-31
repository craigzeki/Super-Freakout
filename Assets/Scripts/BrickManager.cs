using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class BrickManager : NetworkBehaviour
{

    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private float _spacing;
    [SerializeField] private GameObject _brickPrefab;
    [SerializeField] private GameObject _mpBrickPrefab;
    //[SerializeField] private Color[] _brickColors;
    [SerializeField] private AudioClip[] _brickSounds;
    [SerializeField] private int _speedIncrement = 1;
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Transform _ballSpawnPoint;

    private List<GameObject> bricks = new List<GameObject>();

    private AudioSource _myAS;
    private int _levelCount = 0;

    public int SpeedIncrement { get => _speedIncrement; }
    //public Color[] BrickColors { get => _brickColors; }

    private void Awake()
    {
        _myAS = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _levelCount= 0;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SPResetBricks()
    {
        foreach (GameObject brick in bricks)
        {
            Destroy(brick);
        }
        bricks.Clear();

        for (int x = 0; x < _columns; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                Vector2 spawnPos = (Vector2)transform.position + new Vector2(
                    x * (_brickPrefab.transform.localScale.x + _spacing),
                    -y * (_brickPrefab.transform.localScale.y + _spacing)
                    );
                GameObject brick = Instantiate(_brickPrefab, spawnPos, Quaternion.identity);
                //brick.transform.parent = transform;
                
                
                Brick mBrick = brick.GetComponent<Brick>();
                mBrick.AudioIndex = y % _brickSounds.Length;
                
                mBrick.Points = _rows - y;
                mBrick.BrickManager = this;
                mBrick.SetBrickData(y % mBrick.BrickColors.Length, false);
                bricks.Add(brick);
            }
        }

    }

    public void MPResetBricks()
    {
        if (!IsHost) return;
        foreach (GameObject brick in bricks)
        {
            Destroy(brick);
        }
        bricks.Clear();

        int[] brickColors = new int[_columns * _rows];
        int brickColorsIndex = 0;
        bool isGoldenBrick = false;

        for (int x = 0; x < _columns; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                Vector2 spawnPos = (Vector2)transform.position + new Vector2(
                    x * (_brickPrefab.transform.localScale.x + _spacing),
                    -y * (_brickPrefab.transform.localScale.y + _spacing)
                    );
                GameObject brick = Instantiate(_mpBrickPrefab, spawnPos, Quaternion.identity);
                
                
                Brick mBrick = brick.GetComponent<Brick>();
                mBrick.AudioIndex = y % _brickSounds.Length;
                brickColors[brickColorsIndex] = y % mBrick.BrickColors.Length;

                
                mBrick.Points = _rows - y;
                mBrick.BrickManager = this;
                
                brick.SetActive(true);
                NetworkObject netObj = brick.GetComponent<NetworkObject>();
                netObj.Spawn(true);
                //netObj.TrySetParent(transform);
                
                isGoldenBrick = ((x == 5) && (y == 2));
                //must set the color after the network object has been spawned as it is a network variable
                mBrick.SetBrickData(brickColors[brickColorsIndex], isGoldenBrick);
                brickColorsIndex++;
                bricks.Add(brick);
            }
        }
        SetBrickDataClientRpc(brickColors);
        
    }

    //public void RemoveBrick(Brick brick, out bool wasLastBrick)
    public void RemoveBrick(Brick brick, Ball ball)
    {
        //wasLastBrick = false;
        bricks.Remove(brick.gameObject);

        if (bricks.Count == 0)
        {
            //wasLastBrick = true;
            _levelCount++;
            SPResetBricks();
            
            ball.IncreaseSpeed(SpeedIncrement);
            //spawn additional ball
            if ((_ballPrefab != null) && (_ballSpawnPoint != null))
            {
                GameObject newBall = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);
                if(newBall != null)
                {
                    newBall.GetComponent<Ball>().IncreaseRespawnSpeed(SpeedIncrement * _levelCount);
                    GameManager.Instance.CurrentGame.GetPlayerInfo(ball.OwnedByPlayer).Balls++;
                    newBall.GetComponent<Ball>().PreventRespawn= true;
                    newBall.GetComponent<Ball>().SetPlayerOwnership(ball.OwnedByPlayer, ball.GetComponent<SpriteRenderer>().color);
                }
            }
            
        }
    }

    public void PlayBrickSound(int soundIndex)
    {
        if (soundIndex < 0 || soundIndex > _brickSounds.Length - 1) return;

        _myAS.clip = _brickSounds[soundIndex];
        _myAS.Play();
    }

    [ClientRpc]
    private void SetBrickDataClientRpc(int[] colourIDs)
    {
        if(colourIDs.Length > bricks.Count) return;

        for(int i = 0; i < colourIDs.Length; i++)
        {
            bricks[i].GetComponent<Brick>().BrickManager = this;
            bricks[i].GetComponent<Brick>().SetBrickData(colourIDs[i], (i == 27));
            
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach(GameObject brick in bricks)
        {
            Destroy(brick);
        }
    }
}
