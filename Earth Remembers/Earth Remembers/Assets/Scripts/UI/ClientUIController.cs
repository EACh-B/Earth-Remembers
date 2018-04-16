using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ClientUIController : MonoBehaviour {
    public Text[] nationNames;
    Text budgetAvailTxt;
    public Image selectedNationPin;
    Image nonPlayingOverlay;
    ThisPlayerScript thisPlayerScript;

    WorldStatusController wsc;
    public GameObject[] clientViews;
    public GameObject pingEventsPanel, statsPanel;
    GameObject popupBoxObj, mapNotificationObj;
    public Button icfWithdrawBtn;

    GameObject[] natTabs = new GameObject[3];

    bool statsOpen = false;

    private void Start()
    {
        budgetAvailTxt = GameObject.Find("Budget_Txt").GetComponent<Text>();
        statsPanel = GameObject.Find("CountryStats_Panel");
        thisPlayerScript = GetComponent<ThisPlayerScript>();
        wsc = GetComponent<WorldStatusController>();
        pingEventsPanel.transform.localScale = new Vector3(0, 0, 0);
        //Get the popup box for future use
        popupBoxObj = GameObject.Find("Popup_Parent");
        //Get the Notification Obj
        mapNotificationObj = GameObject.Find("WorldMapNotification_Anim");
        nonPlayingOverlay = GameObject.Find("NonPlayingOverlay_Img").GetComponent<Image>();
        //Disable the original box
        popupBoxObj.SetActive(false);
        mapNotificationObj.SetActive(false);
        statsPanel.SetActive(false);
        GoToCountryView();
        GetNationTabs();

        GameObject.Find("SummaryText_Txt").GetComponent<Text>().text = Resources.Load<TextAsset>("Text Info Files/Intro Text/IntroScreen").text;


    }

    private void GetNationTabs()
    {
        natTabs[0] = GameObject.Find("NationOneTab_Img");
        natTabs[1] = GameObject.Find("NationTwoTab_Img");
        natTabs[2] = GameObject.Find("NationThreeTab_Img");

        natTabs[1].SetActive(false);
        natTabs[2].SetActive(false);
    }

    private void Update()
    {
        budgetAvailTxt.text = "$" + thisPlayerScript.currentSelectedNation.budgetMax.ToString("0.00") + "m";

        if(wsc.state != WorldStatusController.STATE.PLAYING)
            nonPlayingOverlay.gameObject.SetActive(true);
        else
            nonPlayingOverlay.gameObject.SetActive(false);

    }

    public void     SetNationNameText(List<Nation> nationsInAlliance)
    {
        for(int i = 0; i < nationsInAlliance.Count; i++)
        {
            nationNames[i].text = nationsInAlliance[i].name;
            
        }
    }

    private void GoToCountryView()
    {
        foreach (GameObject g in clientViews)
        {
            g.transform.localScale = new Vector3(0, 0, 0);
        }

        clientViews[1].transform.localScale = new Vector3(1, 1, 1);
    }


    Color32 clicked_Color = new Color32(255, 255, 255, 255);
    Color32 notSelected_Color = new Color32(200, 200, 200, 230);
    public void Click_SelectNation()
    {
        GameObject clickedBtn = EventSystem.current.currentSelectedGameObject;
        string newSelectedNation = clickedBtn.transform.GetChild(0).GetComponent<Text>().text;

        //Currently displayed in nationnames[0]
        Nation selectedNation = new Nation();

        //Find out what nation was clicked
        foreach(Nation n in thisPlayerScript.nationsInThisAlliance)
            if(n.name == newSelectedNation)
            {
                selectedNation = n;
                break;
            }

        SetNationButtonColors(clickedBtn);

        //Set the current selected nation
        thisPlayerScript.currentSelectedNation = selectedNation;
        //Update to show current spending
        thisPlayerScript.ChangedNationAdjustUI();
        //Set nation ID
        thisPlayerScript.selectedNationIndex = selectedNation.nationID;

        //Changing flag
        Sprite newPin = Resources.Load<Sprite>(selectedNation.name);
        selectedNationPin.sprite = newPin;

        //If stats are open 
        if (statsOpen)
            SetUpStatsPanel();
    }

    void SetNationButtonColors(GameObject clickedButton)
    {
        foreach(GameObject g in natTabs)
        {
            g.SetActive(false);
        }

        switch (clickedButton.name)
        {
            case "NationOne_Btn":
                natTabs[0].SetActive(true);
                break;
            case "NationTwo_Btn":
                natTabs[1].SetActive(true);
                break;
            case "NationThree_Btn":
                natTabs[2].SetActive(true);
                break;
        }
    }

    public void Click_ICFView()
    {
        foreach(GameObject g in clientViews)
        {
            g.transform.localScale = new Vector3(0, 0, 0);
        }

        clientViews[2].transform.localScale = new Vector3(1, 1, 1);
        GetComponent<ClimateFundScript>().SetUpICFView();

        if (thisPlayerScript.currentSelectedNation.canWithdraw)
        {
            icfWithdrawBtn.interactable = true;
            icfWithdrawBtn.transform.GetChild(0).GetComponent<Text>().text = "Withdraw";
        }
        else
        {
            icfWithdrawBtn.interactable = false;
            icfWithdrawBtn.transform.GetChild(0).GetComponent<Text>().text = "ineligible nation";
        }
    }

    public void Click_ReturnToNationView()
    {
        GoToCountryView();
        GameObject.Find("Background_Img").transform.localScale = new Vector3(1, 1, 1);
    }

    public void Click_ICFWithdraw()
    {
        GetComponent<ClimateFundScript>().WithDrawICF();
    }

    public void Click_WorldMapView()
    {

        foreach (GameObject g in clientViews)
        {
            g.transform.localScale = new Vector3(0, 0, 0);
        }
        clientViews[0].transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("Background_Img").transform.localScale = new Vector3(0, 0, 0);
        WorldMapUISetup();
        mapNotificationObj.SetActive(false);
    }

    public void Click_DisplayStats()
    {
        statsPanel.SetActive(true);
        statsOpen = true;
        SetUpStatsPanel();
    }

    void SetUpStatsPanel()
    {
        PieChartTest pie = GameObject.Find("Emissions_PieChart").GetComponent<PieChartTest>();
        Temperature tempScript = GetComponent<Temperature>();

        pie.list[0] = thisPlayerScript.currentSelectedNation.nationalSpending;
        pie.list[1] = thisPlayerScript.currentSelectedNation.icfSpent;
        pie.list[2] = thisPlayerScript.currentSelectedNation.climateAdaptationSpent;
        pie.list[3] = thisPlayerScript.currentSelectedNation.climateMitigationSpent;
        pie.list[4] = thisPlayerScript.currentSelectedNation.climateTechSpent;

        pie.MakeGraph();
    }

    public void Click_StartBtn()
    {
        if(!PhotonNetwork.isMasterClient)
            ChangeLayout.ChangeCurrentLayout("AllianceSelect_Parent");
    }

    public void Click_MapPing(string country)
    {
        print("CLicked map ping");
        EventsScript eventsScript = GetComponent<EventsScript>();
        pingEventsPanel.transform.localScale = new Vector3(1,1,1);
        pingEventsPanel.transform.GetChild(0).GetComponent<Text>().text = "Current events in " + country;
        eventsScript.ShowEventsInThisCountry(country);
    }

    public void Click_ClosePingEvents()
    {
        pingEventsPanel.transform.localScale = new Vector3(0, 0, 0);
    }

    public void  Click_ScoresView()
    {
        foreach(GameObject g in clientViews)
        {
            g.transform.localScale = new Vector3(0, 0, 0);
        }

        clientViews[3].transform.localScale = new Vector3(1, 1, 1);

        //Set up text 
        clientViews[3].transform.GetChild(0).GetComponent<Text>().text = thisPlayerScript.currentSelectedNation.name;

    }

    public void Click_CloseParent()
    {
        if (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name == "CountryStats_Panel")
            statsOpen = false;

        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }
 

    //Displaying a popup message box
    public void ShowPopupMessage(string title, string txt)
    {
        GameObject newPopup = GameObject.Instantiate(popupBoxObj, popupBoxObj.transform.parent);
        newPopup.name = "Popup_Parent";

        newPopup.SetActive(true);
        newPopup.transform.localScale = new Vector3(1, 1, 1);

        newPopup.transform.GetChild(2).GetComponent<Text>().text = title;
        newPopup.transform.GetChild(3).GetComponent<Text>().text = txt;
    }


    Color32 nonClickedBtn_Color = new Color32(255, 255, 255, 50);
    Color32 clickedBtn_Color = new Color32(255, 255, 255, 255);
    public void AllianceSelection_Click()
    {
        string allianceName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;

        foreach(Transform t in clickedObj.transform.parent)
        {
            t.GetComponent<Image>().color = nonClickedBtn_Color;
        }
        clickedObj.GetComponent<Image>().color = clickedBtn_Color;

        thisPlayerScript.selectedAlliance = allianceName;


        //Diplay text
        string text = Resources.Load<TextAsset>("Text Info Files/Alliance info/" + allianceName).text;
        GameObject.Find("AllianceInfo_Txt").GetComponent<Text>().text = text;
    }

    public void ConfirmAlliance_Click()
    {
        if (thisPlayerScript.selectedAlliance != null)
        {
            thisPlayerScript.SetTheReturnedAlliance(thisPlayerScript.selectedAlliance);

            ChangeLayout.ChangeCurrentLayout("ClientView_Parent");
        }

        thisPlayerScript.currentSelectedNation = thisPlayerScript.nationsInThisAlliance[0];
        thisPlayerScript.ChangedNationAdjustUI();

    }

    public void ShowNewEventNotification()
    {
        mapNotificationObj.SetActive(true);
    }

    //Aliance selection scrolling control arrow
    public void AllianceSelectArrow_Click()
    {
        float amountToSkip = GameObject.Find("AllianceList_Container").GetComponent<GridLayoutGroup>().cellSize.x + GameObject.Find("AllianceList_Container").GetComponent<GridLayoutGroup>().spacing.x;
        GameObject container = GameObject.Find("AllianceList_Container");
        GameObject clickedBtn = EventSystem.current.currentSelectedGameObject;


        if (clickedBtn.name == "ArrowRight_Btn")
            container.transform.position = new Vector3(container.transform.position.x - amountToSkip, container.transform.position.y, container.transform.position.z);
        else
            container.transform.position = new Vector3(container.transform.position.x + amountToSkip, container.transform.position.y, container.transform.position.z);
    }

    //calculations and display for the non-interactable slider 
    public void SetNonClimateSlider()
    {
        float amount = thisPlayerScript.currentSelectedNation.budgetAvailable;
        thisPlayerScript.currentSelectedNation.nationalSpending = amount;
        float sliderValue = (amount / thisPlayerScript.currentSelectedNation.budgetMax) * 100;

        print(amount + ": Slider Amount - avail budget");
        print(sliderValue + " : Slider value");

        GameObject.Find("NonClimate_Slider").GetComponent<Slider>().value = sliderValue;
        GameObject.Find("NonClimate_Slider").transform.parent.GetChild(4).GetComponent<InputField>().text = amount.ToString();
    }

    public void WorldMapUISetup()
    {
        //GameObject.Find("News_Panel").transform.GetChild(0).GetChild(0).
        GameObject.Find("WorldMapTemp_Slider").GetComponent<Slider>().value = wsc.worldTemp;
        GameObject.Find("WorldMapTemp_Txt").GetComponent<Text>().text = "Global Temp:\n+" + wsc.worldTemp.ToString("0.00") + "C";
    }

    public void SpendingInfoClick()
    {
        GameObject clicked = EventSystem.current.currentSelectedGameObject;

        string type = clicked.transform.parent.tag;
        string displayTxt = Resources.Load<TextAsset>("Text Info Files/SliderInfo/" + type).text;


        ShowPopupMessage(type + " Slider", displayTxt);
    }


}