using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GMLogScript : MonoBehaviour {

    string logTxt;
    Text LogText;

    private void Start()
    {
        logTxt = "";

        LogText = GameObject.Find("GMTextLog_Panel").transform.GetChild(0).GetComponent<Text>();
    }

    public void SendNewLogData(string msg)
    {
        int turnNumber;
        logTxt = "";

        turnNumber = GetComponent<WorldStatusController>().turn;

        logTxt += "\n\n";
        logTxt += "Turn " + turnNumber + ": ";
        logTxt += msg;

        LogText.text += logTxt;
        print(logTxt);
    }
}
