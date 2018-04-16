using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        print("Connecting to server..");
        //Connect to Photon Network
        PhotonNetwork.ConnectUsingSettings("0.0.0");

        
	}

    //Called when connected to server
    private void OnConnectedToMaster()
    {
        print("Connected to master.");
        //Player name for this instance
        PhotonNetwork.playerName = PlayerNetwork.Instance.playerName;

        //Join the lobby
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    //Called when joining lobby
    private void OnJoinedLobby()
    {
        print("Joined Lobby");
    }
}
