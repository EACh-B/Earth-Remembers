using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TippingPointController : MonoBehaviour {

    bool coralReef, arcticIce, boreal, monsoon, antarcticIce;
    public List<float> TPChances = new List<float>(3);
    WorldStatusController wsc;

    //For getting from WSC
	bool GetCoralReef() { return coralReef; }
    bool GetArcticIce() { return arcticIce; }
    bool GetRainForest() { return boreal; }
    bool GetMonsoon() { return monsoon; }
    bool GetAntarcticIce() { return antarcticIce; }

    private void Start()
    {
        InitializeTPs();
        wsc = GetComponent<WorldStatusController>();
    }

    //For starting the game
    void InitializeTPs()
    {
        coralReef = false;
        arcticIce = false;
        boreal = false;
        monsoon = false;
        antarcticIce = false;
    }

    public void CheckTPs(float value, float CRMin, float CRMax, float ISMin, float ISMax, float RFMin, float RFMax, float monMin, float monMax, float antMin, float antMax)
    {
        if(!coralReef)
            CheckCoralReef(value, CRMin, CRMax);
        if(!arcticIce)
            CheckArcticIce(value, ISMin, ISMax);
        if(!boreal)
            CheckBoreal(value, RFMin, RFMax);
        if (!monsoon)
            CheckMonsoon(value, monMin, monMax);
        if (!antarcticIce)
            CheckAntarcticIce(value, antMin, antMax);
    }

    private void CheckAntarcticIce(float value, float min, float max)
    {
        Image img = GameObject.Find("TP2_Img").GetComponent<Image>();

        if (value >= min)
        {
            TPChances[4] = AsymptoticProbabilityMap.instance.GetProbability(value, min, max) * 100;
        }

        int rnd = UnityEngine.Random.Range(1, 100);

        if (rnd <= TPChances[4] && wsc.turn > 1)
        {
            antarcticIce= true;
            img.color = Color.red;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Passed";
        }
        else
        {
            antarcticIce = false;
            img.color = Color.green;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = TPChances[4].ToString("0.0") + "%";
        }
    }

    private void CheckMonsoon(float value, float min, float max)
    {
        Image img = GameObject.Find("TP4_Img").GetComponent<Image>();

        if (value >= min)
        {
            //The monsoon has been specified (by Manjana) to never trigger since the game's expected range is well below its minimum boundary
            TPChances[3] = 0;
        }
        monsoon = false;
        img.color = Color.green;
        img.gameObject.transform.GetChild(0).GetComponent<Text>().text = TPChances[3].ToString("0.0") + "%";
    }

    void CheckCoralReef(float value, float min, float max)
    {
        Image img = GameObject.Find("TP0_Img").GetComponent<Image>();
        //I've removed conditionality from this one because coralReef has already passed its max temperature tipping point
        //According to Manjana
        coralReef = true;
        img.color = Color.red;
        img.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Passed";

    }
    void CheckArcticIce(float value, float min, float max)
    {
        Image img = GameObject.Find("TP1_Img").GetComponent<Image>();

        if (value >= min)
        {
            TPChances[1] = AsymptoticProbabilityMap.instance.GetProbability(value,min,max) * 100;
        }

        int rnd = UnityEngine.Random.Range(1, 100);

        if (rnd <= TPChances[1] && wsc.turn > 1)
        {
            arcticIce = true;
            img.color = Color.red;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Passed";
        }
        else
        {
            arcticIce = false;
            img.color = Color.green;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = TPChances[1].ToString("0.0") + "%";
        }
    }

    void CheckBoreal(float value, float min, float max)
    {
        Image img = GameObject.Find("TP3_Img").GetComponent<Image>();

        if (value >= min)
        {
            TPChances[2] = AsymptoticProbabilityMap.instance.GetProbability(value, min, max) * 100;
        }

        int rnd = UnityEngine.Random.Range(1, 100);

        if (rnd <= TPChances[2] && wsc.turn > 1)
        {
            boreal = true;
            img.color = Color.red;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Passed";
        }
        else
        {
            boreal = false;
            img.color = Color.green;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = TPChances[2].ToString("0.0") + "%";
        }
    }
}
