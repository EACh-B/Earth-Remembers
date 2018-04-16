using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThisEvent : MonoBehaviour {
    
    public EventsScript eventsScript;
    public bool cardBack, cooldown;
    public EventsScript.WorldEvent thisWorldEvent;

    public void Start()
    {
        cardBack = false;
        cooldown = false;
    }

    private void Update()
    {

    }

    public void FlipCard()
    {
        if (!cooldown)
        {
            print("flip click");
            if (cardBack)
                cardBack = false;
            else
                cardBack = true;
            
            StartCoroutine(RotateCard());
        }

        cooldown = true;
        StartCoroutine(CoolDown());
    }

    IEnumerator RotateCard()
    {
        yield return new WaitForSeconds(0.1f);
        print("rotating card");


        transform.GetComponent<RectTransform>().Rotate(0, 50, 0);
        yield return new WaitForSeconds(0.1f);
        transform.GetComponent<RectTransform>().Rotate(0, 50, 0);

        if (cardBack)
        {
            transform.GetChild(5).transform.localScale = new Vector2(1, 1);
        }
        else
            transform.GetChild(5).transform.localScale = new Vector2(0, 1);
        yield return new WaitForSeconds(0.1f);
        transform.GetComponent<RectTransform>().Rotate(0, 80, 0);
        transform.GetComponent<RectTransform>().Rotate(0, -180, 0);
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        cooldown = false;
    }
}
