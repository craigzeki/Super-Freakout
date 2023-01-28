using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Brick : MonoBehaviour
{
    public int AudioIndex = 0;
    public int Points = 0;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ball")
        {
            if (collision.transform.GetComponent<Ball>().OwnedByPlayer == -1) return;

            this.gameObject.GetComponentInParent<BrickManager>().PlayBrickSound(AudioIndex);
            this.gameObject.GetComponentInParent<BrickManager>().RemoveBrick(this, collision.transform.GetComponent<Ball>());

            PlayerInfo playerInfo = GameManager.Instance.CurrentGame.GetPlayerInfo(collision.transform.GetComponent<Ball>().OwnedByPlayer);
            if(playerInfo != null)
            {
                playerInfo.Points += (Points * collision.transform.GetComponent<Ball>().Speed);
            }
            
            Destroy(gameObject);
        }
        
    }
}
