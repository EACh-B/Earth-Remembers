using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAnimationScript : MonoBehaviour {

    GameObject bgAnimObj, mainMenuObj;

    Vector3 startpos = new Vector3(0, 500, 0);

    public float animtimer = 0;

    // Use this for initialization
    void Start () {
        mainMenuObj = GameObject.Find("Lobby_Parent");
        bgAnimObj = transform.GetChild(0).gameObject;
        bgAnimObj.transform.localPosition = startpos;
	}
	
	// Update is called once per frame
	void Update () {
        if (mainMenuObj.transform.localScale.x <= 0)
        {
            if (animtimer > 0)
                animtimer -= Time.deltaTime;
            if (animtimer <= 0)
                StartCoroutine(DoAnim());
        }

    }

    IEnumerator DoAnim()
    {
        yield return new WaitForSeconds(0.002f);
        

        bgAnimObj.transform.localPosition = new Vector3(bgAnimObj.transform.localPosition.x, bgAnimObj.transform.localPosition.y - 1, 0);

        if (bgAnimObj.transform.localPosition.y <= -800)
        {
            bgAnimObj.transform.localPosition = startpos;
            animtimer = Random.Range(9, 50);
        }
        else if(bgAnimObj.transform.localPosition.y > -800)
            StartCoroutine(DoAnim());
    }
}
