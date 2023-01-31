using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Brick : NetworkBehaviour
{
    [SerializeField] private Color[] _brickColors;
    [SerializeField] private Color _goldenBrickColor;

    private NetworkVariable<int> colorIndex = new NetworkVariable<int>();
    private NetworkVariable<bool> isGoldenBrick = new NetworkVariable<bool>();

    public int AudioIndex = 0;
    public int Points = 0;

    public BrickManager BrickManager;

    public Color[] BrickColors { get => _brickColors; }

    public override void OnNetworkSpawn()
    {
        colorIndex.Value = -1;
        isGoldenBrick.Value = false;
        colorIndex.OnValueChanged += SetColorEvent;
        isGoldenBrick.OnValueChanged += SetGoldenBrickEvent;
    }

    private void SetColorEvent(int previous, int current)
    {
        if (current < 0 || current >= _brickColors.Length) return;
        SetColor(_brickColors[current]);
    }

    private void SetGoldenBrickEvent(bool previous, bool current)
    {
        if(current)
        {
            SetColor(_goldenBrickColor);
        }
    }

    private void SetColor(Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void SetBrickData(int colorIndex, bool isGoldenBrick)
    {
        if (GameManager.Instance.GameMode != GameMode.SINGLE) this.colorIndex.Value = colorIndex;
        if (GameManager.Instance.GameMode != GameMode.SINGLE) this.isGoldenBrick.Value = isGoldenBrick;
        if (isGoldenBrick)
        {
            SetColor(_goldenBrickColor);
        }
        else
        {
            SetColor(_brickColors[colorIndex]);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        colorIndex.OnValueChanged -= SetColorEvent;
        isGoldenBrick.OnValueChanged -= SetGoldenBrickEvent;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((!IsHost) && (GameManager.Instance.GameMode != GameMode.SINGLE)) return;
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
            
            if(isGoldenBrick.Value)
            {
                SetWinClientRpc(collision.transform.GetComponent<Ball>().OwnedByPlayer);
            }    

            Destroy(gameObject);
        }
        
    }

    [ClientRpc]
    private void SetWinClientRpc(int playerID)
    {
        if (playerID == 0)
        {
            GameManager.Instance.SetMPWinMessage("Host Wins!", true);
        }
        else
        {
            GameManager.Instance.SetMPWinMessage("Client Wins!", true);
        }
    }
}
