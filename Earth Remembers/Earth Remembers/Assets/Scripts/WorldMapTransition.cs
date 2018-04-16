using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapTransition : MonoBehaviour
{ 
    public void GoTo ()
    {
        ChangeLayout.ChangeCurrentLayout("WorldMapView_Parent");
    }
	
    public void Return()
    {
        ChangeLayout.ChangeCurrentLayout("ClientView_Parent");
    }
    
}
