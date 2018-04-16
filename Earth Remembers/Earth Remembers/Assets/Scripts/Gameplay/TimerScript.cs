using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {
    public float currentTime = 0; //The value used as teh timer
    WorldStatusController wsc; //status controleer
    public bool ABC, paused;

	// Use this for initialization
	void Start () {
        wsc = GetComponent<WorldStatusController>();
        ABC = false;
        paused = false;
	}
	
	// Update is called once per frame
	void Update () {
        CallTimer();
        if (currentTime < 0)
        {
            currentTime = 0;
            GameObject.Find("Time_IField").GetComponent<InputField>().text = currentTime.ToString("0.00");
        }
	}

    void CallTimer()
    {
        if (wsc.state == WorldStatusController.STATE.PLAYING)
        {
            if (currentTime > 0)
            {
                if (!paused)
                {
                    currentTime -= Time.deltaTime / 1;
                    GameObject.Find("Time_IField").GetComponent<InputField>().text = currentTime.ToString("0.00");
                }
            }
            else
            {
                if (ABC)
                {
                    GetComponent<PhotonView>().RPC("LockInNetworkPlayers", PhotonTargets.All);
                    ABC = false;
                }
            }
        }
        
    }

    public void StartTimer()
    {
        string text = GameObject.Find("Time_IField").GetComponent<InputField>().text;
        currentTime = float.Parse(text);
        ABC = true;
    }

    public void Timer_Click()
    {
        paused = !paused;
    }

    [PunRPC]
    public void LockInNetworkPlayers()
    {
        wsc.Click_SubmitSpending();
    }
}
