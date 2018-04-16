using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetupScript : NetworkBehaviour {



    private void OnConnectedToServer()
    {
        CmdAddToPlayerCount();
    }

    [Command]
    void CmdAddToPlayerCount()
    {
        WorldStatusController wsController = GameObject.Find("Controller").GetComponent<WorldStatusController>();
        wsController.playerCount++;
    }
}
