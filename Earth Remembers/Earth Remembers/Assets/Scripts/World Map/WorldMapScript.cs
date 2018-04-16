using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapScript : MonoBehaviour {

    bool mapView;
    string hitObj;
    List<Nation> toShow;

	// Use this for initialization
	void Start () {
        mapView = false;
	}
	
	// Update is called once per frame
	void Update () {
        //if (GameObject.Find("WorldMapView_Parent").transform.localScale.x > 0)
        //    mapView = true;
        //else
        //    mapView = false;
    }

    private void OnMouseOver()
    {
        transform.localPosition = new Vector3(0, 50, 0);
    }
    private void OnMouseExit()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
