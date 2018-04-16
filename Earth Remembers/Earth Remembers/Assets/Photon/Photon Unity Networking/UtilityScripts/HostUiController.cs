using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HostUiController : MonoBehaviour {
    const int nationsPerAlliance = 3;

    GameObject previousBtn;
    public GameObject drawEventsBtn;
    public GameObject previousBtnAOSIS;
    public GameObject previousBtnEU;
    public GameObject previousBTNBASIC;
    public GameObject previousBtnUmbrella;
    //below: [0] = economy [1] = Climate negotiation [2] emmisions reduced [3]  national climate fund
    //4 - budget, 5 - economy score, 6 - economy mult, 7 - co2 output, 8 - co2 mult

    public float[] currentViewableData = new float[8];
    public Text[] NationSelectBtns;
    public string currentViewableNation;

    float[] Barbadosdata = new float[9];
    float[] DominicanRepublicdata = new float[9];
    float[] Singaporedata = new float[9];
    float[] Francedata = new float[9];
    float[] Swedendata = new float[9];
    float[] Maltadata = new float[9];
    float[] Brazildata = new float[9];
    float[] Chinadata = new float[9];
    float[] Indiadata = new float[9];
    float[] Canadadata = new float[9];
    float[] NZdata = new float[9];
    float[] USAdata = new float[9];

    public Text[] HostAllianceViewTextAssets;

    public Image AOSISPin, UMBRELLAPin, EUPin, BASICPin;
    public Sprite[] hostTabs;
    //The world status controller script
    WorldStatusController worldStatusController;

    Temperature tempScript;
    //Event controlling script
    EventsScript eventsScript;
    // Use this for initialization
    void Start () {

        worldStatusController = GetComponent<WorldStatusController>();
        tempScript = GetComponent<Temperature>();
        //GetExtraTextObjects();

        NationSelectBtns = new Text[nationsPerAlliance];

        
    }
	
    void GetExtraTextObjects()
    {
        //AOSISData will be renamed
        HostAllianceViewTextAssets[5] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(8).GetChild(2).GetComponent<Text>();
        HostAllianceViewTextAssets[6] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(8).GetChild(3).GetComponent<Text>();
        HostAllianceViewTextAssets[7] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(8).GetChild(5).GetComponent<Text>();
        HostAllianceViewTextAssets[8] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(8).GetChild(6).GetComponent<Text>();
        HostAllianceViewTextAssets[9] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(8).GetChild(0).GetComponent<Text>();
        HostAllianceViewTextAssets[10] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(7).GetChild(0).GetComponent<Text>();
        HostAllianceViewTextAssets[11] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(7).GetChild(2).GetComponent<Text>();
        HostAllianceViewTextAssets[12] = GameObject.Find("HostAllianceView_Parent").transform.GetChild(7).GetChild(4).GetComponent<Text>();
    }

    public void SyncSumbittedValuesToHost(float[] nationOne, float[] nationTwo, float[] nationThree, string allianceName)
    {
        switch (allianceName)
        {
            case "AOSIS":
                Barbadosdata = nationOne;
                DominicanRepublicdata = nationTwo;
                Singaporedata = nationThree;
                break;

            case "EU":
                Swedendata = nationOne;
                Francedata = nationTwo;
                Maltadata = nationThree;
                break;

            case "BASIC":
                Brazildata = nationOne;
                Chinadata = nationTwo;
                Indiadata = nationThree;
                break;

            case "UmbrellaGroup":
                Canadadata = nationOne;
                NZdata = nationTwo;
                USAdata = nationThree;
                break;

            default:
                Debug.LogError("No Alliance found");
                break;
        }
    }

    public void StartBtn() // code for the start button
    {
        if (PhotonNetwork.isMasterClient)
        {
            worldStatusController.SetUpGame();
            eventsScript = GetComponent<EventsScript>();
            //worldStatusController.MovePlayersToAllianceSelect();

            worldStatusController.state = WorldStatusController.STATE.SETUP;
            worldStatusController.PreGameToSetup();

            ToMain();
        }
    }

    //Called when prompted to draw the events
    public void DrawEventsBtm()
    {
        drawEventsBtn = EventSystem.current.currentSelectedGameObject.gameObject;
        //Hide button until next round is pressed
        drawEventsBtn.GetComponent<Button>().interactable = false;
        //Start the process for getting new events
        //eventsScript.GetNewEventsProcess();
        worldStatusController.SetupToPlaying();
    }

    public void ToMain()
    {
        ChangeLayout.ChangeCurrentLayout("HostAllianceSelection_Parent");                        //changes the ui layout to the HostMainView
        GameStateController.STATE = GameStateController.GAMESTATE.PLAYING;        //Changes the gamestate to playing
    }

    public void Selectnation()
    {
        GameObject clickedBtn = EventSystem.current.currentSelectedGameObject;
        string clickedNationName = clickedBtn.GetComponent<Text>().text;
        clickedBtn = clickedBtn.transform.parent.gameObject;

        if (clickedBtn.transform.GetChild(2).GetComponent<Text>().text != clickedNationName)
        {
            if(clickedBtn.transform.GetChild(3).GetComponent<Text>().text == clickedNationName)
            {
                string natName0 = clickedBtn.transform.GetChild(2).GetComponent<Text>().text;
                string natName1 = clickedBtn.transform.GetChild(3).GetComponent<Text>().text;
                string natName2 = clickedBtn.transform.GetChild(4).GetComponent<Text>().text;

                clickedBtn.transform.GetChild(2).GetComponent<Text>().text = clickedNationName;
                clickedBtn.transform.GetChild(3).GetComponent<Text>().text = natName0;
                clickedBtn.transform.GetChild(4).GetComponent<Text>().text = natName2;
            }
            if (clickedBtn.transform.GetChild(4).GetComponent<Text>().text == clickedNationName)
            {
                string natName0 = clickedBtn.transform.GetChild(2).GetComponent<Text>().text;
                string natName1 = clickedBtn.transform.GetChild(3).GetComponent<Text>().text;
                string natName2 = clickedBtn.transform.GetChild(4).GetComponent<Text>().text;


                clickedBtn.transform.GetChild(2).GetComponent<Text>().text = clickedNationName;
                clickedBtn.transform.GetChild(3).GetComponent<Text>().text = natName0;
                clickedBtn.transform.GetChild(4).GetComponent<Text>().text = natName1;
            }


            clickedBtn.transform.GetChild(2).GetComponent<Text>().text = clickedNationName;
        }

        Invoke(ReformString(clickedNationName), 0.05f);

    }

    string ReformString(string nameToReform)
    {
        string newString = nameToReform.Replace(' ','_');



        return newString;

    }

    public void Resetprevious()
    {
        //previousBtn.GetComponent<Text>().alignment = TextAnchor.LowerCenter;
    }

    public void ToLeaderboard()
    {
        ChangeLayout.ChangeCurrentLayout("Leaderboard_Parent");
    }

    public void ApplyEventBtn()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;

        eventsScript.ApplyEvent(clickedObj.GetComponent<ThisEvent>().thisWorldEvent, clickedObj);
    }

    public void OpenEventInfoBtn()
    {
        eventsScript.ShowEventInfo(EventSystem.current.currentSelectedGameObject.GetComponent<ThisEvent>().thisWorldEvent);
    }

    public void CloseEventInfoBtn()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }

    public void Click_ScoresView()
    {
        //Get buttton pressed
        GameObject pressedBtn = EventSystem.current.currentSelectedGameObject;
        //get parent of parent - The Host screen for this alliance, enable the scores view child 
        //Unhide
        pressedBtn.transform.parent.transform.parent.GetChild(8).transform.localScale = new Vector3(1, 1, 1); 
    }

    public void Click_TargetsView()
    {
        //Get button pressed
        GameObject pressedBtn = EventSystem.current.currentSelectedGameObject;
        //Get the parent target view
        GameObject targetViewParent = pressedBtn.transform.parent.transform.parent.GetChild(7).gameObject;
        //Unhide target view parent
        targetViewParent.transform.localScale = new Vector3(1, 1, 1);

        
    }

    public void Click_ReturnToAllianceView()
    {
        //Get button pressed
        GameObject pressedBtn = EventSystem.current.currentSelectedGameObject;

        //Hide both Scores and Target Screen
        pressedBtn.transform.parent.transform.parent.GetChild(7).transform.localScale = new Vector3(0,0,0);
        pressedBtn.transform.parent.transform.parent.GetChild(8).transform.localScale = new Vector3(0,0,0);

    }

    public void Click_ICF()
    {
        //Show ICF page 
        GameObject.Find("ICFHostView").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("AlianceSelectionScreen").transform.localScale = new Vector3(1, 1, 1);


        GameObject.Find("ICFHostView").transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "$" + worldStatusController.ICF + "m";

        GameObject.Find("HostAllianceView_Parent").transform.localScale = new Vector3(0, 0, 0);
        GameObject.Find("HostEUScreen").transform.localScale = new Vector3(0, 0, 0);
        GameObject.Find("HostUMBRELLAScreen").transform.localScale = new Vector3(0, 0, 0);
        GameObject.Find("HostBASICScreen").transform.localScale = new Vector3(0, 0, 0);
    }

    public void Click_EventsPage()
    {
        //Show word map with pins
        
    }

    public void Click_ReturnFromICF()
    {
        GameObject.Find("ICFHostView").transform.localScale = new Vector3(0, 1, 1);

    }



    //Below is new*
    
    //Call this function to display data for a particular country
    public void ShowNationData()
    {
        //Nation selected
        string nationName;
        //Get nation name, so get button clicked for switching country view or use the starter nation for an alliance click
        nationName = GetSelectedNationToDisplay(EventSystem.current.currentSelectedGameObject);

        //Fetch teh data
        worldStatusController.FetchViewableData(nationName);

        //*******
        //Rename AOSIS to inspection
        ChangeLayout.ChangeCurrentLayout("HostAllianceView_Parent");
    }

    public void RecievedData()
    {
        VisuallyShowData(currentViewableData, currentViewableNation);
    }

    void VisuallyShowData(float[] data, string natName)
    {
        //Get init scripts
        Initializer init = GetComponent<Initializer>();
        //Get nations list
        List<Nation> natList = new List<Nation>();
        Nation nat = new Nation();

        foreach(Nation n in init.nationsList)
        {
            print(natName);
            if (n.name == natName)
            {
                nat = n;
                break;
            }
        }

        foreach(Nation n in init.nationsList)
        {
            if(n.alliance == nat.alliance)
            {
                print(n.alliance + "  " + nat.name);
                natList.Add(n);
            }
        }

        for(int i = 0; i < nationsPerAlliance; i++)
        {
            NationSelectBtns[i].text = natList[i].name;
            print(natList[i].name + " bo");
        }

        HostAllianceViewTextAssets[0].text = data[0].ToString();
        HostAllianceViewTextAssets[1].text = data[1].ToString();
        HostAllianceViewTextAssets[2].text = data[2].ToString();
        HostAllianceViewTextAssets[3].text = data[3].ToString();
        HostAllianceViewTextAssets[4].text = "$" + data[4].ToString() + "m";
        HostAllianceViewTextAssets[5].text = "Economy Score: " +data[5].ToString();
        HostAllianceViewTextAssets[6].text = "Economy Mult: " + data[6].ToString();
        HostAllianceViewTextAssets[7].text = "CO2 Output: " + data[7].ToString();
        HostAllianceViewTextAssets[8].text = "CO2 Mult: " + data[1].ToString();
        //Score screen name display
        HostAllianceViewTextAssets[9].text = natName;

        //Targets view
        HostAllianceViewTextAssets[10].text = natName;

        //AOSISData[11].text = "Nation Target: " + (data[7] / worldStatusController.worldEmissions) * worldStatusController.globalTarget;
        //AOSISData[12].text = "Global Target: " + worldStatusController.globalTarget;


        print(natName);
        AOSISPin.sprite = Resources.Load<Sprite>(natName);


    }

    string GetSelectedNationToDisplay(GameObject buttonClicked)
    {
        string toReturn;
        if (GameObject.Find("HostAllianceView_Parent").transform.localScale.x > 0)
            toReturn = buttonClicked.GetComponent<Text>().text;
        else
            toReturn = buttonClicked.transform.GetChild(0).GetComponent<Text>().text;

        switch (toReturn)
        {
            case ("AOSIS"):
                toReturn = "BARBADOS";
                break;
            case ("EUROPEAN UNION"):
                toReturn = "SWEDEN";
                break;
            case ("BASIC"):
                toReturn = "BRAZIL";
                break;
            case ("UMBRELLA GROUP"):
                toReturn = "CANADA";
                break;
        }

        return toReturn;
    }

    public void SetTechSlider(float spent, float target)
    {
        Slider slider = GameObject.Find("EmissionsTechBonus_Slider").GetComponent<Slider>();

        slider.value = (spent / target) * 100;

        if(spent>=target)
            GameObject.Find("TechSlider_Fill").GetComponent<Image>().color = Color.green;
    }

    //Set up the screens for the host when the game is run
    public void HostScreenStartUp()
    {
        SwitchTab(GameObject.Find("MainContent_Parent"), GameObject.Find("TP_Parent"));
    }

    public void HostScreenTab_Click()
    {
        //Setup required instances
        GameObject clickedBtn = EventSystem.current.currentSelectedGameObject;
        GameObject contentParent = GameObject.Find("MainContent_Parent");
        GameObject contentToShow;

        //Navigate to find appropriate content based on button which was clicked
        switch (clickedBtn.name)
        {
            case "TP_Btn":
                contentToShow = contentParent.transform.GetChild(0).gameObject;
                contentParent.GetComponent<Image>().sprite = hostTabs[0];
                break;

            case "Funding_Btn":
                contentToShow = contentParent.transform.GetChild(1).gameObject;
                contentParent.GetComponent<Image>().sprite = hostTabs[1];
                break;

            case "Leaderboards_Btn":
                contentToShow = contentParent.transform.GetChild(2).gameObject;
                contentParent.GetComponent<Image>().sprite = hostTabs[2];
                break;

            default:
                contentToShow = new GameObject();
                break;
        }
        //Scale content to show the selected content on screen
        SwitchTab(contentParent, contentToShow);
        //Change colour of buttons
       // AdjustButtonColours(clickedBtn);
    }

    void SwitchTab(GameObject parent, GameObject toShow)
    {
        //Cycle through each child, show the content which has been clicked to display, hide others
        foreach(Transform t in parent.transform)
        {
            t.transform.localScale = new Vector2(1, 1);
            if(t != toShow.transform)
                t.transform.localScale = new Vector2(0, 0);
        }
    }

    Color32 selected, unselected;
    void AdjustButtonColours(GameObject clickedBtn)
    {
        //setup colors
        selected = new Color32(197, 197, 197, 255);
        unselected = new Color32(107, 107, 107, 255);

        //Set the buttons not clicked to unselected color, set the button which was clicked to the clicked button
        foreach(Transform t in clickedBtn.transform.parent)
        {
            t.GetComponent<Image>().color = unselected;
            if(t.gameObject == clickedBtn)
            {
                t.GetComponent<Image>().color = selected;
            }
        }
    }

    public void SetHostDisplay()
    {
        //TIPPING POIT SCREEN
        //Current global emissions
        GameObject.Find("CGE_Txt").GetComponent<Text>().text = tempScript.emissions[worldStatusController.turn-1].ToString("0.00") + " Megatons";
        //Tempretire
        GameObject.Find("TempDisplay_Txt").GetComponent<Text>().text = "+" + worldStatusController.worldTemp.ToString("0.00") + "c";
        //Thermometer
        GameObject.Find("Temp_Slider").GetComponent<Slider>().value = worldStatusController.worldTemp;
        //Gross world product
        GameObject.Find("GWP_Txt").GetComponent<Text>().text = "$" + GetGrossWorldProduct(worldStatusController.turn-1).ToString("0.00") + " trillion";

        GameObject.Find("Year_Txt").GetComponent<Text>().text = "Year: " + worldStatusController.currentYear.ToString();

        //FUNDING SCREEN
        GameObject.Find("TotalNET_Txt").GetComponent<Text>().text = "Total: $" + worldStatusController.totalTechSpent() + "m";

        //LEADERBOARDS
        UpdateLeaderboards();
    }

    void UpdateLeaderboards()
    {
        LeaderboardScript leaderboardScript = GetComponent<LeaderboardScript>();

        //SOrt the lists
        List<Nation> adaptationSpent = leaderboardScript.AdaptionTop5(worldStatusController.turn-1);
        List<Nation> GDPLeaderboard = leaderboardScript.GDPTop5(worldStatusController.turn - 1);
        List<Nation> ICFContrib = leaderboardScript.ICFTop5(worldStatusController.turn - 1);

        worldStatusController.GDPLeaderboard = GDPLeaderboard;

        for (int i = 0; i < 5; i++)
        {
            //Adapatation
            GameObject container = GameObject.Find("AdaptationSpending_Parent").transform.GetChild(1).gameObject;
            GameObject listObj = container.transform.GetChild(i).gameObject;
            //Set text and image
            listObj.transform.GetChild(0).GetComponent<Text>().text = adaptationSpent[i].name;
            listObj.transform.GetChild(1).GetComponent<Text>().text = "$" + adaptationSpent[i].totalAdaptation.ToString("0.00") + "bn";
            listObj.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>(adaptationSpent[i].name);

            //GDP
            container = GameObject.Find("GDP_Parent").transform.GetChild(1).gameObject;
            listObj = container.transform.GetChild(i).gameObject;
            //Set text and image
            listObj.transform.GetChild(0).GetComponent<Text>().text = GDPLeaderboard[i].name;
            listObj.transform.GetChild(1).GetComponent<Text>().text = "$" + GDPLeaderboard[i].gdp[worldStatusController.turn-1].ToString("0.00") + "bn";
            listObj.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>(GDPLeaderboard[i].name);

            //ICF
            container = GameObject.Find("ICFContrib_Parent").transform.GetChild(1).gameObject;
            listObj = container.transform.GetChild(i).gameObject;
            //Set text and image
            listObj.transform.GetChild(0).GetComponent<Text>().text = ICFContrib[i].name;
            listObj.transform.GetChild(1).GetComponent<Text>().text = "$" + ICFContrib[i].icfSpent.ToString("0.00") + "bn";
            listObj.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>(ICFContrib[i].name);
        }
    }

    float GetGrossWorldProduct(int turn)
    {
        float toReturn = 0;
        Initializer initScript = GetComponent<Initializer>();
        
        foreach(Nation n in initScript.nationsList)
        {
            toReturn += n.gdp[turn];
        }

        return toReturn;
    }
}

