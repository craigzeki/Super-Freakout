using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static Color PLAYER_YELLOW = new Color(195, 200, 0);
    private static Color PLAYER_TEAL = new Color(0, 183, 162);

    //[SerializeField] private Paddle _paddle;
    //[SerializeField] private Ball _ball;
    [SerializeField] private BrickManager _brickManager;

    [SerializeField] private GameObject _paddlePrefab;
    [SerializeField] private Transform _p1SpawnPoint;
    [SerializeField] private Transform _p2SpawnPoint;

    [SerializeField] private int _startingBallsSinglePlayer = 3;

    private List<Paddle> _players = new List<Paddle>();
    private List<PlayerInfo> _playerInfo = new List<PlayerInfo>();

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
        player.GetComponent<SpriteRenderer>().color = PLAYER_YELLOW;
        _playerInfo.Add(new PlayerInfo(PLAYER_YELLOW, "Player 1", 0, _startingBallsSinglePlayer));
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
        if (_players.Count > 0) return; //player1 already exists
        GameObject player = Instantiate(_paddlePrefab, _p1SpawnPoint.position, Quaternion.identity);
        player.transform.parent = this.gameObject.transform;
        player.GetComponent<Paddle>().PlayerID = 0;
        player.GetComponent<SpriteRenderer>().color = PLAYER_YELLOW;
        _playerInfo.Add(new PlayerInfo(PLAYER_YELLOW, "Host", 0, 0));

        player = Instantiate(_paddlePrefab, _p2SpawnPoint.position, Quaternion.identity);
        player.transform.parent = this.gameObject.transform;
        player.GetComponent<Paddle>().PlayerID = 1;
        player.GetComponent<SpriteRenderer>().color = PLAYER_TEAL;
        _playerInfo.Add(new PlayerInfo(PLAYER_TEAL, "Client", 0, 0));


    }

    public void StartMultiPlayerClient()
    {
        if (_players.Count > 0) return; //player1 already exists
        GameObject player = Instantiate(_paddlePrefab, _p1SpawnPoint.position, Quaternion.identity);
        player.transform.parent = this.gameObject.transform;
        player.GetComponent<Paddle>().PlayerID = 0;
        player.GetComponent<SpriteRenderer>().color = PLAYER_YELLOW;
        _playerInfo.Add(new PlayerInfo(PLAYER_YELLOW, "Host", 0, 0));

        player = Instantiate(_paddlePrefab, _p2SpawnPoint.position, Quaternion.identity);
        player.transform.parent = this.gameObject.transform;
        player.GetComponent<Paddle>().PlayerID = 1;
        player.GetComponent<SpriteRenderer>().color = PLAYER_TEAL;
        _playerInfo.Add(new PlayerInfo(PLAYER_TEAL, "Client", 0, 0));
    }
}
