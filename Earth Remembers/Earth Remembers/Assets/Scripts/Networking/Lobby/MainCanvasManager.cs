﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour {
    public static MainCanvasManager instance;

    [SerializeField]
    private LobbyCanvas lobbyCanvas;
    public LobbyCanvas LobbyCanvas
    {
        get { return lobbyCanvas; }
    }

    private void Awake()
    {
        instance = this;
    }
}
