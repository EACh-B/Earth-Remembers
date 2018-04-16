using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeLayout : MonoBehaviour {

	public static void ChangeCurrentLayout(string newLayout)
    {
        int i = 0;
        foreach(Transform g in GameObject.Find("Canvas").transform)
        {
            if(i>0 && g.name != "Popup_Parent")
                g.gameObject.transform.localScale = new Vector2(0, 0);
            else
                g.gameObject.transform.localScale = new Vector3(1, 1, 1);
            i++;
        }

        GameObject.Find(newLayout).transform.localScale = new Vector2(1,1);
    }
}
