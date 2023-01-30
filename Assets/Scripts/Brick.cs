using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Brick : NetworkBehaviour
{
    [SerializeField] private Color[] _brickColors;

    private NetworkVariable<int> colorIndex = new NetworkVariable<int>();

    public int AudioIndex = 0;
    public int Points = 0;

    public BrickManager BrickManager;

    public Color[] BrickColors { get => _brickColors; }

    public override void OnNetworkSpawn()
    {
        colorIndex.Value = -1;
        colorIndex.OnValueChanged += SetColorEvent;
    }

    private void SetColorEvent(int previous, int current)
    {
        if (current < 0 || current >= _brickColors.Length) return;
        SetColor(_brickColors[current]);
    }

    private void SetColor(Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void SetColorIndex(int colorIndex)
    {
        if(GameManager.Instance.GameMode != GameMode.SINGLE) this.colorIndex.Value = colorIndex;
        SetColor(_brickColors[colorIndex]);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        colorIndex.OnValueChanged -= SetColorEvent;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsHost) return;
        if(collision.transform.tag == "Ball")
        {
            if (collision.transform.GetComponent<Ball>().OwnedByPlayer == -1) return;

            if(BrickManager != null)
            {
                //this.gameObject.GetComponentInParent<BrickManager>().PlayBrickSound(AudioIndex);
                //this.gameObject.GetComponentInParent<BrickManager>().RemoveBrick(this, collision.transform.GetComponent<Ball>());

                BrickManager.PlayBrickSound(AudioIndex);
                BrickManager.RemoveBrick(this, collision.transform.GetComponent<Ball>());
            }


            PlayerInfo playerInfo = GameManager.Instance.CurrentGame.GetPlayerInfo(collision.transform.GetComponent<Ball>().OwnedByPlayer);
            if(playerInfo != null)
            {
                playerInfo.Points += (Points * collision.transform.GetComponent<Ball>().Speed);
            }
            
            Destroy(gameObject);
        }
        
    }
}
