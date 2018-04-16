using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour {
    //List of nations stored, list of tipping points stored
    public List<Nation> nationsList = new List<Nation>();
    public List<TippingPoint> tPList = new List<TippingPoint>();
    //Doc reader script for reference
    XMLDocReader docReader;

    //Starting values are initialized in the game start 
    public float startingEmissions = 650000, startingTemp = 1;



	// Use this for initialization
	void Start () {
        //Change view to lobby view
        ChangeLayout.ChangeCurrentLayout("Lobby_Parent");
        //Get the doc reader script component
        docReader = GetComponent<XMLDocReader>();
	}

    public void SetUpGameLists()
    {
        
        //Get the nation list from the XML reader
        nationsList = docReader.ParseNationsXML(GetComponent<XMLDocReader>().nationsData);
        //Get tipping point list
        tPList = docReader.ParseTPXML(GetComponent<XMLDocReader>().tPData);

        GetComponent<WorldStatusController>().AssignLists();
        foreach (Nation n in nationsList)
        {
            Debug.Log("Setting AI to true");
            n.AIControlled = true;
        }
    }

}
