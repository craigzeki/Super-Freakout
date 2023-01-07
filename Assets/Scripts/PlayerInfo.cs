using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public Color Color;
    public string Name;
    public int Points;
    public int Balls;

    public PlayerInfo(Color color, string name, int points, int balls)
    {
        Color = color;
        Name = name;
        Points = points;
        Balls = balls;
    }
}
