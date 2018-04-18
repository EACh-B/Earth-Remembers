using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;


public class TippingPointController : MonoBehaviour {

    bool coralReef, arcticIce, boreal, monsoon, antarcticIce;
    public List<float> TPChances = new List<float>(3);
    /// <summary>
    /// Floor value for probability.
    /// </summary>
    public float baseProb = 0;
    /// <summary>
    /// Affects the abruptness of the transition from low probability to high. Between 0 and 1.
    /// This is the primary tweakable variable in this.
    /// If we want separate slopes for different environmental issues this will require refactoring.
    /// </summary>
    public float slope = 0.5f;
    /// <summary>
    /// The number of data points we want to include in this mapping. Set this high enough that the probabilities are acceptably approximated
    /// Probabilities are calculated in these discrete steps. Continuous temperature values are binned into their closest discrete step
    /// </summary>
    public int stepCount = 50;

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
            TPChances[4] = GetProbability(value,min,max) * 100;
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
            TPChances[3] = GetProbability(value, min, max) * 100;
        }

        int rnd = UnityEngine.Random.Range(1, 100);

        if (rnd <= TPChances[3] && wsc.turn > 1)
        {
            monsoon = true;
            img.color = Color.red;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Passed";
        }
        else
        {
            monsoon = false;
            img.color = Color.green;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = TPChances[3].ToString("0.0") + "%";
        }
    }

    void CheckCoralReef(float value, float min, float max)
    {
        Image img = GameObject.Find("TP0_Img").GetComponent<Image>();

        if(value >= min)
        {
            TPChances[0] = GetProbability(value, min, max) * 100;
        }

        int rnd = UnityEngine.Random.Range(1, 100);

        if (rnd <= TPChances[0])
        {
            coralReef = true;
            img.color = Color.red;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Passed";
        }
        else
        {
            coralReef = false;
            img.color = Color.green;
            img.gameObject.transform.GetChild(0).GetComponent<Text>().text = TPChances[0].ToString("0.0") + "%";
        }
    }
    void CheckArcticIce(float value, float min, float max)
    {
        Image img = GameObject.Find("TP1_Img").GetComponent<Image>();

        if (value >= min)
        {
            TPChances[1] = GetProbability(value, min, max) * 100;
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
            TPChances[2] = GetProbability(value, min, max) * 100;
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

    /////Ross' main addition to this class

    public float GetProbability(float currentWorldTemp, float minTemp, float maxTemp)
    {
        //If we exceed the graph boundaries this will asymptote
        if (currentWorldTemp >= maxTemp) { return 1.0f; };
        if (currentWorldTemp <= minTemp) { return 0.0f; };
        float range = maxTemp - minTemp;
        if (minTemp > maxTemp)
        {
            throw new System.Exception
                ("GenerateRampedThreshold arg error: minimum value is greater than maximum. Min: " + minTemp.ToString("0.00") + " Max: " + maxTemp.ToString("0.00"));
        }

        List<float> temperatures = fspan(minTemp, maxTemp, this.stepCount);
        if (currentWorldTemp >= temperatures.Last()) { return 1.0f; };
        if (currentWorldTemp <= temperatures.First()) { return 0.0f; };

        //ramp = ((temperatures-tmin) + abs(temperatures-tmin))/2.
        List<float> ramp = GenerateRamp(temperatures, minTemp);
        //ramped_thresh = ramp / (tmax - tmin)
        List<float> rampedThresholds = ramp.Select(i => i / range).ToList<float>();
        //tanh_ramp = tanh((ramped_thresh + baseprob) / slope)
        List<float> tanhRamp = rampedThresholds.Select(i => (float)Math.Tanh((i + baseProb) / slope)).ToList<float>();
        //Get the closest index for the temperature so we can use that index in the same sized list of probabilities(tanhRamp)
        int probabilityIndex = GetApproximatedTemperatureIndex(currentWorldTemp, temperatures);
        var dic = temperatures.Select((k, i) => new { k, v = tanhRamp[i] })
              .ToDictionary(x => x.k, x => x.v);
        return tanhRamp[probabilityIndex];
    }
    /// <summary>
    /// Returns the index of the closest value to the provided world temperature within the list of temperatures.
    /// </summary>
    /// <param name="currentWorldTemp">Temperature we want to approximate</param>
    /// <param name="temperatures"></param>
    /// <returns>the index of the closest value to the provided world temperature</returns>
    private int GetApproximatedTemperatureIndex(float currentWorldTemp, List<float> temperatures)
    {
        int closestIndex = 0;
        //Assert the world temp is within the temperature bounds
        if (!(currentWorldTemp >= temperatures.First() && currentWorldTemp <= temperatures.Last()))
        { throw new System.Exception("Passed in value outside the range of temperatures"); } //This should never happen as we filter out-of-range temps earlier
        if (currentWorldTemp == temperatures.First()) { return 0; }
        for (int i = 1; i < temperatures.Count; ++i)
        {
            if (currentWorldTemp <= temperatures[i])
            {
                if (currentWorldTemp == temperatures[i]) { return i; }
                float a = temperatures[i];
                float b = temperatures[i - 1];
                float deltaA = Mathf.Abs(a - currentWorldTemp);
                float deltaB = Mathf.Abs(b - currentWorldTemp);
                closestIndex = deltaA < deltaB ? i : (i + 1);
                return closestIndex;
            }
        }
        return closestIndex;
    }
    /// <summary>
    /// Implementation of fspan, which returns an evenly spaced list of values stepCount+1 long, between min and max inclusive
    /// </summary>
    /// <param name="min">Lower bound value must be < max</param>
    /// <param name="max">Upper bound value must be > min</param>
    /// <param name="stepCount">'Resolution' of the steps from min->max</param>
    /// <returns>an evenly spaced list of values stepCount+1 long, between min and max inclusive</returns>
    private static List<float> fspan(float min, float max, int stepCount)
    {
        if (min > max) { throw new System.Exception("fspan arg error: minimum value is greater than maximum. Min: " + min.ToString("0.00") + " Max: " + max.ToString("0.00")); }
        if (stepCount <= 0) { throw new System.Exception("stepCount must be greater than 0. Provided stepcount: " + stepCount.ToString()); }
        List<float> spanList = new List<float>();
        float stepSize = (max - min) / (float)stepCount;
        for (int i = 0; i <= stepCount; i++)
        {
            spanList.Add(min + (float)i * stepSize);
        }
        return spanList;
    }
    /// <summary>
    /// Moved to its own method because, assuming .NET 3.5, this line of the formula is quite verbose.
    /// ramp = ((temperatures - tmin) + abs(temperatures - tmin)) / 2.
    /// </summary>
    private List<float> GenerateRamp(List<float> temperatures, float tmin)
    {
        //ramp = ((temperatures - tmin) + abs(temperatures - tmin)) / 2.
        List<float> temperaturesMinusTMin = temperatures.Select(i => i - tmin).ToList<float>();
        List<float> abstemperaturesMinusTMin = temperaturesMinusTMin.Select(i => Mathf.Abs(i)).ToList<float>();
        //Enumerable.Zip is not supported in .NET 3.5 so performing a zip manually
        List<float> ramp = new List<float>();
        for (int i = 0; i < temperaturesMinusTMin.Count; i++)
        {
            ramp.Add((temperaturesMinusTMin[i] + abstemperaturesMinusTMin[i]) / 2.0f);
        }
        return ramp;
    }
}
