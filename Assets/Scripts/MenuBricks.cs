using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuBricks
{
    [SerializeField] public GameObject LeftBrick;
    [SerializeField] public GameObject RightBrick;

    public void SetActive(bool value)
    {
        if (LeftBrick == null) return;
        LeftBrick.GetComponent<Image>().enabled = value;

        if (RightBrick == null) return;
        RightBrick.GetComponent<Image>().enabled = value;

    }
}
