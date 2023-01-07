using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Paddle _paddle;
    [SerializeField] private Ball _ball;
    [SerializeField] private BrickManager _brickManager;

    public Paddle Paddle { get => _paddle; }
    public Ball Ball { get => _ball; }
    public BrickManager BrickManager { get => _brickManager; }

}
