using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    [SerializeField]
    private Text _roomName;
    private Text RoomName
    {
        get { return _roomName; }
    }

    //Clicking 'create room'
    public void OnClick_CreateRoom()
    {
        //Set options for this new room - Choose options such as max players
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 11 };

        //Create the room
        if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {
            print("create room successfully sent. " + RoomName.text );
            //Sent created room popup
            GameObject.Find("Controller").GetComponent<ClientUIController>().ShowPopupMessage("Created Game Successfully", 
                "You have successfully created a game. You are now the host of: " + RoomName.text);
        }
        //Failure
        else
            print("create room failed to send");
    }

    //If we failed to amke a new room
    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("create room failed:" + codeAndMessage[1]);
    }

    //When we succesfully create a room
    private void OnCreatedRoom()
    {
        print("room created successfuly");
        //Change view for the host 
        ChangeLayout.ChangeCurrentLayout("WelcomeScreen_Parent");
        //Set up starting values
        GameObject.Find("Controller").GetComponent<WorldStatusController>().SetUpGame();
        //This game state is now in setup
        GameStateController.STATE = GameStateController.GAMESTATE.SETUP;
    }
}
