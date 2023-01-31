using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Paddle : NetworkBehaviour
{
    private NetworkVariable<int> _playerIDVar = new NetworkVariable<int>();
    public struct MPPlayerData : INetworkSerializable
    {
        public int ColorIndex;
        

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ColorIndex);
            
        }
    }

    private NetworkVariable<MPPlayerData> _mPPlayerData = new NetworkVariable<MPPlayerData>(
        new MPPlayerData
        {
            ColorIndex = (int)PlayerColors.DEFAULT

        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
        );

    public int PlayerID = 0;

    [SerializeField] private float _speed = 200;

    private float _inputDir;
    private Rigidbody2D _myRB;
    private AudioSource _myAS;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _playerIDVar.Value = -1;
        _playerIDVar.OnValueChanged += SetPlayerIDEvent;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _playerIDVar.OnValueChanged -= SetPlayerIDEvent;
    }

    private void SetPlayerIDEvent(int previous, int current)
    {
        PlayerID = current;
    }

    public void SetPlayerID(int playerID)
    {
        if (GameManager.Instance.GameMode != GameMode.SINGLE) _playerIDVar.Value = playerID;
        PlayerID = playerID;
    }

    private void Awake()
    {
        _myRB = GetComponent<Rigidbody2D>();
        _myAS = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //if (!IsOwner) return;
        
    }

    private void FixedUpdate()
    {
        if ((!IsOwner) && ((GameManager.Instance.GameMode != GameMode.NOT_SET) && (GameManager.Instance.GameMode != GameMode.SINGLE))) return;
        _inputDir = Input.GetAxisRaw("Horizontal");
        if(GameManager.Instance.GameMode == GameMode.CLIENT)
        {
            _inputDir *= -1;
        }
        _myRB.velocity = Vector2.right * _speed * _inputDir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.gameObject.tag == "Ball")
        {
            collision.gameObject.GetComponent<Ball>().SetPlayerOwnership(PlayerID, gameObject.GetComponent<SpriteRenderer>().color);
            _myAS.Play();
        }
    }

    [ClientRpc]
    //private void SetColorClientRpc(int colorIndex, ClientRpcParams clientRpcParams = default)
    //{
    //    Debug.Log("ClientRPC Triggered, colorIndex: " + colorIndex);
    //    if (colorIndex >= GameManager.Instance.PlayerColors.Count) return;
    //    gameObject.GetComponent<SpriteRenderer>().color = GameManager.Instance.PlayerColors[colorIndex];
    //}
    private void SetPlayerDataClientRpc(int colorIndex, int playerID)
    {
        Debug.Log("ClientRPC Triggered, colorIndex: " + colorIndex + ", playerID: " + playerID);
        if (colorIndex >= GameManager.Instance.PlayerColors.Count) return;
        gameObject.GetComponent<SpriteRenderer>().color = GameManager.Instance.PlayerColors[colorIndex];
        PlayerID = playerID;
    }

    

    public void SetPlayerData(int colorIndex, int playerID)
    {
        if (!IsOwner) return;
        //SetColorClientRpc(colorIndex, new ClientRpcParams
        //{
        //    Send = new ClientRpcSendParams { TargetClientIds = new List<ulong>() { 1, } }
        //});
        SetPlayerDataClientRpc(colorIndex, playerID);

    }

}
