using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    private WorldStatusController wsc;

    private void Awake()
    {
        //Get the world status controller script
        wsc = GameObject.Find("Controller").GetComponent<WorldStatusController>();
    }

    //Called when I join a room
    void OnJoinedRoom()
    {
        print("Successfully joined room");
        //Get XML Lists
        wsc.gameObject.GetComponent<Initializer>().SetUpGameLists();
        wsc.gameObject.GetComponent<ClientUIController>().ShowPopupMessage("Successfully joined game",
            "You have successfully joined this game of Earth Remembers. There is " + wsc.playerCount + " players in the game");
        //If this instance is not the host, change view to the regular player view
        if (!PhotonNetwork.isMasterClient)
        {
            wsc.GetGameStatus();
            
            //Set up this player with assigning a country
            StartCoroutine(wsc.gameObject.GetComponent<ThisPlayerScript>().AssignNation());
            //Find out if game has started
            ChangeLayout.ChangeCurrentLayout("WelcomeScreen_Parent");

        }
        else
        {
            //Go to first tab screen
            wsc.gameObject.GetComponent<HostUiController>().HostScreenStartUp();
            //Get and set text values
            wsc.gameObject.GetComponent<HostUiController>().SetHostDisplay();
        }
    }

    //Called when failed to join room
    void OnPhotonJoinRoomFailed()
    {
        print("FAILED TO JOIN ROOM");
        GameObject.Find("Controller").GetComponent<ClientUIController>().ShowPopupMessage("Game capacity full",
            "Failed to join the game, the game is already full.");
    }

    //called when a player joins the room
    void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        //increase player count on wsc
        wsc.playerCount++;
    }

    //Called when a player leaves the room
    void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        //Decrease player count on wsc
        wsc.playerCount--;
    }
}
