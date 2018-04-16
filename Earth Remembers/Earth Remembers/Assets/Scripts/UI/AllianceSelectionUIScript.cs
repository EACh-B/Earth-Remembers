using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllianceSelectionUIScript : MonoBehaviour {
    GameObject allianceListingObj;

	// Use this for initialization
	void Start () {
        allianceListingObj = GameObject.Find("AllianceListing_Obj");
        allianceListingObj.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ClearList()
    {
        foreach(Transform t in allianceListingObj.transform.parent)
        {
            if (t.gameObject.activeInHierarchy)
                Destroy(t.gameObject);
        }
    }

    //Refresh the list after an alliance has been chosen
    public void RefreshListing(string[] unselectedAlliances)
    {
        print("\n\n\n\n\n jash");
        ClearList();
        GameObject newListing;
        foreach(string s in unselectedAlliances)
        {
            newListing = GameObject.Instantiate(allianceListingObj, allianceListingObj.transform.parent);


            newListing.transform.GetChild(0).GetComponent<Text>().text = s;


            newListing.SetActive(true);
        }

        AddNationsToAllianceListing();
    }

    public void AddNationsToAllianceListing()
    {
        Initializer init = GameObject.Find("Controller").GetComponent<Initializer>();
        GameObject listContainer = GameObject.Find("AllianceList_Container");
        GameObject nameText;


        foreach (Nation n in init.nationsList)
        {
            string alliance = n.alliance.ToString();
            foreach (Transform t in listContainer.transform)
            {
                if (alliance == t.GetChild(0).GetComponent<Text>().text)
                {
                    nameText = t.GetChild(1).GetChild(0).gameObject;
                    GameObject newText = GameObject.Instantiate(nameText, nameText.transform.parent);
                    newText.transform.GetComponent<Text>().text = n.name;
                }
            }
        }


        foreach (Transform t in listContainer.transform)
        {
           // t.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }
    }
}
