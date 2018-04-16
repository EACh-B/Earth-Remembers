using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temperature : MonoBehaviour {

    /*
     * ********************************
     * Things to note about this script
     * ********************************
     * This script will not work properly if it gets called more than once per turn.
     * It should be possible to remove entries with some additional work, this should not be neccessary.
     */

    //By making these into lists we can have them behave like functions of time.
    public List<float> temp = new List<float>(); //Degrees Celsius
    public List<float> lowerOceanTemp = new List<float>();

    public List<float> atmosphericConcentration = new List<float>(); //Gigatons
    public List<float> upperOceanConcentration = new List<float>(); //Gigatons
    public List<float> lowerOceanConcentration = new List<float>(); //Gigatons

    public List<float> globalCapital = new List<float>(); //$ trillions

    public List<float> emissions = new List<float>(); //Gigatons
    public List<float> landEmissions = new List<float>();

    public List<float> radForcing = new List<float>();

    public List<float> Fex = new List<float>(); //Gigatons

    List<float> simulatedtemp = new List<float>(); //Degrees Celsius
    List<float> simulatedradForcing = new List<float>();
    List<float> simulatedatmosphericConcentration = new List<float>(); //Gigatons
    public List<float> simulatedemissions = new List<float>(); //Gigatons
    List<float> simulatedlowerOceanTemp = new List<float>(); //Degrees Celsius
    public List<float> simulatednonCO2GHGs = new List<float>(); //Gigatons
    List<float> simulatedbiosphereConcentration = new List<float>(); //Gigatons

    public float[] reductionNeeded = new float[2];   //[0]is for +1.5 and [1] is for +2.0
 
    int turn; //The turn can serve as the list index.
    int simulatedturn;

	void Start () {
        //We know the initial conditions for many things.
        turn = 1;

        temp.Add(1.08f);
        lowerOceanTemp.Add(0.007f);

        atmosphericConcentration.Add(851f);
        upperOceanConcentration.Add(460f);
        lowerOceanConcentration.Add(1740f);

        emissions.Add(36.40836469f);
        landEmissions = new List<float>(){2.6f, 2.301f, 2.036385f, 1.802200725f, 1.594947642f, 1.411528663f, 1.249202867f, 1.105544537f,
            0.978406915f, 0.86589012f, 0.766312756f};

        Fex = new List<float>() {0.25f,0.275f,0.3f,0.325f,0.35f,0.375f,0.4f,0.425f,0.45f,0.475f,0.5f };
        RadiativeForcing(1);

        reductionNeeded[0] = 0f;
        reductionNeeded[1] = 0f;
    }

    public float CalculateTemperature(int turn)
    {
        AtmosphericConcentration(turn);
        UpperOceanConcentration(turn);
        LowerOceanConcentration(turn);

        RadiativeForcing(turn);
        LowerOceanTemperature(turn);
        Temp(turn);

        return temp[turn];
    }

    void Temp(int turn)
    {
        float temperature = 0.8718f * temp[turn - 1] + 0.0088f * lowerOceanTemp[turn - 1] + 0.1005f * radForcing[turn - 1];
        temp.Add(temperature);
    }

    void LowerOceanTemperature(int turn)
    {
        float temperature = 0.025f * temp[turn - 1] + 0.975f * lowerOceanTemp[turn - 1];
        lowerOceanTemp.Add(temperature);
    }

    void AtmosphericConcentration(int turn)
    {
        float CO2 = 0.88f * atmosphericConcentration[turn - 1] + 0.196f * upperOceanConcentration[turn - 1] + (5 / 3.666f) * emissions[turn - 1];
        atmosphericConcentration.Add(CO2);
    }

    void UpperOceanConcentration(int turn)
    {
        float CO2 = 0.12f * atmosphericConcentration[turn - 1] + 0.797f * upperOceanConcentration[turn - 1] + 0.001465f * lowerOceanConcentration[turn - 1];
        upperOceanConcentration.Add(CO2);
    }

    void LowerOceanConcentration(int turn)
    {
        float CO2 = 0.007f * upperOceanConcentration[turn - 1] + 0.99853488f * lowerOceanConcentration[turn - 1];
        lowerOceanConcentration.Add(CO2);
    }

    void RadiativeForcing(int turn)
    {
        float rad = 3.8f * Mathf.Log(((0.88f * atmosphericConcentration[turn-1] + 0.196f * upperOceanConcentration[turn-1] +
            (5 / 3.666f) * emissions[turn-1]) / 588), 2f) + Fex[turn-1];
        radForcing.Add(rad);
    }

    //======================
    //Simulating Temperature
    //======================

    public void findReductionNeeded()
    {
   
        reductionNeeded[0] = emissionsNeeded(turn, 1.5f);
        reductionNeeded[1] = emissionsNeeded(turn, 2f);
        Debug.Log("reduction to get to 1.5- " + reductionNeeded[0]);
        Debug.Log("reduction to get to 2- " + reductionNeeded[1]);
    }

    public float emissionsNeeded(int turn, float tempValue)
    {

        simulatedturn = turn;
        int emitionindex = 0;
        float predictedTemp=0;
        float predictedradForcing;
        float predictedatmosphericConcentration = 0;
        float predictedemissions = 0;
        float desiredradForcing;
        float desiredatmosphericConcentration = 0;
        float desiredemissions = 0;
        //set the simulated variables to the real ones for every turn so far
        for (int i = 0; i < simulatedturn; i++)
        {
            simulatedtemp.Add(temp[i]);
            simulatedradForcing.Add(radForcing[i]);
            simulatedatmosphericConcentration.Add(atmosphericConcentration[i]);
            simulatedemissions.Add(emissions[i]);
            simulatedlowerOceanTemp.Add(lowerOceanTemp[i]);
            simulatednonCO2GHGs.Add(Fex[i]);
            simulatedbiosphereConcentration.Add(upperOceanConcentration[i]);
            emitionindex = i;
        }
 

        for(int i = simulatedturn; i <= 10; i++)
        {
            simulatedemissions.Add(simulatedemissions[emitionindex]);
        }
        //runs simulated temperature calculation up to turn 10
        for(int i = simulatedturn; i < 10; i++)
        {
            predictedTemp = simulatedCalculateTemperature(i);
        }


        //find the predicted emissions 
        predictedradForcing = findR(predictedTemp, 10);
        predictedatmosphericConcentration = findA(predictedradForcing, 10);
        predictedemissions = findE(predictedatmosphericConcentration, 10);

        //find the desired emissions 
        desiredradForcing = findR(tempValue, 10);
        desiredatmosphericConcentration = findA(desiredradForcing, 10);
        desiredemissions = findE(desiredatmosphericConcentration, 10);

        //return the difference
        return predictedemissions - desiredemissions;
    }

    public float simulatedCalculateTemperature(int simulatedturn)
    {
      
        simulatedBiosphereConcentration(simulatedturn);
     
        simulatedLowerOceanTemperature(simulatedturn);
     
        simulatedAtmosphericConcentration(simulatedturn);
    
        simulatednonCO2GHGs.Add(0.25f);
      
        simulatedRadiativeForcing(simulatedturn);
       
        simulatedTemp(simulatedturn);
    
        return simulatedtemp[simulatedturn];
    }

    void simulatedTemp(int simulatedturn)
    {
        float temperature = simulatedtemp[simulatedturn - 1] + 0.1005f * (simulatedradForcing[simulatedturn] - (3.8f / 2.6f) * simulatedtemp[simulatedturn - 1] - 0.12f * (simulatedtemp[simulatedturn - 1] - simulatedlowerOceanTemp[simulatedturn - 1]));
        simulatedtemp.Add(temperature);
    }

    void simulatedRadiativeForcing(int simulatedturn)
    {
        float rad = (3.8f * (float)Math.Log(simulatedatmosphericConcentration[simulatedturn] / 588f)) / (float)Math.Log(2) + simulatednonCO2GHGs[simulatedturn];
        simulatedradForcing.Add(rad);
    }

    void simulatedAtmosphericConcentration(int simulatedturn)
    {
        
        float CO2 = 0.88f * simulatedatmosphericConcentration[simulatedturn - 1];
        
        CO2 = CO2 + 0.196f * simulatedbiosphereConcentration[simulatedturn - 1];
       
        CO2 = CO2 + (float)(5 / 3.666) * simulatedemissions[simulatedturn - 1];
        
        simulatedatmosphericConcentration.Add(CO2);
        
    }

    void simulatedLowerOceanTemperature(int simulatedturn)
    {
        float temperature = simulatedlowerOceanTemp[simulatedturn - 1] + 0.025f * (simulatedtemp[simulatedturn - 1] - simulatedlowerOceanTemp[simulatedturn - 1]);
        simulatedlowerOceanTemp.Add(temperature);
    }

    void simulatedNonCO2GHGs(float GHGs)
    {
        simulatednonCO2GHGs.Add(GHGs);
    }

    void simulatedBiosphereConcentration(int simulatedturn)
    {
        float CO2 = 0.088f * simulatedatmosphericConcentration[simulatedturn - 1] + 0.959f * simulatedbiosphereConcentration[simulatedturn - 1] + 3.375f;
        simulatedbiosphereConcentration.Add(CO2);
    }


    //finds the radForcing necesarry for the world temp to be at the desired value
    float findR(float temperature, int simulatedturn)
    {
        float R;

        R = (temperature / 1.005f) - (simulatedtemp[simulatedturn - 1] / 1.005f) + ((3.8f / 2.8f) * simulatedtemp[simulatedturn - 1]) + (0.12f * (simulatedtemp[simulatedturn - 1] - simulatedlowerOceanTemp[simulatedturn - 1]));

        return R;
    }

    //finds the AtmosphericConcentration necesarry for the world temp to be at the desired value
    float findA(float R , int simulatedturn)
    {
        float A;

        A= 588*Mathf.Pow(10f,(((float)Math.Log(2)*R)/3.8f) - (((float)Math.Log(2) * simulatednonCO2GHGs[simulatedturn - 1]) / 3.8f));
        
        return A;
    }

    //finds the emmisions necesarry for the world temp to be at the desired value
    float findE(float A, int simulatedturn)
    {
        float E;

        E = (A / (5f / 3.666f)) - ((0.88f * simulatedatmosphericConcentration[simulatedturn - 1]) / (5f / 3.666f)) - ((0.196f * simulatedbiosphereConcentration[simulatedturn - 1]) / (5f / 3.666f));

        return E;
    }
}
