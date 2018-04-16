using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour {

    [SerializeField]
    private Text _roomNameText;
    private Text RoomNameText
    {
        get { return _roomNameText; }
    }

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

    private void Start()
    {
        //Get the lobby canvas object
        GameObject lobbyCanvasObj = MainCanvasManager.instance.LobbyCanvas.gameObject;
        if (lobbyCanvasObj == null)
            return;

        //Get lobby canvas scripts
        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();

        //Add listener to this newly spawend button
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(RoomName));

        //Set text for this button
        SetRoomNameText(RoomName);
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    //Setting text on button
    public void SetRoomNameText(string text)
    {
        RoomName = text;
        RoomNameText.text =  RoomName;
    }
}
