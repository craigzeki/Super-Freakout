using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class BrickManager : MonoBehaviour
{

    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private float _spacing;
    [SerializeField] private GameObject _brickPrefab;
    [SerializeField] private Color[] _brickColors;
    [SerializeField] private AudioClip[] _brickSounds;
    [SerializeField] private int _speedIncrement = 1;

    private List<GameObject> bricks = new List<GameObject>();

    private AudioSource _myAS;

    public int SpeedIncrement { get => _speedIncrement; }

    private void Awake()
    {
        _myAS = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetBricks();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetBricks()
    {
        foreach (GameObject brick in bricks)
        {
            Destroy(brick);
        }
        bricks.Clear();

        for (int x = 0; x < _columns; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                Vector2 spawnPos = (Vector2)transform.position + new Vector2(
                    x * (_brickPrefab.transform.localScale.x + _spacing),
                    -y * (_brickPrefab.transform.localScale.y + _spacing)
                    );
                GameObject brick = Instantiate(_brickPrefab, spawnPos, Quaternion.identity);
                brick.transform.parent = transform;
                brick.GetComponent<SpriteRenderer>().color = _brickColors [y % _brickColors.Length];
                brick.GetComponent<Brick>().AudioIndex = y % _brickSounds.Length;
                brick.GetComponent<Brick>().Points = _rows - y;
                bricks.Add(brick);
            }
        }

    }

    public void RemoveBrick(Brick brick, out bool wasLastBrick)
    {
        wasLastBrick = false;
        bricks.Remove(brick.gameObject);

        if (bricks.Count == 0)
        {
            wasLastBrick = true;
            ResetBricks();
        }
    }

    public void PlayBrickSound(int soundIndex)
    {
        if (soundIndex < 0 || soundIndex > _brickSounds.Length - 1) return;

        _myAS.clip = _brickSounds[soundIndex];
        _myAS.Play();
    }
}
