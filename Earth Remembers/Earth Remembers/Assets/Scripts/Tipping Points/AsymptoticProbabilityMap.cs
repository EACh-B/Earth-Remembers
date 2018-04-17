using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/* parameters and array setup
baseprob = 0.
slope  = 0.5
temperatures = fspan (295.,305.,50)
tmin = 300.
tmax = 303.
*/
/*
ramp = ((temperatures-tmin) + abs(temperatures-tmin))/2.
ramped_thresh = ramp/(tmax - tmin)
tanh_ramp = tanh((ramped_thresh+baseprob)/slope)
*/

/// <summary>
/// Monobehaviour implementation of Matthew Huber's specified formula for the purpose of calculating tipping point probability
/// </summary>
public class AsymptoticProbabilityMap : MonoBehaviour {
    /// <summary>
    /// Singleton instance of this behaviour (allows you to fiddle with the formula in the editor)
    /// </summary>
    public static AsymptoticProbabilityMap instance = null;

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
    /// </summary>
    public int stepCount = 50; 

    void Awake()
    {
        //Singleton Pattern. The instance of this can be accessed via AsymptoticProbabilityMap.instance.* in code
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);
    }

    public float GetProbability(float currentWorldTemp, float minTemp, float maxTemp)
    {
        //If we exceed the graph boundaries this will asymptote
        if (currentWorldTemp >= maxTemp) { return 1.0f; };
        if (currentWorldTemp <= minTemp) { return 0.0f; };

        //These are chainable but references maintained to intermediate lists for potential debugging.
        List<float> temperatures = fspan(minTemp, maxTemp, this.stepCount);
        if (currentWorldTemp >= temperatures.Last()) { return 1.0f; };
        if (currentWorldTemp <= temperatures.First()) { return 0.0f; };

        List<float> ramp = GenerateRamp(temperatures,minTemp);
        List<float> rampedThresholds = GenerateRampedThreshold(minTemp, maxTemp, ramp);
        //These are probabilities presumably between 0 and 1 (must check) at the same index as the temperature at which the probability occurs
        List<float> tanhRamp = GenerateTanhRamp(rampedThresholds);
        int probabilityIndex = getApproximatedTemperatureIndex(currentWorldTemp, temperatures);
        var dic = temperatures.Select((k, i) => new { k, v = tanhRamp[i] })
              .ToDictionary(x => x.k, x => x.v);
        return tanhRamp[probabilityIndex];
    }
    /// <summary>
    /// Returns the index of the closest value to the provided world temperature within the list of temperatures.
    /// </summary>
    /// <param name="currentWorldTemp">Temperature we want to approximate</param>
    /// <param name="temperatures"></param>
    /// <returns></returns>
    private int getApproximatedTemperatureIndex(float currentWorldTemp, List<float>temperatures)
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
    public static List<float> fspan(float min, float max, int stepCount)
    {
        if (min > max) { throw new System.Exception("fspan arg error: minimum value is greater than maximum. Min: "+ min.ToString("0.00") + " Max: " + max.ToString("0.00")); }
        if (stepCount <= 0) { throw new System.Exception("stepCount must be greater than 0. Provided stepcount: " + stepCount.ToString()); }
        List<float> spanList = new List<float>();
        float stepSize = (max - min) / (float)stepCount;
        for (int i = 0; i <= stepCount; i++)
        {
            spanList.Add(min + (float)i * stepSize);
        }
        return spanList;
    }
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
    private List<float> GenerateRampedThreshold(float min, float max, List<float> ramp)
    {
        //We could use abs here instead but this would just conceal a logical error in the game itself
        if (min > max) { throw new System.Exception("GenerateRampedThreshold arg error: minimum value is greater than maximum. Min: " + min.ToString("0.00") + " Max: " + max.ToString("0.00")); } 
        float range = max - min;
        return (ramp.Select(i => i / range).ToList<float>());
    }
    private List<float> GenerateTanhRamp(List<float> rampedThreshold)
    {
        return rampedThreshold.Select(i => (float)Math.Tanh((i + baseProb) / slope)).ToList<float>();
    }
    void Update()
    {
        AsymptoticProbabilityMap.instance.GetProbability(4f, 2f, 5.5f);
    }
}
