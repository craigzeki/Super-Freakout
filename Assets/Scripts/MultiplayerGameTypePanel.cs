using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameTypePanel : MonoBehaviour
{
    private const int NUM_OF_MENU_ITEMS = 3;
    [SerializeField] private MenuBricks[] _menuBricks = new MenuBricks[NUM_OF_MENU_ITEMS];
    [SerializeField] private GameObject _gameSelectPanel;
    [SerializeField] private GameObject _multiPlayerDetailsPanel;
    

    private int _currentMenuItem = 0;

    private void Awake()
    {
        MoveBricksTo(_currentMenuItem);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState != GameState.MENU) return;

        if(Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            _currentMenuItem = Mathf.Clamp(_currentMenuItem + 1, 0, NUM_OF_MENU_ITEMS - 1);
            MoveBricksTo(_currentMenuItem);
        }
        else if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            _currentMenuItem = Mathf.Clamp(_currentMenuItem - 1, 0, NUM_OF_MENU_ITEMS - 1);
            MoveBricksTo(_currentMenuItem);
        }

        if(Input.GetKeyUp(KeyCode.Return))
        {
            switch (_currentMenuItem)
            {
                case 0: //Host
                    GameManager.Instance.GameMode = GameMode.HOST;
                    _multiPlayerDetailsPanel.SetActive(true);
                    _multiPlayerDetailsPanel.GetComponent<MultiplayerDetailsPanel>().ResetMenu();
                    gameObject.SetActive(false);
                    break;
                case 1: //Join
                    GameManager.Instance.GameMode = GameMode.CLIENT;
                    GameManager.Instance.SetGameState(GameState.MULTI_PLAYER_BATTLE);
                    
                    break;
                case 2: //Back
                    _gameSelectPanel.SetActive(true);
                    _gameSelectPanel.GetComponent<GameSelectPanel>().ResetMenu();
                    gameObject.SetActive(false);
                    break;

                default:
                    break;
            }
        }
    }

    private void MoveBricksTo(int menuIndex)
    {
        foreach (MenuBricks menuBricks in _menuBricks)
        {
            menuBricks.SetActive(false);
        }
        _menuBricks[menuIndex % _menuBricks.Length].SetActive(true);
    }

    public void ResetMenu()
    {
        _currentMenuItem = 0;
        MoveBricksTo(0);
    }
}
