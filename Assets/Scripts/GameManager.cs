using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;



public enum GameState : int
{
    MENU = 0,
    SINGLE_PLAYER,
    MULTI_PLAYER_H2H,
    MULTI_PLAYER_BATTLE,
    QUIT,
    NUM_OF_STATES
}

public enum GameMode : int
{
    HOST = 0,
    CLIENT,
    SINGLE,
    NOT_SET,
    NUM_OF_MODES
}

public enum PlayerColors : int
{
    DEFAULT = 0,
    YELLOW,
    TEAL,
    NUM_OF_COLORS
}

public class GameManager : MonoBehaviour
{


    [SerializeField] private Vector3 singlePlayerGamePosition = Vector3.zero;
    [SerializeField] private Vector3 multiplayerPlayer1GamePosition = Vector3.zero;
    [SerializeField] private Vector3 multiplayerPlayer2GamePosition = Vector3.zero;
    [SerializeField] private GameObject _gameContainerPrefab;
    [SerializeField] private GameObject _gameContainerMultiBattlePrefab;
    [SerializeField] private Canvas _menuCanvas;
    [SerializeField] private Canvas _singlePlayerCanvas;
    [SerializeField] private Canvas _multiPlayerCanvas;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _mpStatusPanel;
    [SerializeField] private TextMeshProUGUI _mpStatusText;

    [SerializeField] private List<Color> _playerColors = new List<Color>();

    //[SerializeField] private int _numOfPlayers = 1;
    //[SerializeField] private int _startingBalls = 3;

    //private List<Game> _games = new List<Game>();

    private Game _currentGame = null;
    private GameState _gameState = GameState.MENU;
    private GameMode _gameMode = GameMode.NOT_SET;

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
    public GameMode GameMode
    {
        get => _gameMode;
        
        set
        {
            //can only be set externally to one of the two valid options, host or client
            if(value >= GameMode.NOT_SET) return;
            _gameMode = value;
        }
    }

    public Game CurrentGame { get => _currentGame;  }
    public List<Color> PlayerColors { get => _playerColors;  }

    private void Awake()
    {
        _playerColors.Add(Color.white); //PLAYER_DEFAULT
        _playerColors.Add(new Color(195, 200, 0)); //PLAYER_YELLOW
        _playerColors.Add(new Color(0, 183, 162)); //PLAYER_TEAL

        _singlePlayerCanvas.enabled = false;
        _multiPlayerCanvas.enabled = false;
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
            case GameState.MULTI_PLAYER_H2H:
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    SetGameState(GameState.MENU);
                }
                break;
            case GameState.MULTI_PLAYER_BATTLE:
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    NetworkManager.Singleton.Shutdown();
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
                    case GameState.MULTI_PLAYER_H2H:
                        //Do Nothing for now
                        break;
                    case GameState.MULTI_PLAYER_BATTLE:
                        if(LoadMultiPlayerBattle()) _gameState = targetGameState;
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
            case GameState.MULTI_PLAYER_H2H:
                break;
            case GameState.MULTI_PLAYER_BATTLE:
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
            case GameState.QUIT:
                QuitGame();
                _gameState = targetGameState;
                break;
            case GameState.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void DestroyGame()
    {
        if (_currentGame == null) return;
        Destroy(_currentGame.gameObject);
        _currentGame = null;
    }



    private void LoadSinglePlayer()
    {
        _menuCanvas.enabled = false;
        _multiPlayerCanvas.enabled = false;
        _singlePlayerCanvas.enabled = true;

        DestroyGame();
        _gameMode = GameMode.SINGLE;


        GameObject gameContainer = Instantiate(_gameContainerPrefab, singlePlayerGamePosition, Quaternion.identity);
        gameContainer.GetComponent<Game>().StartSinglePlayer();
        _currentGame = gameContainer.GetComponent<Game>();
        
    }

    private bool LoadMultiPlayerBattle()
    {
        if (_gameMode >= GameMode.NOT_SET) return false;
        _menuCanvas.enabled = false;
        _multiPlayerCanvas.enabled = true;
        _singlePlayerCanvas.enabled = false;

        DestroyGame();

        //create new multiplayer game
        GameObject gameContainer = Instantiate(_gameContainerMultiBattlePrefab, singlePlayerGamePosition, Quaternion.identity);
        _currentGame = gameContainer.GetComponent<Game>();

        if (_gameMode == GameMode.HOST)
        {
            _currentGame.StartMultiPlayerHost();
        }
        else if(_gameMode == GameMode.CLIENT)
        {
            _camera.transform.rotation = Quaternion.Euler(0, 0, 180);
            _currentGame.StartMultiPlayerClient();

        }

        return true;
    }

    //public PlayerInfo GetPlayerInfo(int playerID)
    //{
    //    if ((playerID > _playerInfo.Count - 1) || (playerID < 0)) return null;

    //    return _playerInfo[playerID];
    //}

    //public Game GetGameInfo(int playerID)
    //{
    //    if ((playerID > _games.Count - 1) || (playerID < 0)) return null;

    //    return _games[playerID];
    //}

    public void GameOver()
    {
        switch (_gameState)
        {
            case GameState.MENU:
                break;
            case GameState.SINGLE_PLAYER:
                // handle switching to high score entry here
                break;
            case GameState.MULTI_PLAYER_H2H:
                break;
            case GameState.MULTI_PLAYER_BATTLE:
                //NetworkManager.Singleton.Shutdown();
                break;
            case GameState.QUIT:
                break;
            case GameState.NUM_OF_STATES:
            default:
                break;
        }
    }

    public void SetMPWinMessage(string message, bool gameOver)
    {
        _mpStatusText.text = message;
        _mpStatusPanel.SetActive(true);
        if (gameOver) GameOver();
    }

    private void LoadMenu()
    {
        _camera.transform.rotation = Quaternion.Euler(0, 0, 0);
        DestroyGame();
        _menuCanvas.enabled = true;
        _singlePlayerCanvas.enabled = false;
        _multiPlayerCanvas.enabled = false;

        //_multiPlayerMode = MultiPlayerMode.NOT_SET;
    }

    public void SetMPStatusEnabled(bool enabled)
    {
        _mpStatusPanel.SetActive(enabled);
    }

    public void SetMPStatusText(string statusText)
    {
        _mpStatusText.text = statusText;
    }
}
