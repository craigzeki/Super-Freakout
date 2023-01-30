using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Game : NetworkBehaviour
{


    //[SerializeField] private Paddle _paddle;
    //[SerializeField] private Ball _ball;
    [SerializeField] private BrickManager _brickManager;

    [SerializeField] private GameObject _paddlePrefab;
    [SerializeField] private GameObject _paddleMPHostPrefab;
    [SerializeField] private GameObject _paddleMPClientPrefab;
    [SerializeField] private Transform _p1SpawnPoint;
    [SerializeField] private Transform _p2SpawnPoint;
    [SerializeField] private GameObject _mpBallPrefab;
    [SerializeField] private List<Transform> _mpBallSpawnPoints = new List<Transform>();

    [SerializeField] private int _startingBallsSinglePlayer = 3;

    [SerializeField] private GameObject _statusPanel;
    [SerializeField] private TextMeshProUGUI _statusText;

    private List<Paddle> _players = new List<Paddle>();
    private List<PlayerInfo> _playerInfo = new List<PlayerInfo>();

    private Paddle _hostPlayer;
    private Paddle _clientPlayer;

    private bool _mpBallSpawned = false;
    private GameObject[] _balls = new GameObject[2];

    private Vector2[] _mpBallStartingVelocities = new Vector2[2];


    //public Paddle Paddle { get => _paddle; }
    //public Ball Ball { get => _ball; }
    public BrickManager BrickManager { get => _brickManager; }
    public List<PlayerInfo> PlayerInfo { get => _playerInfo; }

    public void StartSinglePlayer()
    {
        if (_players.Count > 0) return; //player1 already exists
        GameObject player = Instantiate(_paddlePrefab, _p1SpawnPoint.position, Quaternion.identity);
        player.transform.parent = this.gameObject.transform;
        player.GetComponent<Paddle>().PlayerID = 0;
        player.GetComponent<SpriteRenderer>().color = GameManager.Instance.PlayerColors[(int)PlayerColors.YELLOW];
        _playerInfo.Add(new PlayerInfo(GameManager.Instance.PlayerColors[(int)PlayerColors.YELLOW], "Player 1", 0, _startingBallsSinglePlayer));
        _brickManager.SPResetBricks();
    }

    private void DestroyPlayers()
    {
        foreach (PlayerInfo playerInfo in _playerInfo)
        {
            //do nothing for now
        }
        _playerInfo.Clear();
    }

    public PlayerInfo GetPlayerInfo(int playerID)
    {
        if ((playerID > _playerInfo.Count - 1) || (playerID < 0)) return null;

        return _playerInfo[playerID];
    }

    public void StartMultiPlayerHost()
    {
        //if (_players.Count > 0) return; //player1 already exists
        //GameObject player = Instantiate(_paddlePrefab, _p1SpawnPoint.position, Quaternion.identity);
        //player.transform.parent = this.gameObject.transform;
        //player.GetComponent<Paddle>().PlayerID = 0;
        //player.GetComponent<SpriteRenderer>().color = PLAYER_YELLOW;
        //_playerInfo.Add(new PlayerInfo(PLAYER_YELLOW, "Host", 0, 0));

        //player = Instantiate(_paddlePrefab, _p2SpawnPoint.position, Quaternion.identity);
        //player.transform.parent = this.gameObject.transform;
        //player.GetComponent<Paddle>().PlayerID = 1;
        //player.GetComponent<SpriteRenderer>().color = PLAYER_TEAL;
        //_playerInfo.Add(new PlayerInfo(PLAYER_TEAL, "Client", 0, 0));
        CreateNewBallStartingVelocities();
        NetworkManager.Singleton.StartHost();
    }

    public void StartMultiPlayerClient()
    {
        //if (_players.Count > 0) return; //player1 already exists
        //GameObject player = Instantiate(_paddlePrefab, _p1SpawnPoint.position, Quaternion.identity);
        //player.transform.parent = this.gameObject.transform;
        //player.GetComponent<Paddle>().PlayerID = 0;
        //player.GetComponent<SpriteRenderer>().color = PLAYER_YELLOW;
        //_playerInfo.Add(new PlayerInfo(PLAYER_YELLOW, "Host", 0, 0));

        //player = Instantiate(_paddlePrefab, _p2SpawnPoint.position, Quaternion.identity);
        //player.transform.parent = this.gameObject.transform;
        //player.GetComponent<Paddle>().PlayerID = 1;
        //player.GetComponent<SpriteRenderer>().color = PLAYER_TEAL;
        //_playerInfo.Add(new PlayerInfo(PLAYER_TEAL, "Client", 0, 0));

        NetworkManager.Singleton.StartClient();
    }

    private void CreateNewBallStartingVelocities()
    {
        _mpBallStartingVelocities[0] = (UnityEngine.Random.insideUnitCircle.normalized);
        _mpBallStartingVelocities[1] = (UnityEngine.Random.insideUnitCircle.normalized);
    }

    [ServerRpc(RequireOwnership = false)] //server side, but client can request
    public void SpawnPlayersServerRpc(ulong clientId, bool isHost)
    {
        int colorIndex = (int)PlayerColors.DEFAULT;
        GameObject player;
        if (isHost)
        {
            colorIndex = (int)PlayerColors.YELLOW;
            player = (GameObject)Instantiate(_paddleMPHostPrefab, _p1SpawnPoint.position, Quaternion.identity);
            _hostPlayer = player.GetComponent<Paddle>();
            _hostPlayer.PlayerID = 0;
            player.GetComponent<SpriteRenderer>().color = GameManager.Instance.PlayerColors[colorIndex];
            
            _playerInfo.Add(new PlayerInfo(GameManager.Instance.PlayerColors[colorIndex], "Host", 0, 0));
        }
        else
        {
            //client has now joined - spawn the client, the balls and inform the client of the balls velocities
            colorIndex = (int)PlayerColors.TEAL;
            player = (GameObject)Instantiate(_paddleMPClientPrefab, _p2SpawnPoint.position, Quaternion.identity);
            _clientPlayer = player.GetComponent<Paddle>();
            _clientPlayer.PlayerID = 1;
            player.GetComponent<SpriteRenderer>().color = GameManager.Instance.PlayerColors[colorIndex];
            _playerInfo.Add(new PlayerInfo(GameManager.Instance.PlayerColors[colorIndex], "Client", 0, 0));
            _brickManager.MPResetBricks();
            GameReadyToStartClientRpc(_mpBallStartingVelocities);

        }
            
        NetworkObject netObj = player.GetComponent<NetworkObject>();
        player.SetActive(true);
        player.GetComponent<Paddle>().SetColor(colorIndex);
        netObj.SpawnAsPlayerObject(clientId, true);
        //netObj.Spawn(true);
    }

    public override void OnNetworkSpawn()
    {
        SpawnPlayersServerRpc(NetworkManager.Singleton.LocalClientId, IsHost);
        
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.SetMPStatusEnabled(true);
    }

    //private void Update()
    //{
    //    if (!IsOwner) return;

    //    if(NetworkManager.ConnectedClientsIds.Count >= 2)
    //    {
    //        if(!_mpBallSpawned)
    //        {
    //            foreach(Transform spawnPoint in _mpBallSpawnPoints)
    //            {
    //                SpawnBall(spawnPoint);
    //            }
    //            _mpBallSpawned = true;
    //        }
    //    }

    //}

    private GameObject SpawnBall(Transform spawnPoint, Vector2 startingVelocity)
    {
        if ((_mpBallPrefab != null) && (spawnPoint != null))
        {
            GameObject newBall = Instantiate(_mpBallPrefab, spawnPoint.position, Quaternion.identity);
            if (newBall != null)
            {
                newBall.GetComponent<Ball>().PreventRespawn = false;
                newBall.GetComponent<Ball>().SetPlayerOwnership(-1, GameManager.Instance.PlayerColors[(int)PlayerColors.DEFAULT]);
                newBall.GetComponent<Ball>().Respawn(true, true, startingVelocity, true);
                
                //NetworkObject netObj = newBall.GetComponent<NetworkObject>();
                //if (netObj != null) netObj.Spawn(true);
                
            }

            return newBall;
        }

        return null;
    }

    

    [ClientRpc]
    private void GameReadyToStartClientRpc(Vector2[] startingVelocities)
    {
        GameManager.Instance.SetMPStatusEnabled(false);

        if (_mpBallSpawnPoints.Count > startingVelocities.Length) return;
        for(int i = 0; i < _mpBallSpawnPoints.Count; i++)
        {
            _balls[i] = SpawnBall(_mpBallSpawnPoints[i], startingVelocities[i]);
            if (_balls[i] != null) _balls[i].GetComponent<Ball>().BallID = i;
        }
        _mpBallSpawned = true;
    }

    [ClientRpc]
    private void RespawnBallClientRpc(int ballID, Vector2 startingVelocity)
    {
        _balls[ballID].GetComponent<Ball>().Respawn(true, true, startingVelocity, true);
    }

    public void RespawnBall(int ballID)
    {
        if (!IsHost) return; //could request that host respawns - will see if needed due to lag
        _mpBallStartingVelocities[ballID] = (UnityEngine.Random.insideUnitCircle.normalized);
        RespawnBallClientRpc(ballID, _mpBallStartingVelocities[ballID]);

    }
}
