using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ball")
        {
            

            PlayerInfo playerInfo = GameManager.Instance.GetPlayerInfo(collision.gameObject.GetComponent<Ball>().OwnedByPlayer);
            if(playerInfo != null)
            {
                playerInfo.Balls = Mathf.Clamp(playerInfo.Balls - 1, 0, playerInfo.Balls);
                if(playerInfo.Balls == 0)
                {
                    collision.gameObject.GetComponent<Ball>().gameObject.SetActive(false);
                    GameManager.Instance.GameOver();
                }
                else
                {
                    if(collision.gameObject.GetComponent<Ball>().PreventRespawn)
                    {
                        Destroy(collision.gameObject);
                    }
                    else
                    {
                        collision.gameObject.GetComponent<Ball>().Respawn(true);
                    }
                    
                }
            }
            else
            {
                collision.gameObject.GetComponent<Ball>().Respawn(true);
            }
        }
    }
}
