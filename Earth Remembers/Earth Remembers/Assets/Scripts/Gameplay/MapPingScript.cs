using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPingScript : MonoBehaviour {
    WorldStatusController wsc;
    ClientUIController uiController;

    private void Start()
    {
        wsc = GameObject.Find("Controller").GetComponent<WorldStatusController>();
        uiController = GameObject.Find("Controller").GetComponent<ClientUIController>();
    }

    private void OnMouseDown()
    {
        print("ya");
        uiController.Click_MapPing(gameObject.name);
    }


}
