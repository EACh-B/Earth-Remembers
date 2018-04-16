using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour {

	public enum GAMESTATE
    {
        SETUP,
        PLAYING
    }

    [SerializeField]
    public static GAMESTATE STATE;
}
