using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Nation {
    //name of nation
    public string name;
    public string playerUserName;

    //Alliances
    public enum Alliances
    {
        UmbrellaGroup,
        EU,
        EnvironmentalIntegrityGroup,
        AOSIS,
        AfricanGroup,
        ArabGroup,
        AILAC,
        ALBA,
        LDC,
        LMDC
    }
    public Alliances alliance;

    public enum Personality
    {
        Selfish,
        Altrusistic,
        Tech,
        Mitigation,
        Balance,
        Adaptation
    }

    public Personality behaviour;

    public float totalNationSpending, totalMitigation, totalTech, totalIcf, totalAdaptation, totalSpent;

    //used for switiching personality types, all but tech are in relation tothe total avergage spent on that area, while tech is in relation to how much this indivitual country has spent on mitigation
    public float natSpentThreshold, mitigationThreshold, techThreshold, icfThreshold, adaptThreshold;

    //Slider values
    public float nationalSpending, climateAdaptationSpent, climateMitigationSpent, climateTechSpent, icfSpent,
        budget, budgetMax, budgetAvailable;
    
    //Country stats
    public float[] emissionsIntensity = new float[10];
    public float[] tfp = new float[10];
    public float[] playerCapital = new float[10];
    public float[] savingsRate = new float[10];
    public float[] mitigationRate = new float[10];
    public float alphaK;
    public float[] theta;

    //Stats the player can see on main country page
    public float[] gdp = new float[10];
    public float[] totalEmissions = new float[10];
    public float[] mitigationCost = new float[10];

    //Stats on extra stats page
    public float[] labour = new float[10];
    public float[] totalDamage = new float[10];

    public float temperature;

    //Slider values
    public int oldSliderValue, nationID;

    //ICF withdrawls
    public bool canWithdraw, techBonus, ICFBonus;

    //Bool for AI
    public bool AIControlled, preSkipAI;
    
    //Constructor
    public Nation()
    {

    }
    public Nation(Alliances alliance, string name, float gdp, float[] labour, float[] tfp, 
        float[] savingsRate, float alphaK, float capital)
    {
        //Initial stats
        this.alliance = alliance;
        this.name = name;
        this.gdp[0] = gdp;
        this.labour = labour;
        this.tfp = tfp;
        this.savingsRate = savingsRate;
        this.playerCapital[0] = capital;
        //Set initial stats
        this.emissionsIntensity = new float[] { 0.350320027f, 0.324682279f, 0.301034941f, 0.279215233f, 0.259074324f, 0.240476082f,
            0.223295946f, 0.207419902f, 0.192743545f, 0.179171239f};
        this.mitigationCost = new float[] { 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f };
        this.mitigationRate = new float[] { 0.03f, 0.187151008f, 0.211465058f, 0.237698399f, 0.265906995f, 0.29615028f, 0.328490392f,
            0.362991483f, 0.399719103f, 0.438739652f };
        this.theta = new float[] { 0.06696572f, 0.060536245f, 0.054744727f, 0.049525877f, 0.044821285f, 0.040578676f, 0.036751247f,
            0.033297077f, 0.030178606f, 0.027362167f };
        //Calculated initial stats **THE ORDER MATTERS A LOT**
        climateAdaptationSpent = 0f;
        climateMitigationSpent = 0f;

        this.budget = (gdp / 2);
        CalcTotalEmissions(0);
        CalcTotalDamage(0);

        techBonus = false;
        ICFBonus = false;
    }

    public void EndTurn(int turn, float adaptationSpend, float mitigationSpend,float greenTechSpend, float globalTemp, float carbonRemovalPrice)
    {
        adaptationSpend = CheckICFBonus(adaptationSpend);
        
        CalcPlayerCapital(turn);//Requires last turn's Capital and Gross Investment
        CalcGDP(turn); //Requires capital
        CalcTotalDamage(turn); //Requires Climate Damage and Gross Output
        CalcTotalEmissions(turn); //Requires Industrial, Land Use and Other Emissions
        totalTech += greenTechSpend;

    }

    //Assuming that these calculations are made at the end of a turn
    void CalcGDP(int turn)
    {
        gdp[turn] = tfp[turn] * Mathf.Pow(playerCapital[turn], alphaK) * Mathf.Pow((labour[turn]),(1-alphaK));
    }

    void CalcTotalEmissions(int turn)
    {
        totalEmissions[turn] = emissionsIntensity[turn] * (1 - mitigationRate[turn]) * gdp[turn] - 
            (climateMitigationSpent/mitigationCost[turn]);
        if (techBonus)
        {
            totalEmissions[turn] *= 0.95f;
        }
    }

    void CalcPlayerCapital(int turn)
    {
        playerCapital[turn] = Mathf.Pow((1 - 0.08f), 5) * playerCapital[turn - 1] +
            5 * totalDamage[turn] * gdp[turn] * savingsRate[turn];
    }

    void CalcTotalDamage(int turn)
    {
        totalDamage[turn] = Mathf.Max( (1 - 0.00236f * Mathf.Pow(temperature, 2) - theta[turn] * Mathf.Pow(mitigationRate[turn], 2.6f)) - 
            climateAdaptationSpent, 0f);
    }

    void CalcBudget(int turn)
    {
        budget = (gdp[turn] / 2)*1000;
    }

    float CheckICFBonus(float adaptationSpent)
    {
        float toReturn = adaptationSpent;
        if (ICFBonus && canWithdraw)
            toReturn += 500;


        return toReturn;
    }

}
