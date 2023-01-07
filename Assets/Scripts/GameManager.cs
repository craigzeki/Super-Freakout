using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum GameState : int
{
    MENU = 0,
    SINGLE_PLAYER,
    MULTI_PLAYER,
    QUIT,
    NUM_OF_STATES
}

public class GameManager : MonoBehaviour
{
    private static Color PLAYER_YELLOW = new Color(195, 200, 0);
    private static Color PLAYER_TEAL = new Color(0, 183, 162);

    [SerializeField] private Vector3 singlePlayerGamePosition = Vector3.zero;
    [SerializeField] private Vector3 multiplayerPlayer1GamePosition = Vector3.zero;
    [SerializeField] private Vector3 multiplayerPlayer2GamePosition = Vector3.zero;
    [SerializeField] private GameObject _gameContainerPrefab;
    [SerializeField] private Canvas _menuCanvas;
    [SerializeField] private Canvas _singlePlayerCanvas;
    
    [SerializeField] private int _numOfPlayers = 1;
    [SerializeField] private int _startingBalls = 3;

    private List<Game> _games = new List<Game>();
    private List<PlayerInfo> _playerInfo = new List<PlayerInfo>();
    private GameState _gameState = GameState.MENU;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    public GameState GameState { get => _gameState;}

    private void Awake()
    {
        _singlePlayerCanvas.enabled = false;
        _menuCanvas.enabled = true;
        _gameState = GameState.MENU;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        switch (_gameState)
        {
            case GameState.MENU:
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    SetGameState(GameState.QUIT);
                }
                break;
            case GameState.SINGLE_PLAYER:
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    SetGameState(GameState.MENU);
                }
                if(Input.GetKeyUp(KeyCode.Return))
                {
                    LoadSinglePlayer();
                }
                break;
            case GameState.MULTI_PLAYER:
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    SetGameState(GameState.MENU);
                }
                break;
            case GameState.QUIT:
                break;
            case GameState.NUM_OF_STATES:
            default:
                break;
        }


    }

    private void QuitGame()
    {
        Application.Quit();
    }

    public void SetGameState(GameState targetGameState)
    {
        switch (_gameState)
        {
            case GameState.MENU:
                switch (targetGameState)
                {
                    case GameState.SINGLE_PLAYER:
                        LoadSinglePlayer();
                        _gameState = targetGameState;
                        break;
                    case GameState.MULTI_PLAYER:
                        //Do Nothing for now
                        break;
                    case GameState.QUIT:
                        QuitGame();
                        _gameState = targetGameState;
                        break;
                }
                break;
            case GameState.SINGLE_PLAYER:
                switch (targetGameState)
                {
                    case GameState.MENU:
                        LoadMenu();
                        _gameState = targetGameState;
                        break;
                    
                    case GameState.QUIT:
                        QuitGame();
                        _gameState = targetGameState;
                        break;
                    
                }
                break;
            case GameState.MULTI_PLAYER:
                break;
            case GameState.QUIT:
                QuitGame();
                _gameState = targetGameState;
                break;
            case GameState.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void DestroyGames()
    {
        foreach (Game game in _games)
        {
            Destroy(game.gameObject);
        }
        _games.Clear();
    }

    private void DestroyPlayers()
    {
        foreach(PlayerInfo playerInfo in _playerInfo)
        {
            //do nothing for now
        }
        _playerInfo.Clear();
    }

    private void LoadSinglePlayer()
    {
        _menuCanvas.enabled = false;
        _singlePlayerCanvas.enabled = true;

        DestroyGames();
        DestroyPlayers();

        _playerInfo.Add(new PlayerInfo(PLAYER_YELLOW, "Player 1", 0, _startingBalls));

        GameObject gameContainer = Instantiate(_gameContainerPrefab, singlePlayerGamePosition, Quaternion.identity);
        gameContainer.GetComponent<Game>().Paddle.GetComponent<SpriteRenderer>().color = PLAYER_YELLOW;
        _games.Add(gameContainer.GetComponent<Game>());
        
    }

    public PlayerInfo GetPlayerInfo(int playerID)
    {
        if ((playerID > _playerInfo.Count - 1) || (playerID < 0)) return null;

        return _playerInfo[playerID];
    }

    public Game GetGameInfo(int playerID)
    {
        if((playerID > _games.Count - 1) || (playerID < 0)) return null;

        return _games[playerID];
    }

    public void GameOver()
    {
        switch (_gameState)
        {
            case GameState.MENU:
                break;
            case GameState.SINGLE_PLAYER:
                // handle switching to high score entry here
                break;
            case GameState.MULTI_PLAYER:
                break;
            case GameState.QUIT:
                break;
            case GameState.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void LoadMenu()
    {
        DestroyGames();
        _menuCanvas.enabled = true;
        _singlePlayerCanvas.enabled = false;
    }
}
