using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvas : MonoBehaviour {

    [SerializeField]
    private RoomListingLayout _roomLayoutGroup;
    private RoomListingLayout RoomLayoutGroup
    {
        get { return _roomLayoutGroup; }
    }

    GameObject hostPanel, joinPanel;
    Button joinBtn, hostBtn;

    Vector2 show, hide;

    private void Start()
    {
        hostPanel = GameObject.Find("HostSetup_Panel");
        joinPanel = GameObject.Find("JoinGame_Panel");
        hostBtn = GameObject.Find("HostGame_Btn").GetComponent<Button>();
        joinBtn = GameObject.Find("JoinGame_Btn").GetComponent<Button>();

        hostPanel.SetActive(false);
        joinPanel.transform.localScale = hide;

        hide = new Vector2(0, 0);
        show = new Vector2(1, 1);

        print(gameObject.name);
    }

    //Called when clicking a game in lobby
    public void OnClickJoinRoom(string roomName)
    {
        //Join Game
        if (PhotonNetwork.JoinRoom(roomName))
        {

        }
        //Failed to join
        else
        {
            print("Join room failed.");
        }
    }

    public void Cancel_Click()
    {
        hostPanel.SetActive(false);
        joinPanel.transform.localScale = hide;

        hostBtn.interactable = true;
        joinBtn.interactable = true;
    }

    public void Host_Click()
    {
        hostPanel.SetActive(true);

        hostBtn.interactable = false;
        joinBtn.interactable = false;
    }


    public void Join_Click()
    {
        joinPanel.transform.localScale = show;

        hostBtn.interactable = false;
        joinBtn.interactable = false;
    }
}
