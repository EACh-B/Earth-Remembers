using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class WorldStatusController : MonoBehaviour
{
    EventsScript eventsScript;
    HostUiController hostUiController;
    Temperature temper;
    TippingPointController tpController;

    public float worldEmissions, worldTemp;
    public int currentYear;
    public float ICF, ICFGOAL;
    public GameObject oceanPlane, confirmPopup;
    public string[] nats;
    //Initializer Script
    public Initializer initializer;
    //Timer script
    TimerScript timerScript;
    //List of nations which are playable
    public List<string> unpickedAlliances = new List<string>();
    public List<Nation> GDPLeaderboard;
    //Nations which are already picked and the player username assigned to
    public Dictionary<string, Nation> assginedNations = new Dictionary<string, Nation>();

    //To be used to check all players have submitted spending
    public bool[] playersSubmitted = new bool[4];

    public bool ElNinoTipped, WAISTipped, CoralReefTipped, RainForestTipped;
    Button submitBtn, ICFBtn;

    public float NPCEmissions = 16794;
    //Displayed on the master funding tab
    float currentICFSpent;

    //for testing networks
    public int playerCount = 0;
    public int readyCount = 0;
    //The state of this game
    public STATE state;
    //display a warning when submitting
    bool submitWarning;

    public enum STATE
    {
        SETUP,
        PLAYING,
        SUBMITTED,
        PREGAME
    }

    //variables for tech emmitions reduction
    private float techSpending = 0f;
    private float totalTechThreshold;
    private float flatReduction;
    private float overSpendMultipiler;
    private float bonusPercentPerMillion;
    private bool totalTechThresholdReached;
    public Text PostGameText;

    float[] AIbehaviours = new float[4]; //values 0=low 1=med 2=high

    System.Random rng = new System.Random();

    public Text[] Tippingpoints;
    public Text temp;
    public float[] tfp = new float[] { 5.115f, 5.536f, 5.979f, 6.445f, 6.934f, 7.446f, 7.981f, 8.540f, 9.122f, 9.727f };
    public float[] priceOfCarbon = new float[] { 30.69f, 36.718f, 43.526f, 51.170f, 59.696f, 69.152f, 79.584f, 91.038f, 103.562f, 117.2f };
    public float[] sigma = new float[] { 0.350f, 0.333f, 0.317f, 0.301f, 0.287f, 0.273f, 0.259f, 0.247f, 0.235f, 0.223f };
    public float[] backstopTech = new float[] { 536.250f, 522.844f, 509.773f, 497.028f, 484.603f, 472.488f, 460.675f, 449.158f,
        437.930f, 426.981f};
    public int turn;


    //tipping point min and max values - CHANGED IN INSPECTOR
    public float CRmin = 1, CRmax = 1, ISmin = 1, ISmax = 10, RFmin = 2, RFmax = 12, monMin, monMax, antMin, antMax;

    private void Start()
    {
        //Set turn to 1 at start, this overrides the inspector
        turn = 1;
        totalTechThresholdReached = false;
        flatReduction = 5;
        totalTechThreshold = 50000;
        bonusPercentPerMillion = 0.2f;
        currentICFSpent = 0;

        ElNinoTipped = false;
        WAISTipped = true;
        CoralReefTipped = true;
        RainForestTipped = false;

        temper = GetComponent<Temperature>();
        eventsScript = GetComponent<EventsScript>();
        hostUiController = GetComponent<HostUiController>();
        timerScript = GetComponent<TimerScript>();
        tpController = GetComponent<TippingPointController>();

        submitBtn = GameObject.Find("Submit_Btn").GetComponent<Button>();
        //*****
        //ICFBtn = GameObject.Find("ReliefFund_Btn").GetComponent<Button>();
        //****
        worldEmissions = 650000;
        worldTemp = this.GetComponent<Temperature>().CalculateTemperature(1);
        temp.text = "+" + worldTemp.ToString("0.00");
        ICFGOAL = 1000;
        SetupSpendingConfirmationBox();
    }

    private void SetupSpendingConfirmationBox()
    {
        Button b = GameObject.Find("ConfirmSpending_Btn").GetComponent<Button>();

        confirmPopup = b.gameObject.transform.parent.gameObject;
        confirmPopup.SetActive(false);
    }

    public void AssignLists()
    {
        rng = new System.Random();
        int i = rng.Next(0, 5);

        print("Assigning Lists");
        initializer = GetComponent<Initializer>();


        if (PhotonNetwork.isMasterClient)
            foreach (Nation n in initializer.nationsList)
                if (!unpickedAlliances.Contains(n.alliance.ToString()))
                    unpickedAlliances.Add(n.alliance.ToString());

        foreach (Nation n in initializer.nationsList)
        {
            n.budgetMax = n.budget;
            Debug.Log(n.name + n.budgetMax);
            //assigns the ai personality of the nations
            switch (i)
            {
                case (0):
                    {
                        n.behaviour = Nation.Personality.Selfish;
                        n.natSpentThreshold = 0.9f;
                        n.icfThreshold = 0.5f;
                        n.adaptThreshold = 0.7f;
                        n.mitigationThreshold = 0.7f;
                        n.techThreshold = 0.4f;
                        i = rng.Next(0, 5);
                    }

                    break;
                case (1):
                    {
                        n.behaviour = Nation.Personality.Altrusistic;
                        n.natSpentThreshold = 0.5f;
                        n.icfThreshold = 0.9f;
                        n.adaptThreshold = 0.7f;
                        n.mitigationThreshold = 0.7f;
                        n.techThreshold = 0.4f;
                        i = rng.Next(0, 5);
                    }

                    break;
                case (2):
                    {
                        n.behaviour = Nation.Personality.Balance;
                        n.natSpentThreshold = 0.8f;
                        n.icfThreshold = 0.8f;
                        n.adaptThreshold = 0.8f;
                        n.mitigationThreshold = 0.8f;
                        n.techThreshold = 0.5f;
                        i = rng.Next(0, 5);
                    }

                    break;
                case (3):
                    {
                        n.behaviour = Nation.Personality.Mitigation;
                        n.natSpentThreshold = 0.5f;
                        n.icfThreshold = 0.5f;
                        n.adaptThreshold = 0.6f;
                        n.mitigationThreshold = 0.9f;
                        n.techThreshold = 0.3f;
                        i = rng.Next(0, 5);
                    }

                    break;
                case (4):
                    {
                        n.behaviour = Nation.Personality.Tech;
                        n.natSpentThreshold = 0.5f;
                        n.icfThreshold = 0.5f;
                        n.adaptThreshold = 0.6f;
                        n.mitigationThreshold = 0.5f;
                        n.techThreshold = 0.7f;
                        i = rng.Next(0, 5);
                    }
                    break;
                case (5):
                    {
                        n.behaviour = Nation.Personality.Adaptation;
                        n.natSpentThreshold = 0.6f;
                        n.icfThreshold = 0.5f;
                        n.adaptThreshold = 0.9f;
                        n.mitigationThreshold = 0.5f;
                        n.techThreshold = 0.4f;
                        i = rng.Next(0, 5);
                    }
                    break;
            }
            Debug.Log(n.name + " " + n.behaviour);

        }
    }

    private void Update()
    {
        GameObject.Find("PlayerCount_Txt").GetComponent<Text>().text = "Players: " + playerCount + "/10";

        if (state == STATE.PLAYING)
        {
            //ICFBtn.interactable = true;
            submitBtn.interactable = true;
        }
        else
        {
            //ICFBtn.interactable = false;
            submitBtn.interactable = false;
        }
    }

    //Syncing variables via Photon net
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //If this system is currently writing
        if (stream.isWriting == true)
        {
            stream.SendNext(nats);
            stream.SendNext(worldEmissions);
            stream.SendNext(currentYear);
            stream.SendNext(playerCount);
            stream.SendNext(ICF);
            stream.SendNext(turn);
            stream.SendNext(timerScript.currentTime);
        }
        //Otherwise
        else
        {
            nats = (string[])stream.ReceiveNext();
            worldEmissions = (float)stream.ReceiveNext();
            currentYear = (int)stream.ReceiveNext();
            playerCount = (int)stream.ReceiveNext();
            ICF = (float)stream.ReceiveNext();
            turn = (int)stream.ReceiveNext();
            timerScript.currentTime = (float)stream.ReceiveNext();
        }
    }

    //Called once when a room is created
    public void SetUpGame()
    {
        Initializer initializer = GetComponent<Initializer>();
        worldEmissions = initializer.startingEmissions;
        worldTemp = initializer.startingTemp;
        worldEmissions = 18;
        foreach (Nation n in initializer.nationsList)
            worldEmissions += n.totalEmissions[turn - 1];

        tpController.CheckTPs(worldTemp, CRmin, CRmax, ISmin, ISmax, RFmin, RFmax, monMin, monMax, antMin, antMax);
    }

    public void SetupToPlaying()
    {
        GetComponent<PhotonView>().RPC("ChangeStateToPlayingRPC", PhotonTargets.Others);
    }

    public void PreGameToSetup()
    {
        GetComponent<PhotonView>().RPC("PreGameToSetupRPC", PhotonTargets.AllBuffered);
    }

    public void NextTurn_Click()
    {
        readyCount = 0;
        currentICFSpent = 0;

        turn++;

        GetComponent<PhotonView>().RPC("UpdateTurnRPC", PhotonTargets.Others, turn);

        state = STATE.SUBMITTED;

        UpdatePlayerNationTotalSpent();
        //Increase year
        GetComponent<PhotonView>().RPC("IncreaseYearRPC", PhotonTargets.AllBuffered);


        //Unhide events draw button
        hostUiController.drawEventsBtn.GetComponent<Button>().interactable = true;
        //Formula for calculating shit goes here


        

        //Set all players back to playing state
        //GetComponent<PhotonView>().RPC("ChangeStateToPlayingRPC", PhotonTargets.AllBuffered);

        //Set next turn button to non-interactable as events must be drawn first
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        Debug.Log(worldEmissions);

        //sets the ai behaviour and runs the ai
        CountryAI();
        UpdateAINationTotalSpent();

        foreach (Nation n in initializer.nationsList)
        {
            Debug.Log(n.name + "AI enabled = " + n.AIControlled);
            Debug.Log(n.name + "national spending" + n.nationalSpending);
            Debug.Log(n.name + "Climate Adaptation" + n.climateAdaptationSpent);
            Debug.Log(n.name + "Icf" + n.icfSpent);
        }

        //running to test
        temper.findReductionNeeded();
        Debug.Log(temper.reductionNeeded);

        //cheks for the initial reduction for tech
        if (turn == 4 && totalTechThresholdReached == false)
        {
            Debug.Log("flat reduction triggering");
            if (totalTechSpent() >= totalTechThreshold)
            {
                foreach (Nation n in initializer.nationsList)
                {
                    n.techBonus = true;
                }
                totalTechThresholdReached = true;
            }
        }

        if (turn <= 4)
        {
            hostUiController.SetTechSlider(totalTechSpent(), totalTechThreshold);
        }

        ////activates bonus emmisions reduction for tech
        //if(totalTechThresholdReached == true)
        //{
        //    Debug.Log("bonus reduction triggering");
        //    overSpendMultipiler = 0.05f*(totalTechSpent() - totalTechThreshold);
        //    foreach (Nation n in initializer.nationsList)
        //    {
        //        n.totalEmissions[turn-1] = n.totalEmissions[turn-2] * (100 - ((flatReduction) / 100)) + (100 - ((overSpendMultipiler) / 100));
        //    }
        //}

        bool goalPassed = false;
        ICF = 0;


        //adds icf spent by countries to the ICF
        foreach (Nation n in initializer.nationsList)
        {
            ICF += n.icfSpent;
        }

        if (ICF >= ICFGOAL)
            goalPassed = true;

        //Figure out if the ICF goal has been passed and notify nations
        foreach (Nation n in initializer.nationsList)
        {
            if (ICF >= ICFGOAL)
            {
                if (n.canWithdraw)
                    n.ICFBonus = true;
                else
                    n.ICFBonus = false;
            }
            else
            {
                n.ICFBonus = false;
            }
        }

        //stream to other players
        GetComponent<PhotonView>().RPC("SetICFBonusRPC", PhotonTargets.Others, goalPassed);



        //checks for the adaptation goal, also checks for the icf bonus for climateVulnerable countries
        //foreach (Nation n in initializer.nationsList)
        //{
        //    if (n.climateAdaptationSpent >= n.adaptationGoal)
        //    {
        //        n.adaptationGoalMet = true;
        //    }
        //    else if (n.canWithdraw == true && ICF >= ICFGOAL && (n.climateAdaptationSpent + (n.adaptationGoal * 0.25)) >= n.adaptationGoal)
        //    {
        //        n.adaptationGoalMet = true;
        //    }
        //    else
        //    {
        //        n.adaptationGoalMet = false;
        //    }

        //    n.adaptationGoal = n.adaptationGoal * 1.1f;
        //    Debug.Log(n.name + n.adaptationGoal);
        //}

        ICFGOAL = ICFGOAL * 1.1f;

        Debug.Log("turn- " + turn);
        float emissionsTemp = this.GetComponent<RestOfWorld>().emissions[turn - 1];
        foreach (Nation n in initializer.nationsList)
        {
            n.EndTurn(turn - 1, n.climateAdaptationSpent, n.climateMitigationSpent, n.climateTechSpent, worldTemp, priceOfCarbon[turn - 1]);
            emissionsTemp += n.totalEmissions[turn - 1];
        }
        this.GetComponent<Temperature>().emissions.Add(emissionsTemp);
        worldEmissions = emissionsTemp;

        worldTemp = this.GetComponent<Temperature>().CalculateTemperature(turn-1);
        temp.text = ("+" + worldTemp.ToString("0.00"));
    
        HostSendScoresToClients();
        tpController.CheckTPs(worldTemp, CRmin, CRmax, ISmin, ISmax, RFmin, RFmax, monMin, monMax, antMin, antMax);

        if (turn == 10)
        {

        }

        hostUiController.SetHostDisplay();
        //Reset the slider component values
        GetComponent<PhotonView>().RPC("ResetSlidersRPC", PhotonTargets.Others);
        GameObject.Find("CurrentICFSpent_Txt").GetComponent<Text>().text = "$" + currentICFSpent.ToString("0.0") + "bn";

        //Set all spent values back to 0
        GetComponent<PhotonView>().RPC("ResetSpentValuesRPC", PhotonTargets.AllBuffered);
        if(turn == 10)
        {
            jumpForward();
        }
    }


    public void jumpForward()
    {
        ElNinoTipped = true;
        RainForestTipped = true;
        if (ElNinoTipped && RainForestTipped)
        {
            GetComponent<PhotonView>().RPC("SetPostGameText", PhotonTargets.AllBuffered, "Both triggered");
        }
        else if (ElNinoTipped)
        {
            GetComponent<PhotonView>().RPC("SetPostGameText", PhotonTargets.AllBuffered, "El Nino Trigered");
        }
        else if (RainForestTipped)
        {
            GetComponent<PhotonView>().RPC("SetPostGameText", PhotonTargets.AllBuffered, "Rain Forrest Triggered");
        }
        else
        {
            GetComponent<PhotonView>().RPC("SetPostGameText", PhotonTargets.AllBuffered, "None Triggered");
        }

        GetComponent<PhotonView>().RPC("ToPostGame", PhotonTargets.AllBuffered);

    }

    [PunRPC]
    public void SetPostGameText(string text)
    {
        PostGameText.text = text;
    }

    [PunRPC]
    void ToPostGame()
    {
        ChangeLayout.ChangeCurrentLayout("PostGame_Parent");
    }


    //After the next turn button presssed, calculated economy scores & multipliers, send data back to clients
    public void HostSendScoresToClients()
    {
        //Array of economy score, economy mult & new budget which will be passed to clients
        float[] nationOne = new float[3];
        float[] nationTwo = new float[3];
        float[] nationThree = new float[3];
        
        foreach(Nation n in initializer.nationsList)
        {
            GetComponent<PhotonView>().RPC("UpdateNationData", PhotonTargets.All, n.name, n.gdp[turn-1], n.totalEmissions[turn-1], priceOfCarbon[turn-1], n.budget, n.labour[turn-1], n.totalDamage[turn-1],
                n.alliance.ToString());
        }
        
    }



    [PunRPC]
    public bool HasGameStarted()
    {
        bool started;

        if (state != STATE.SETUP)
            started = true;
        else
            started = false;

        return started;
    }

    //Called when someone removes funds from the ICF in order to update the budget
    public void UpdateICF(float amount)
    {
        GetComponent<PhotonView>().RPC("UpdateICFRPC", PhotonTargets.MasterClient, amount);
    }

    //Called by host when they start the game
    public void MovePlayersToAllianceSelect()
    {
        GetComponent<PhotonView>().RPC("ChangeViewsToAllianceSelectRPC", PhotonTargets.All);
    }

    //Called by all players to move them to alliance select screen
    [PunRPC]
    public void ChangeViewsToAllianceSelectRPC()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            //Change layout
            ChangeLayout.ChangeCurrentLayout("AllianceSelect_Parent");
            //Move slider to middle
            GameObject container = GameObject.Find("AllianceList_Container");
            container.transform.localPosition = new Vector3(container.transform.localScale.x/2, 0, 0);
        }
    }

    public void Click_SubmitShowWarning()
    {
        confirmPopup.SetActive(true);
    }

    public void Click_RejectSpending()
    {
        confirmPopup.SetActive(false);
    }

    //Called to submit spending on clients
    public void Click_SubmitSpending()
    {
        confirmPopup.SetActive(false);
        ClientUIController uIController = GetComponent<ClientUIController>();
        uIController.ShowPopupMessage("Spending Submitted", "Your spending has been sent to the game master. Please wait for the turn to end");
        //This player script
        ThisPlayerScript thisPlayer = GetComponent<ThisPlayerScript>();
        turn++;
        //Stop submit from being pressed twice
        GameObject.Find("Submit_Btn").GetComponent<Button>().interactable = false;
        //ICFBtn.interactable = false;

        //Each spent value for each nation
        float[] nationOneData = new float[9];
        float[] nationTwoData = new float[9];
        float[] nationThreeData = new float[9];

        //Get spent data for nations

        //Pass in alliance name
        string allianceName = thisPlayer.thisPlayerAlliance;
        //Lock in values and disable adjustments

        state = STATE.SUBMITTED;

        //Sent the spent values to the host
        //GetComponent<PhotonView>().RPC("SubmitSpendingRPC", PhotonTargets.MasterClient, nationOneData, nationTwoData, nationThreeData, allianceName);


        //The data to be transfered from the sliders of this player
        float[] transferData = new float[5];
        //Called once for each of the nations in the alliance
        for (int i = 0; i < thisPlayer.nationsInThisAlliance.Count; i++)
        {
            transferData[0] = thisPlayer.nationsInThisAlliance[i].nationalSpending;
            transferData[1] = thisPlayer.nationsInThisAlliance[i].climateAdaptationSpent;
            transferData[2] = thisPlayer.nationsInThisAlliance[i].climateMitigationSpent;
            transferData[3] = thisPlayer.nationsInThisAlliance[i].climateTechSpent;
            transferData[4] = thisPlayer.nationsInThisAlliance[i].icfSpent;
            //End turn calculations on host
            GetComponent<PhotonView>().RPC("SubmitClientSpend", PhotonTargets.MasterClient, thisPlayer.nationsInThisAlliance[i].name, transferData);

            Nation n = thisPlayer.nationsInThisAlliance[i];
            //End turn calculations on client side
            n.EndTurn(turn - 1, n.climateAdaptationSpent, n.climateMitigationSpent, n.climateTechSpent, worldTemp, priceOfCarbon[turn - 1]);
        }
        GetComponent<PhotonView>().RPC("IncreaseReadyCount", PhotonTargets.MasterClient);
        thisPlayer.CreateNewHistoryData(turn);

    }

    //Called from HOSTUiController on host side to call all players for gathering data for the nation to be viewed by host
    public void FetchViewableData(string nationName)
    {
        //Call to all
        GetComponent<PhotonView>().RPC("FetchNationDataRPC", PhotonTargets.All, nationName);
    }

    //Get the unselected alliances for refreshing list of unpicked alliances, called by clients
    public void GetUnselectedAlliances(string username)
    {
        GetComponent<PhotonView>().RPC("GetUnselectedAlliancesFromHost", PhotonTargets.MasterClient, username);
    }

    public void GetGameStatus()
    {
        GetComponent<PhotonView>().RPC("ReturnStatusRPC", PhotonTargets.MasterClient);
    }

    //Called by host
    [PunRPC]
    public void ReturnStatusRPC()
    {
        string ans = "null";
        switch (state)
        {
            case STATE.PLAYING:
                ans = "playing";
                break;
            case STATE.SETUP:
                ans = "setup";
                break;
            case STATE.PREGAME:
                ans = "preGame";
                break;
        }

        GetComponent<PhotonView>().RPC("RecieveStatusRPC", PhotonTargets.MasterClient, ans);
    }

    //Called by client
    [PunRPC]
    public void RecieveStatusRPC(string theState)
    {
        switch (theState)
        {
            case "playing":
                state = STATE.PLAYING;
                break;
            case "setup":
                state = STATE.SETUP;
                break;
            case "preGame":
                state = STATE.PREGAME;
                break;
            case "null":
                Debug.LogError("State Sync Error");
                break;
        }
    }

    //Called by Host
    [PunRPC]
    public void GetUnselectedAlliancesFromHost(string username)
    {
        string[] unselectedAlliances = unpickedAlliances.ToArray();
        GetComponent<PhotonView>().RPC("ReturnUnselectedAlliancesToClient", PhotonTargets.All, username, unselectedAlliances);
    }

    public void ConfirmSelectedAlliance(string alliance)
    {
        print("confirm");
        ThisPlayerScript tps = GetComponent<ThisPlayerScript>();
        GetComponent<PhotonView>().RPC("HostRemoveSelectedAlliance", PhotonTargets.MasterClient, alliance, tps.userName);
    }


    //Players rejoining game
    public void CheckToRejoinPlayer(string username)
    {
        GetComponent<PhotonView>().RPC("CheckUsernameAgainstNationsRPC", PhotonTargets.MasterClient, username);
    }

    //Calledb by host
    [PunRPC]
    void CheckUsernameAgainstNationsRPC(string userName)
    {
        string returnString = "null";
        foreach(Nation n in initializer.nationsList)
        {
            if (n.playerUserName == userName)
            {
                returnString = n.alliance.ToString();
                break;
            }
        }

        GetComponent<PhotonView>().RPC("ReturnRejoinAnswerRPC", PhotonTargets.Others, userName, returnString);
    }

    //Return to player
    [PunRPC]
    void ReturnRejoinAnswerRPC(string userName, string ans)
    {
        ThisPlayerScript thisPlayerScript = GetComponent<ThisPlayerScript>();
        
        print(userName + " --- " + thisPlayerScript.userName);
        if (ans == "null")
        {
            PhotonNetwork.LeaveRoom();
            ChangeLayout.ChangeCurrentLayout("Lobby_Parent");
            ClientUIController uIController = GetComponent<ClientUIController>();
            uIController.ShowPopupMessage("Session already started", "You have been removed from the game, as this game is already in session.");
        }


        else
        {

            if (userName == thisPlayerScript.userName)
            {

                thisPlayerScript.SetTheReturnedAlliance(ans);

                ChangeLayout.ChangeCurrentLayout("ClientView_Parent");

                thisPlayerScript.currentSelectedNation = thisPlayerScript.nationsInThisAlliance[0];
                thisPlayerScript.ChangedNationAdjustUI();
            }
        }
    }

    //Called by host
    [PunRPC]
    public void HostRemoveSelectedAlliance(string alliance, string userName)
    {
        foreach(string s in unpickedAlliances)
        {
            if(s == alliance)
            {
                foreach(Nation n in initializer.nationsList)
                {
                    if(s == n.alliance.ToString())
                    {
                        n.AIControlled = false;
                        n.playerUserName = userName;
                        Debug.Log("Removing " + n.name + "s AI");
                    }
                }
                unpickedAlliances.Remove(s);
                break;
            }
        }
        GetComponent<PhotonView>().RPC("InitiateAllPlayerRefresh", PhotonTargets.All);
    }

    [PunRPC]
    public void InitiateAllPlayerRefresh()
    {
        ThisPlayerScript tps = GetComponent<ThisPlayerScript>();
         GetUnselectedAlliances(tps.userName);
    }

    //Called by clients
    //Return the refreshed alliance list to client
    [PunRPC]
    public void ReturnUnselectedAlliancesToClient(string username, string[] unselectedAlliances)
    {
        ThisPlayerScript tps = GetComponent<ThisPlayerScript>();
        if(username == tps.userName)
        {
            tps.unselectedAlliances = unselectedAlliances;
            tps.CompleteRefresh();
        }
    }

    //Called by all players
    [PunRPC]
    public void FetchNationDataRPC(string nationName)
    {
        //Will be changed to true if the nation is in this player's alliance
        bool mynation = false;
        Nation nat = new Nation();
        float[] data = new float[9];
        //Get player script to check alliance
        ThisPlayerScript tps = GetComponent<ThisPlayerScript>();
        //Search for the nation
        foreach(Nation n in tps.nationsInThisAlliance)
        {
            if(n.name == nationName)
            {
                mynation = true;
                nat = n;
                break;
            }
        }

        //Will only do this point if the nation to be checked is in the alliance of this player
        if (mynation)
        {
            data[0] = nat.nationalSpending;
            data[1] = nat.climateAdaptationSpent;
            data[2] = nat.climateMitigationSpent;
            data[3] = nat.climateAdaptationSpent;
            data[4] = nat.budgetAvailable;

            GetComponent<PhotonView>().RPC("ReturnDataToMasterRPC", PhotonTargets.MasterClient, nat.name, data);
        }
    }

    [PunRPC]
    void SetICFBonusRPC(bool goalPassed)
    {
        ThisPlayerScript thisPlayer = GetComponent<ThisPlayerScript>();
        ClientUIController uIController = GetComponent<ClientUIController>();

        List<string> eligibleICFCountries = new List<string>();
        foreach (Nation n in thisPlayer.nationsInThisAlliance)
        {
            if (goalPassed)
            {
                if (n.canWithdraw)
                {
                    n.ICFBonus = true;
                    eligibleICFCountries.Add(n.name);
                }
                else
                    n.ICFBonus = false;
            }
            else
                n.ICFBonus = false;
        }


        if (goalPassed)
            if (eligibleICFCountries.Count > 0)
            {
                string msg = "Due to the ICF goal being reached, the following countries in your alliance have recieved a bonus to climate adaptation:";

                foreach (string s in eligibleICFCountries)
                    msg += "\n" + s;
                uIController.ShowPopupMessage("Adaptation Bonus", msg);
            }
            else uIController.ShowPopupMessage("Adaptation Bonus", "The ICF goal was reached, but no countries in your alliance are eligible for the bonus");



    }

    [PunRPC]
    void ResetSlidersRPC()
    {
        GameObject[] sliders = GameObject.FindGameObjectsWithTag("Slider");

        foreach(GameObject g in sliders)
        {
            g.GetComponent<SliderSnapScript>().ResetSlider();
        }
    }

    [PunRPC]
    void ReturnDataToMasterRPC(string nation, float[] data)
    {
        hostUiController.currentViewableData = data;
        hostUiController.currentViewableNation = nation;

        hostUiController.RecievedData();
    }

    public void ConnectedSendStats()
    {
        //This player script
        ThisPlayerScript thisPlayer = GetComponent<ThisPlayerScript>();
        //Each spent value for each nation
        float[] nationOneData = new float[9];
        float[] nationTwoData = new float[9];
        float[] nationThreeData = new float[9];

        string allianceName = thisPlayer.thisPlayerAlliance;

        GetComponent<PhotonView>().RPC("OnConnectShareValuesRPC", PhotonTargets.MasterClient, nationOneData, nationTwoData, nationThreeData, allianceName);
    }

    //Called to then RPC to host client
    public void AskHostForAlliance()
    {
        //RPC only to host client
        GetComponent<PhotonView>().RPC("HostGiveBackAlliance", PhotonTargets.MasterClient, PhotonNetwork.playerName, 1);
    }

    [PunRPC]
    void IncreaseYearRPC()
    {
        currentYear += 5;
    }

    //Called by player
    public void GetGDPLeaderboardRank(string natName)
    {
        GetComponent<PhotonView>().RPC("HostReturnGDPLeaderboardRank", PhotonTargets.MasterClient, natName, PhotonNetwork.playerName);
    }

    //Host
    [PunRPC]
    public void HostReturnGDPLeaderboardRank(string natName, string userName)
    {
        int rank = 1;

        foreach(Nation n in GDPLeaderboard)
        {
            if (n.name == natName)
                break;
            else
                rank++;
        }

        GetComponent<PhotonView>().RPC("ReturnGDPRankToHost", PhotonTargets.Others, rank, userName);
    }

    //Player recieved
    [PunRPC]
    public void ReturnGDPRankToHost(int rank, string userName)
    {
        if (userName == PhotonNetwork.playerName)
        {
            GameObject.Find("GDP&Rank_Txt").GetComponent<Text>().text += "\nRank: " + rank;

            if (rank == 1 || rank == 21 || rank == 31)
                GameObject.Find("GDP&Rank_Txt").GetComponent<Text>().text += "st";
            else if (rank == 2 || rank == 22 || rank == 32)
                GameObject.Find("GDP&Rank_Txt").GetComponent<Text>().text += "nd";
            else if (rank == 3 || rank == 23 || rank == 33)
                GameObject.Find("GDP&Rank_Txt").GetComponent<Text>().text += "rd";
            else
                GameObject.Find("GDP&Rank_Txt").GetComponent<Text>().text += "th";
        }
    }


    //ONLY CALLED BY HOST
    [PunRPC]
    void HostGiveBackAlliance(string userName, int j)
    {
        //Randomly pick nation
        string allianceName = unpickedAlliances[UnityEngine.Random.Range(0, unpickedAlliances.Count)];

        //Remove the seleected nation from the pool of available nations
        foreach (string s in unpickedAlliances)
            if (s == allianceName)
            {
                unpickedAlliances.Remove(s);
                break;
            }

        //Send to all clients
        GetComponent<PhotonView>().RPC("RecievedNationNowAssign", PhotonTargets.All, allianceName, userName);
    }

    //Will be recieved by all clients
    [PunRPC]
    void RecievedNationNowAssign(string allianceName, string userName)
    {
        //Refresh alliance list

        //Only continue if the ID is the same ID which originally contacted the master client
        if (PhotonNetwork.playerName == userName)
        {
            // Get the script that stores player data
            ThisPlayerScript thisPlayer = GetComponent<ThisPlayerScript>();
            //Return the nation name
            thisPlayer.SetTheReturnedAlliance(allianceName);

            foreach (Nation n in initializer.nationsList)
            {
                Debug.Log(n.name + n.AIControlled);
            }
        }
    }

    [PunRPC]
    public void GeneralUIUpdate()
    {
        ThisPlayerScript tps = GetComponent<ThisPlayerScript>();
    }

    [PunRPC]
    public void ChangeStateToPlayingRPC()
    {
        state = STATE.PLAYING;
        GetComponent<ClientUIController>().ShowPopupMessage(currentYear + " negotiations", "A new round has started, you may now configure your alliance's spending");
    }

    [PunRPC]
    public void PreGameToSetupRPC()
    {
        state = STATE.SETUP;
    }

    [PunRPC]
    public void ResetSpendingRPC()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            ThisPlayerScript thisPlayerScript = GetComponent<ThisPlayerScript>();
            foreach (Nation n in thisPlayerScript.nationsInThisAlliance)
            {
                n.budgetMax = n.budget;
                n.budgetAvailable = n.budgetMax;
                
            }

            float[] ICFToSendToHost = new float[3];
            ICFToSendToHost[0] = thisPlayerScript.nationsInThisAlliance[0].icfSpent;
            ICFToSendToHost[1] = thisPlayerScript.nationsInThisAlliance[1].icfSpent;
            ICFToSendToHost[2] = thisPlayerScript.nationsInThisAlliance[2].icfSpent;


            //GetComponent<PhotonView>().RPC("SendICFFundsToHostRPC", PhotonTargets.MasterClient, ICFToSendToHost);
        }
    }

    //[PunRPC]
    //private void SendICFFundsToHostRPC(float[] nationsICFSpent)
    //{
    //    Debug.Log("Recieving ICF Funds");
    //    foreach(float f in nationsICFSpent)
    //    {
    //        ICF += (int)f;
    //    }
    //}

    //[PunRPC]
    //public void SubmitSpendingRPC(float[] nationOne, float[] nationTwo, float[] nationThree, string allianceName)
    //{
    //    print(allianceName);
    //    foreach (Nation n in initializer.nationsList)
    //    {
    //        Debug.Log(n.name + n.AIControlled);
    //    }
    //    //Get host ui on master client
    //    HostUiController hostUiController = GetComponent<HostUiController>();
    //    //Pass in values to host to display on host view
    //    hostUiController.SyncSumbittedValuesToHost(nationOne, nationTwo, nationThree, allianceName);
    //    //pass values to world status controller
    //    WorldStatusController WorldStatusController = GetComponent<WorldStatusController>();
    //    //WorldStatusController.SyncSumbittedValuesToHost(nationOne, nationTwo, nationThree, allianceName);

    //    readyCount++;
    //    if (readyCount >= playerCount)
    //    {
    //        GameObject.Find("NextRound_Btn").GetComponent<Button>().interactable = true;
    //    }

    //    //Log to show that this alliance has submitted
    //    GMLogScript logScript = GetComponent<GMLogScript>();
    //    logScript.SendNewLogData(allianceName + " have submitted their spending.");

    //}

    [PunRPC]
    public void IncreaseReadyCount()
    {
        readyCount++;

        if (readyCount >= playerCount)
        {
            GameObject.Find("NextRound_Btn").GetComponent<Button>().interactable = true;
        }
    }

    [PunRPC]
    //Submit Player Spending
    public void SubmitClientSpend(string nationName, float[] nationSpending)
    {
        ThisPlayerScript tps = this.GetComponent<ThisPlayerScript>();
        foreach (Nation nHost in initializer.nationsList)
        {
            if (nationName == nHost.name)
            {
                nHost.nationalSpending = nationSpending[0];
                nHost.climateAdaptationSpent = nationSpending[1];
                nHost.climateMitigationSpent = nationSpending[2];
                nHost.climateTechSpent = nationSpending[3];
                nHost.icfSpent = nationSpending[4];

                nHost.totalNationSpending += nHost.nationalSpending;
                nHost.totalMitigation += nHost.climateMitigationSpent;
                nHost.totalTech += nHost.climateTechSpent;
                nHost.totalAdaptation += nHost.climateAdaptationSpent;
                nHost.totalIcf += nHost.icfSpent;

                AddICFTotalThisRound(nHost.icfSpent);

                break;
            }
        }

        
    }

    void AddICFTotalThisRound(float icfToAdd)
    {
        currentICFSpent += icfToAdd;

        GameObject.Find("CurrentICFSpent_Txt").GetComponent<Text>().text = "$" + currentICFSpent.ToString("0.0") + "bn";

        if (currentICFSpent >= 400)
            GameObject.Find("CurrentICFSpent_Txt").GetComponent<Text>().text = "ICF goal achieved!";
    }

    [PunRPC]
    //Called on host at start
    public void OnConnectShareValuesRPC(float[] nationOne, float[] nationTwo, float[] nationThree, string allianceName)
    {
        if(PhotonNetwork.isMasterClient)
        {
            hostUiController.SyncSumbittedValuesToHost(nationOne, nationTwo, nationThree, allianceName);
        }
    }


    [PunRPC]
    public void ResetSpentValuesRPC()
    {
        ThisPlayerScript thisPlayerScript = GetComponent<ThisPlayerScript>();

        //Reset each value and set budget avail to max budget
        foreach(Nation n in thisPlayerScript.nationsInThisAlliance)
        {
            n.nationalSpending = 0;
            n.climateMitigationSpent = 0;
            n.climateAdaptationSpent = 0;
            n.icfSpent = 0;

            n.budgetAvailable = n.budgetMax;
        }

        //Reset text and slider value
        //foreach(GameObject g in thisPlayerScript.sliderUiParents)
        //{
        //    g.transform.GetChild(3).GetComponent<Slider>().value = 0;
        //    g.transform.GetChild(1).GetComponent<InputField>().text = "0";
        //}

        //Reactivate submit button
        submitBtn.interactable = true;
       // ICFBtn.interactable = true;
    }

    [PunRPC]
    public void ReturnEventEffects(string nation)
    {
        ThisPlayerScript tps = GetComponent<ThisPlayerScript>();
        //if this player is the alliance we are looking for
        foreach(Nation n in tps.nationsInThisAlliance)
        {
            if(n.name == nation)
            {
                //Fill in effects
            }
        }
    }
    
    [PunRPC]
    public void UpdateICFRPC(float value)
    {
        //update ICF on host
        ICF -= (int)value;
    }

    [PunRPC]
    public void UpdateTurnRPC(int newTurn)
    {
        turn = newTurn;
    }

    [PunRPC]
    public void ClientRecieveNewScoresRPC(float[] nationOne, float[] nationTwo, float[] nationThree, string alliance)
    {
        ThisPlayerScript tps = GetComponent<ThisPlayerScript>();

    }

    [PunRPC]
    public void UpdateNationData(string name, float gdp, float emissions, float socialCostOfCarbon, float budget, float population,
        float totalDamage, string alliance)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            ThisPlayerScript tps = GetComponent<ThisPlayerScript>();

            if (alliance == tps.thisPlayerAlliance)
            {
                foreach (Nation n in tps.nationsInThisAlliance)
                {
                    if (name == n.name)
                    {
                        n.gdp[turn] = gdp;
                        n.totalEmissions[turn] = emissions;
                        //n.socialCostOfCarbon[turn] = socialCostOfCarbon;
                        n.temperature = worldTemp;
                        n.budget = budget;
                        n.labour[turn] = population;
                        n.totalDamage[turn] = totalDamage;

                    }
                }
            }
        }
    }

    public void CountryAI()
    {
        rng = new System.Random();
        float maxBudget;
        float spentbudget = 0;


        foreach (Nation n in initializer.nationsList)
        {
            if (n.AIControlled == true)           //only runs AI if there is no player controlling it
            {

                switch (n.behaviour)              //different behaviours for diferent peronalities 
                {
                    case (Nation.Personality.Selfish):  //will focus on national spending
                        {
                            maxBudget = n.budget;
                            spentbudget = 0;

                            if ((n.totalNationSpending / n.totalSpent) < (1.2 * (AverageTotalNationalSpent() / AverageTotalSpent())))
                            {
                                n.nationalSpending = rng.Next((int)maxBudget * (3 / 5), (int)maxBudget);
                            }
                            else
                            {
                                n.nationalSpending = rng.Next(0, (int)maxBudget);
                            }
                            spentbudget += n.nationalSpending;

                            n.climateAdaptationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateAdaptationSpent;

                            n.climateMitigationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateMitigationSpent;

                            n.climateTechSpent = rng.Next(0, (int)n.climateMitigationSpent);
                            n.climateMitigationSpent = n.climateMitigationSpent - n.climateTechSpent;
                            n.icfSpent = maxBudget - spentbudget;

                            spentbudget = 0;
                        }
                        break;

                    case (Nation.Personality.Altrusistic):   //will focus on ICF
                        {
                            maxBudget = n.budget;
                            spentbudget = 0;
                            if ((n.totalIcf / n.totalSpent) < (1.2 * (AverageICFSpent() / AverageTotalSpent())))
                            {
                                n.icfSpent = rng.Next((int)maxBudget * (3 / 5), (int)maxBudget);
                            }
                            else
                            {
                                n.icfSpent = rng.Next(0, (int)maxBudget);
                            }
                            spentbudget += n.icfSpent;

                            n.climateAdaptationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateAdaptationSpent;

                            n.climateMitigationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateMitigationSpent;

                            n.climateTechSpent = rng.Next(0, (int)n.climateMitigationSpent);
                            n.climateMitigationSpent = n.climateMitigationSpent - n.climateTechSpent;
                            n.nationalSpending = maxBudget - spentbudget;

                            spentbudget = 0;
                        }
                        break;

                    case (Nation.Personality.Mitigation):    //will focus on Mitigation
                        {
                            maxBudget = n.budget;
                            spentbudget = 0;

                            if ((n.totalMitigation / n.totalSpent) < (1.2 * (AverageMitigationSpent() / AverageTotalSpent())))
                            {
                                n.climateMitigationSpent = rng.Next((int)maxBudget * (3 / 5), (int)maxBudget);
                            }
                            else
                            {
                                n.climateMitigationSpent = rng.Next(0, (int)maxBudget);
                            }
                            spentbudget += n.climateMitigationSpent;
                            n.climateTechSpent = rng.Next(0, (int)(n.climateMitigationSpent / 3));
                            n.climateMitigationSpent = n.climateMitigationSpent - n.climateTechSpent;

                            n.climateAdaptationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateAdaptationSpent;
                            n.nationalSpending = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.nationalSpending;
                            n.icfSpent = maxBudget - spentbudget;

                            spentbudget = 0;
                        }
                        break;

                    case (Nation.Personality.Balance):      //will try to keep spending balanced, about a quarter of the budget for each area with tech and mitigation being split in about half
                        {
                            maxBudget = n.budget;
                            spentbudget = 0;
                            n.climateAdaptationSpent = rng.Next((int)((maxBudget / 4) * 0.9), (int)((maxBudget / 4) * 1.1));
                            spentbudget += n.climateAdaptationSpent;

                            n.climateMitigationSpent = rng.Next((int)((maxBudget / 4) * 0.9), (int)((maxBudget / 4) * 1.1));
                            spentbudget += n.climateMitigationSpent;

                            n.climateTechSpent = rng.Next((int)((n.climateMitigationSpent / 2) * 0.9), (int)((n.climateMitigationSpent / 2) * 1.1));
                            n.climateMitigationSpent = n.climateMitigationSpent - n.climateTechSpent;
                            n.icfSpent = rng.Next((int)((maxBudget / 4) * 0.9), (int)((maxBudget / 4) * 1.1));
                            spentbudget += n.icfSpent;
                            n.nationalSpending = maxBudget - spentbudget;

                            spentbudget = 0;
                        }
                        break;

                    case (Nation.Personality.Tech):      //will focus on tech by prioritising mitigation then spending at least 2/3 of that on tech
                        {
                            maxBudget = n.budget;
                            spentbudget = 0;

                            if ((n.totalMitigation / n.totalSpent) < (1.2 * (AverageMitigationSpent() / AverageTotalSpent())))
                            {
                                n.climateMitigationSpent = rng.Next((int)maxBudget * (3 / 5), (int)maxBudget);
                            }
                            else
                            {
                                n.climateMitigationSpent = rng.Next(0, (int)maxBudget);
                            }
                            spentbudget += n.climateMitigationSpent;
                            n.climateTechSpent = rng.Next((int)((n.climateMitigationSpent / 3) * 2), (int)n.climateMitigationSpent);
                            n.climateMitigationSpent = n.climateMitigationSpent - n.climateTechSpent;

                            n.climateAdaptationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateAdaptationSpent;
                            n.nationalSpending = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.nationalSpending;
                            n.icfSpent = maxBudget - spentbudget;

                            spentbudget = 0;
                        }
                        break;
                    case (Nation.Personality.Adaptation):  //will focus on adaptation
                        {
                            maxBudget = n.budget;
                            spentbudget = 0;

                            if ((n.totalAdaptation / n.totalSpent) < (1.2 * (AverageAdaptationSpent() / AverageTotalSpent())))
                            {
                                n.climateAdaptationSpent = rng.Next((int)maxBudget * (3 / 5), (int)maxBudget);
                            }
                            else
                            {
                                n.climateAdaptationSpent = rng.Next(0, (int)maxBudget);
                            }
                            spentbudget += n.climateAdaptationSpent;

                            n.nationalSpending = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.nationalSpending;

                            n.climateMitigationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateMitigationSpent;

                            n.climateTechSpent = rng.Next(0, (int)n.climateMitigationSpent);
                            n.climateMitigationSpent = n.climateMitigationSpent - n.climateTechSpent;
                            n.icfSpent = maxBudget - spentbudget;

                            spentbudget = 0;
                        }
                        break;
                    default:     //here in case the personality system somehow breaks mid game so the ai will still run
                        {
                            maxBudget = n.budget;
                            spentbudget = 0;
                            n.climateAdaptationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateAdaptationSpent;

                            n.climateMitigationSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.climateMitigationSpent;

                            n.climateTechSpent = rng.Next(0, (int)n.climateMitigationSpent);
                            n.climateMitigationSpent = n.climateMitigationSpent - n.climateTechSpent;
                            n.icfSpent = rng.Next(0, (int)(maxBudget - spentbudget));
                            spentbudget += n.icfSpent;
                            n.nationalSpending = maxBudget - spentbudget;

                            spentbudget = 0;
                        }
                        break;
                }

                n.totalNationSpending += n.nationalSpending;
                n.totalMitigation += n.climateMitigationSpent;
                n.totalTech += n.climateTechSpent;
                n.totalAdaptation += n.climateAdaptationSpent;
                n.totalIcf += n.icfSpent;

                Debug.Log(n.name + " ");
                Debug.Log("Adaptation:" + n.climateAdaptationSpent);
                Debug.Log("Mitigation:" + n.climateMitigationSpent);
                Debug.Log("Mitgation 2:" + n.climateMitigationSpent);
                Debug.Log("National:" + n.nationalSpending);
            }
        }
        AIPersonalityUpdate();
    }

    //updates the nations total money spent throughout the game
    public void UpdatePlayerNationTotalSpent()
    {
        foreach (Nation n in initializer.nationsList)
        {
            if (n.AIControlled == false)
            {
                n.totalSpent += n.totalNationSpending;
                n.totalSpent += n.totalMitigation;
                n.totalSpent += n.totalTech;
                n.totalSpent += n.totalAdaptation;
                n.totalSpent += n.totalIcf;
            }
        }
    }

    public void UpdateAINationTotalSpent()
    {
        foreach (Nation n in initializer.nationsList)
        {
            if (n.AIControlled == true)
            {
                n.totalSpent += n.totalNationSpending;
                n.totalSpent += n.totalMitigation;
                n.totalSpent += n.totalTech;
                n.totalSpent += n.totalAdaptation;
                n.totalSpent += n.totalIcf;
            }
        }
    }

    //finds the average ampunt spent in total
    public float AverageTotalSpent()
    {
        float average = 0;
        float total = 0;
        float natCount = 0;

        foreach (Nation n in initializer.nationsList)
        {
            total += n.totalSpent;
            natCount++;
        }

        average = total / natCount;

        return average;
    }

    //finds the average total spent on nationall spending
    public float AverageTotalNationalSpent()
    {
        float TotalNatSpent = 0;
        float AverageTotalNatSpent = 0;
        float natCount = 0;

        foreach (Nation n in initializer.nationsList)
        {
            TotalNatSpent += n.totalNationSpending;
            natCount++;
        }

        AverageTotalNatSpent = TotalNatSpent / natCount;

        return AverageTotalNatSpent;
    }

   // finds the average total spend on icf
    public float AverageICFSpent()
    {
        float TotalIcfSpent = 0;
        float AverageIcfSpent = 0;
        float natCount = 0;

        foreach (Nation n in initializer.nationsList)
        {
            TotalIcfSpent += n.totalIcf;
            natCount++;
        }

        AverageIcfSpent = TotalIcfSpent / natCount;

        return AverageIcfSpent;
    }

    //finds the average total spend on mitigation
    public float AverageMitigationSpent()
    {
        float TotalMitigationSpent = 0;
        float AveraceMitSpent = 0;
        float natCount = 0;

        foreach (Nation n in initializer.nationsList)
        {
            TotalMitigationSpent += n.totalMitigation;
            natCount++;
        }

        AveraceMitSpent = TotalMitigationSpent / natCount;

        return AveraceMitSpent;
    }

    //finds the average total spend on adaptation
    public float AverageAdaptationSpent()
    {
        float TotalAdaptationSpent = 0;
        float AverageAdaptation = 0;
        float natCount = 0;

        foreach (Nation n in initializer.nationsList)
        {
            TotalAdaptationSpent += n.totalAdaptation;
            natCount++;
        }

        AverageAdaptation = TotalAdaptationSpent / natCount;

        return AverageAdaptation;
    }

    //finds the total spend on tech
    public float totalTechSpent()
    {
        float techThisTurn = 0;

        foreach (Nation n in initializer.nationsList)
        {
            techThisTurn += n.totalTech;
        }
        return techThisTurn;
    }

    public void AIPersonalityUpdate()
    {
        foreach (Nation n in initializer.nationsList)
        {
            if ((n.totalNationSpending / n.totalSpent) < (n.natSpentThreshold * (AverageTotalNationalSpent() / AverageTotalSpent())))
            {
                n.behaviour = Nation.Personality.Selfish;
            }
            else
            if ((n.totalIcf / n.totalSpent) < (n.icfThreshold * (AverageICFSpent() / AverageTotalSpent())))
            {
                n.behaviour = Nation.Personality.Altrusistic;
            }
            else
            if ((n.totalMitigation / n.totalSpent) < (n.mitigationThreshold * (AverageMitigationSpent() / AverageTotalSpent())))
            {
                n.behaviour = Nation.Personality.Mitigation;
            }
            else
            if ((n.totalAdaptation / n.totalSpent) < (n.adaptThreshold * (AverageAdaptationSpent() / AverageTotalSpent())))
            {
                n.behaviour = Nation.Personality.Adaptation;
            }
            else
            if (n.totalTech < (n.totalMitigation * n.techThreshold))
            {
                n.behaviour = Nation.Personality.Tech;
            }
            else
            {
                n.behaviour = Nation.Personality.Balance;
            }
        }
    }

    [PunRPC]
    public void CountryAiSelectionOn(string n)
    {
        foreach (Nation nat in initializer.nationsList)
        {
            if (n == nat.name)
            {
                nat.AIControlled = true;
                Debug.Log(nat.name + nat.AIControlled);
            }
        }
    }

    [PunRPC]
    public void CountryAiSelectionOff(string n)
    {
        foreach (Nation nat in initializer.nationsList)
        {
            if (n == nat.name)
            {
                nat.AIControlled = false;
                Debug.Log(nat.name + nat.AIControlled);
            }
        }
    }
}
