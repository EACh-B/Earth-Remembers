using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkManagerScript : MonoBehaviour {
    public int playerCount = 0;
    public Text playerCountTxt;
    // Use this for initialization

    private void Update()
    {
        playerCountTxt.text = "Players: " + playerCount + "/10";
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + playerCount + " connected from " + player.ipAddress + ":" + player.port);
    }

}
