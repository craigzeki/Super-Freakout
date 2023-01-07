using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SinglePlayerCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private TextMeshProUGUI _ballsText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    private void OnGUI()
    {
        if (GameManager.Instance.GameState != GameState.SINGLE_PLAYER) return;
        PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(0);
        if (playerInfo == null) return;
        _pointsText.text = "Points: " + playerInfo.Points.ToString("00000000");
        if(playerInfo.Balls > 0)
        {
            _ballsText.text = "Balls: " + playerInfo.Balls.ToString();
        }
        else
        {
            _ballsText.text = "GAME OVER";
        }
        
    }
}
