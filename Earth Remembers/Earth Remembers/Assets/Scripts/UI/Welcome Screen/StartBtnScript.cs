using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBtnScript : MonoBehaviour {
    WorldStatusController wsc;
	// Use this for initialization
	void Start () {
        wsc = GameObject.Find("Controller").GetComponent<WorldStatusController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("WelcomeScreen_Parent").transform.localScale.x > 0 && !PhotonNetwork.isMasterClient)
        {
            if (wsc.state != WorldStatusController.STATE.PREGAME)
                gameObject.transform.localScale = new Vector2(1, 1);
            else
                gameObject.transform.localScale = new Vector2(0,0);
        }
	}
}
