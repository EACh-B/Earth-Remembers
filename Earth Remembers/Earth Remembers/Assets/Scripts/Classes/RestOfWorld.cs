using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestOfWorld : MonoBehaviour {

    float[] population = new float[] { 2841.34f, 3014.07f, 3176.73f, 3328.72f, 3468.74f, 3594.21f, 3703.05f, 3793.85f, 3866.90f, 3924.28f };

    float[] labour = new float[] { 1359.79f, 1449.68f, 1533.96f, 1613.15f, 1681.61f, 1737.77f, 1779.71f, 1812.35f, 1837.38f, 1856.30f };

    float[] capital = new float[] {67745.29f, 81100.91f, 95665.69f, 111259.44f, 128579.34f, 147600.53f, 168269.75f, 191138.94f,
        216577.48f, 244667.05f};

    float[] gdp = new float[] { 27140.80f, 32491.47f, 38326.56f, 44573.88f, 51512.76f, 59133.22f, 67413.93f, 76576.02f, 86767.46f,
        98020.99f };

    float[] tfp = new float[] { 2.36f, 2.49f, 2.61f, 2.74f, 2.87f, 3.01f, 3.16f, 3.32f, 3.49f, 3.67f };

    float[] savingsRate = new float[] { 0.2686f, 0.2598f, 0.2516f, 0.2479f, 0.2440f, 0.2401f, 0.2380f, 0.2366f, 0.2349f, 0.2328f };

    float alphaK = 0.55f;

    public float[] emissions = new float[10];

    float[] emissionsIntensity = new float[] { 0.350320027f, 0.324682279f, 0.301034941f, 0.279215233f, 0.259074324f, 0.240476082f,
            0.223295946f, 0.207419902f, 0.192743545f, 0.179171239f};

    float[] mitigationRate = new float[] { 0.03f, 0.187151008f, 0.211465058f, 0.237698399f, 0.265906995f, 0.29615028f, 0.328490392f,
            0.362991483f, 0.399719103f, 0.438739652f };

    public bool techBonus = false;

    // Use this for initialization
    void Start () {
        Emissions(0);
	}
	
	void Emissions(int turn)
    {
        emissions[turn] = emissionsIntensity[turn] * (1 - mitigationRate[turn]) * gdp[turn];
        if (techBonus)
        {
            emissions[turn] *= 0.95f;
        }
    }

    public void EndTurn(int turn)
    {
        Emissions(turn);
    }
}
