using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThisPlayerScript : MonoBehaviour {
    public string userName;
    public string thisPlayerAlliance;
    public string[] unselectedAlliances;
    public int selectedNationIndex;
    public List<Nation> nationsInThisAlliance = new List<Nation>();
    //public List<PlayerNation> nationsInThisAlliance = new List<PlayerNation>();

    public Nation currentSelectedNation;
    //public PlayerNation currentSelectedNation;

    WorldStatusController wsc;
    ClientUIController clientUIController;
    [SerializeField]
    Text nationText;
    public float budgetMax, budgetAvailable;
    public GameObject[] sliderUiParents;
    public List<SpendingHistory> spendingHistory;

    //For start of game alliance selecting
    public string selectedAlliance;

    //Get the wsc script
    private void Start()
    {
        wsc = GetComponent<WorldStatusController>();
        clientUIController = GetComponent<ClientUIController>();
        selectedNationIndex = 0;
        budgetMax = 100;
        budgetAvailable = budgetMax;

        spendingHistory = new List<SpendingHistory>();
    }

    //Get the ID of this user which is randomized 'NewPlayer' + num
    void GetUsername()
    {
        userName = PhotonNetwork.playerName;
        print("I am " + userName);
        PlayerPrefs.SetString("UNAME", userName);
    }

    //To get a nation
    public IEnumerator AssignNation()
    {
        yield return new WaitForSeconds(0.4f);
        if (wsc.state == WorldStatusController.STATE.PREGAME || wsc.state == WorldStatusController.STATE.SETUP && wsc.turn == 1)
        {

            //First get username
            GetUsername();
            //Get the unselected alliances form the host
            wsc.GetUnselectedAlliances(userName);
        }
        //Else look to rejoin game or kick player as the game has already started
        else
        {
            userName = PlayerPrefs.GetString("UNAME");

            wsc.CheckToRejoinPlayer(userName);
        }
    }

    public void CompleteRefresh()
    {
        //Refresh Alliances list
        GameObject.Find("AllianceSelect_Parent").GetComponent<AllianceSelectionUIScript>().RefreshListing(unselectedAlliances);
    }

    //We have the nation name, now find that nation and set it as 'ThisNation'
    public void SetTheReturnedAlliance(string allianceName)
    {
        thisPlayerAlliance = allianceName;
        //Get initializer
        Initializer init = GetComponent<Initializer>();
        string nationAlliance = "";
        //Check all nations initialized from the XML doc
        foreach (Nation n in init.nationsList)
        { 
            nationAlliance = System.Enum.GetName(typeof(Nation.Alliances), n.alliance);
            n.playerUserName = userName;

            if (nationAlliance == allianceName)
            {
                //When we find the correct nation which was passed back, set it as this nation
                nationsInThisAlliance.Add(n);

                /*
                PlayerNation pN = new PlayerNation(nationAlliance, n.name, n.gdp[0], n.totalEmissions[0], n.carbonRemovalPrice[0], n.budget,
                    wsc.worldTemp, n.population[0], n.totalDamage[0], n.AIControlled, n.isVulnerable);
                    */
            }
        }
        clientUIController.SetNationNameText(nationsInThisAlliance);
        currentSelectedNation = nationsInThisAlliance[selectedNationIndex];
        int index = 0;
        foreach(Nation n in nationsInThisAlliance)
        {
            n.budgetAvailable = n.budget;
            n.budgetMax = n.budget;

            n.nationalSpending = 0;
            n.climateAdaptationSpent = 0;
            n.climateMitigationSpent = 0;
            n.icfSpent = 0;

            n.oldSliderValue = 0;
            n.nationID = index;

            //disables AI for player nations

            n.AIControlled = false;
            Debug.Log("Setting " + n.name + " AI to " + n.AIControlled);
            index++;
        }

        //Set flag img
        Sprite newPin = Resources.Load<Sprite>(currentSelectedNation.name);
        clientUIController.selectedNationPin.sprite = newPin;

        //Send selected nation to host to confirm and remove from playable list
        wsc.ConfirmSelectedAlliance(allianceName);
        //wsc.ConnectedSendStats();
    }

    public void ChangedNationAdjustUI()
    {
        currentSelectedNation.nationalSpending = currentSelectedNation.budgetAvailable;
        print(sliderUiParents[0].transform.GetChild(2).gameObject.name);
        //economy
        sliderUiParents[0].transform.GetChild(4).gameObject.GetComponent<InputField>().text = currentSelectedNation.budgetAvailable.ToString("0.00");
        sliderUiParents[0].transform.GetChild(3).gameObject.GetComponent<Slider>().value = ((float)currentSelectedNation.budgetAvailable / (float)currentSelectedNation.budgetMax) *100;
        //Climate Adaptation
        sliderUiParents[2].transform.GetChild(4).gameObject.GetComponent<InputField>().text = currentSelectedNation.climateAdaptationSpent.ToString("0.00");
        sliderUiParents[2].transform.GetChild(3).gameObject.GetComponent<Slider>().value = ((float)currentSelectedNation.climateAdaptationSpent / (float)currentSelectedNation.budgetMax) * 100;
        //Climate RMitigation
        sliderUiParents[3].transform.GetChild(4).gameObject.GetComponent<InputField>().text = currentSelectedNation.climateMitigationSpent.ToString("0.00");
        sliderUiParents[3].transform.GetChild(3).gameObject.GetComponent<Slider>().value = ((float)currentSelectedNation.climateMitigationSpent / (float)currentSelectedNation.budgetMax) * 100;
        //Green slider
        if(currentSelectedNation.climateTechSpent > 0)
            GameObject.Find("GreenTech_Slider").GetComponent<Slider>().value = ((float)currentSelectedNation.climateTechSpent / (float)currentSelectedNation.climateMitigationSpent) * 100;

        //international fund
        sliderUiParents[1].transform.GetChild(4).gameObject.GetComponent<InputField>().text = currentSelectedNation.icfSpent.ToString("0.00");
        sliderUiParents[1].transform.GetChild(3).gameObject.GetComponent<Slider>().value = ((float)currentSelectedNation.icfSpent / (float)currentSelectedNation.budgetMax) * 100;

        //Left panel UI adjustment
        GameObject.Find("GDP&Rank_Txt").GetComponent<Text>().text = "GDP: $ " + currentSelectedNation.gdp[wsc.turn - 1].ToString("0.00");
        wsc.GetGDPLeaderboardRank(currentSelectedNation.name);

        GameObject.Find("GlobalTemp_Txt").GetComponent<Text>().text = "Global Temperature:\n+" + wsc.worldTemp.ToString("0.00") + "c";
        GameObject.Find("EmisReduc_Txt").GetComponent<Text>().text = "Emissions Reduction:\n" + currentSelectedNation.mitigationCost[wsc.turn].ToString("0.00") + "m per Ton";

        //Calculate percentage of world
        float percentageOfWorldEmissions = (currentSelectedNation.totalEmissions[wsc.turn-1] / wsc.worldEmissions) * 100;
        GameObject.Find("TotalEmis_Txt").GetComponent<Text>().text = "% of World Emissions: " + percentageOfWorldEmissions.ToString("0.0");
    
        //ai button
        if (currentSelectedNation.AIControlled == true)
        {
            sliderUiParents[1].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = false;
            sliderUiParents[2].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = false;
            sliderUiParents[3].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = false;
        }
        else
        {
            sliderUiParents[1].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = true;
            sliderUiParents[2].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = true;
            sliderUiParents[3].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = true;
        }
    }

    public void CreateNewHistoryData(int turnNum)
    {
        for(int i = 0; i < nationsInThisAlliance.Count; i++)
        {
            spendingHistory.Add(new SpendingHistory(turnNum, nationsInThisAlliance[i].name, "nationalSpending", nationsInThisAlliance[i].nationalSpending));
            spendingHistory.Add(new SpendingHistory(turnNum, nationsInThisAlliance[i].name, "climateAdaptationSpent", nationsInThisAlliance[i].climateAdaptationSpent));
            spendingHistory.Add(new SpendingHistory(turnNum, nationsInThisAlliance[i].name, "climateMitigationSpent", nationsInThisAlliance[i].climateMitigationSpent));
            spendingHistory.Add(new SpendingHistory(turnNum, nationsInThisAlliance[i].name, "icfSpent", nationsInThisAlliance[i].icfSpent));
        }
    }

    [System.Serializable]
    public struct SpendingHistory
    {
        [SerializeField]
        public float turn;
        [SerializeField]
        public string country;
        [SerializeField]
        public string var;
        [SerializeField]
        public float value;

        public SpendingHistory(float _turn, string _country, string _var, float _value)
        {
            turn = _turn;
            country = _country;
            var = _var;
            value = _value;
        }
    }

    public void CountryAISelectOn()
    {
        Nation n = nationsInThisAlliance[selectedNationIndex];
        GetComponent<PhotonView>().RPC("CountryAiSelectionOn", PhotonTargets.All, n.name);
        currentSelectedNation.AIControlled = true;
        sliderUiParents[1].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = false;
        sliderUiParents[2].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = false;
        sliderUiParents[3].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = false;
    }

    public void CountryAISelectOff()
    {
        Nation n = nationsInThisAlliance[selectedNationIndex];
        GetComponent<PhotonView>().RPC("CountryAiSelectionOff", PhotonTargets.All, n.name);
        currentSelectedNation.AIControlled = false;
        sliderUiParents[1].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = true;
        sliderUiParents[2].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = true;
        sliderUiParents[3].transform.GetChild(3).gameObject.GetComponent<Slider>().enabled = true;
    }
}
