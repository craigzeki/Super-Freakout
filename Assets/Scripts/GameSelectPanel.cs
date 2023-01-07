using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectPanel : MonoBehaviour
{
    private const int NUM_OF_MENU_ITEMS = 3;
    [SerializeField] private MenuBricks[] _menuBricks = new MenuBricks[NUM_OF_MENU_ITEMS];

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
                case 0: //Single Player
                    GameManager.Instance.SetGameState(GameState.SINGLE_PLAYER);
                    break;
                case 1: //Multi Player
                    GameManager.Instance.SetGameState(GameState.MULTI_PLAYER);
                    break;
                case 2: //Quit
                    GameManager.Instance.SetGameState(GameState.QUIT);
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
}
