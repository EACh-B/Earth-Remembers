using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardScript : MonoBehaviour {
    //Initializer required to view all nation details
    Initializer initScript;

    private void Start()
    {
        initScript = GetComponent<Initializer>();
    }

    //Sort lists for adapation
    public List<Nation> AdaptionTop5(int turn)
    {
        List<Nation> toReturn;
        List<Nation> allNations = new List<Nation>();

        foreach (Nation n in initScript.nationsList)
            allNations.Add(n);

        bool sorting = true;

        while (sorting)
        {
            sorting = false;
            for (int i = 0; i < allNations.Count - 1; i++)
            {
                float firstNat, secondNat;
                firstNat = allNations[i].totalAdaptation;
                secondNat = allNations[i + 1].totalAdaptation;

                if(firstNat < secondNat)
                {
                    sorting = true;

                    Nation temp = allNations[i];
                    allNations[i] = allNations[i + 1];
                    allNations[i + 1] = temp;
                    
                }
            }
        }

        toReturn = allNations;
        return toReturn;
    }
    //Sort lists for GDP
    public List<Nation> GDPTop5(int turn)
    {
        List<Nation> toReturn;
        List<Nation> allNations = new List<Nation>();

        foreach (Nation n in initScript.nationsList)
            allNations.Add(n);

        bool sorting = true;

        while (sorting)
        {
            sorting = false;
            for (int i = 0; i < allNations.Count - 1; i++)
            {
                float firstNat, secondNat;
                firstNat = allNations[i].gdp[turn];
                secondNat = allNations[i + 1].gdp[turn];

                if (firstNat < secondNat)
                {
                    sorting = true;

                    Nation temp = allNations[i];
                    allNations[i] = allNations[i + 1];
                    allNations[i + 1] = temp;

                }
            }
        }

        toReturn = allNations;
        return toReturn;
    }
    //Sort lists for ICF contributions
    public List<Nation> ICFTop5(int turn)
    {
        List<Nation> toReturn;
        List<Nation> allNations = new List<Nation>();

        foreach (Nation n in initScript.nationsList)
            allNations.Add(n);


        bool sorting = true;

        while (sorting)
        {
            sorting = false;
            for (int i = 0; i < allNations.Count - 1; i++)
            {
                float firstNat, secondNat;
                firstNat = allNations[i].icfSpent;
                secondNat = allNations[i + 1].icfSpent;

                if (firstNat < secondNat)
                {
                    sorting = true;

                    Nation temp = allNations[i];
                    allNations[i] = allNations[i + 1];
                    allNations[i + 1] = temp;

                }
            }
        }

        toReturn = allNations;
        return toReturn;
    }

}
