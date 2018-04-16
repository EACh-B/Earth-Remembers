using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsScript : MonoBehaviour {
    List<string> eventsGeneral, eventsEN, eventsWAIS, eventsCR, eventsRF;
    public List<string> eventPool;
    public List<WorldEvent> currentWorldEvents;
    public List<WorldEvent> alreadyUsedEvents;
    public List<WorldEvent> WorldEventsList;
    Nation effectedNation;
    WorldEvent currentWE;

    public WorldStatusController wsc;
    Initializer initializer;
    ThisPlayerScript thisPlayerScript;


    public GameObject newEventObj, currentEventIcon, eventPopupPanel, eventInfoPanel, pingEventsContainer;

    [System.Serializable]
    public class WorldEvent
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string[] possibleCountries;
        [SerializeField]
        public string tp;
        [SerializeField]
        public string info;

        public string country;

    }

    private void Start()
    {
        wsc = GetComponent<WorldStatusController>();
        initializer = GetComponent<Initializer>();
        thisPlayerScript = GetComponent<ThisPlayerScript>();

        alreadyUsedEvents = new List<WorldEvent>();
        currentWorldEvents = new List<WorldEvent>();

        currentWE = new WorldEvent();

        eventsGeneral = new List<string>();
        eventsEN = new List<string>();
        eventsWAIS = new List<string>();
        eventsCR = new List<string>();
        eventsRF = new List<string>();

        //Populate lists for each tipping point
        foreach (WorldEvent we in WorldEventsList)
        {
            switch (we.tp)
            {
                case "EN":
                    eventsEN.Add(we.name);
                    break;
                case "WAIS":
                    eventsWAIS.Add(we.name);
                    break;
                case "CR":
                    eventsCR.Add(we.name);
                    break;
                case "RF":
                    eventsRF.Add(we.name);
                    break;
                case "GENERAL":
                    eventsGeneral.Add(we.name);
                    break;
                default:
                    Debug.LogError("No tipping point found for this event '" + we.name + "'");
                    break;
            }
        }
    }

    private void Update()
    {
        if (eventPopupPanel.activeInHierarchy)
        {
            if (eventPopupPanel.transform.GetChild(0).transform.childCount <= 2)
            {
                //eventPopupPanel.SetActive(false);
                //Next turn button can now be pressed
               // wsc.SetupToPlaying();
            }
        }
    }

    public void GetNewEventsProcess()
    {
        //Find which tipping points reached
        bool[] tippingPointBools = new bool[4];
        tippingPointBools[0] = wsc.ElNinoTipped;
        tippingPointBools[1] = wsc.WAISTipped;
        tippingPointBools[2] = wsc.CoralReefTipped;
        tippingPointBools[3] = wsc.RainForestTipped;

        //Get pool of available events
        GetPool(tippingPointBools);

        //Get amount of events to activate
        int eventsToGet = 1;
        foreach (bool b in tippingPointBools)
        {
            if (b == true)
                eventsToGet++;
        }

        List<WorldEvent> newActiveEvents = GetNewActiveEvents(eventsToGet);

        if(newActiveEvents.Count < 1)
            wsc.SetupToPlaying();
        else
            //Show visual effect
            StartCoroutine(ShowNewEventVisual(newActiveEvents));
    }

    public void GetPool(bool[] tippingPointBools)
    {
        //Reset list
        eventPool = new List<string>();

        //Find out which tipping points are true and add the corresponding 
        //events to the event pool
        if (tippingPointBools[0])
            foreach (string s in eventsEN)
                eventPool.Add(s);
        if (tippingPointBools[1])
            foreach (string s in eventsWAIS)
                eventPool.Add(s);
        if (tippingPointBools[2])
            foreach (string s in eventsCR)
                eventPool.Add(s);
        if (tippingPointBools[3])
            foreach (string s in eventsRF)
                eventPool.Add(s);

        //Add all the general events
        foreach (string s in eventsGeneral)
            eventPool.Add(s);

        foreach(WorldEvent e in alreadyUsedEvents)
        {
            eventPool.Remove(e.name);
        }
    }

    List<WorldEvent> GetNewActiveEvents(int eventsToGet)
    {
        //List to return
        List<WorldEvent> eventsToReturn = new List<WorldEvent>();
        //Get each event
        for (int i = 0; i < eventsToGet; i++)
        {
            if (eventPool.Count > 0)
            {
                //Randomization
                int random = Random.Range(0, eventPool.Count);
                //Pick event out by its name
                string thisEventName = eventPool[random];
                //get event with this name
                WorldEvent thisEvent = FindEventWithName(thisEventName);
                //Affected country
                thisEvent.country = thisEvent.possibleCountries[Random.Range(0, thisEvent.possibleCountries.Length - 1)];

                eventsToReturn.Add(thisEvent);
                eventPool.Remove(thisEvent.name);
                alreadyUsedEvents.Add(thisEvent);
            }

        }

        //Return the list
        return eventsToReturn;
    }


    public void ApplyEvent(EventsScript.WorldEvent appliedEvent, GameObject eventObj)
    {
        GameObject newWorldEventIcon = GameObject.Instantiate(currentEventIcon, currentEventIcon.transform.parent);
        newWorldEventIcon.SetActive(true);

        newWorldEventIcon.GetComponent<ThisEvent>().eventsScript = GetComponent<EventsScript>();

        WorldEvent newWorldEvent = new WorldEvent();

        newWorldEvent.name = appliedEvent.name;
        newWorldEvent.country = appliedEvent.country;
        newWorldEvent.tp = appliedEvent.tp;
        newWorldEvent.info = appliedEvent.info;

        newWorldEventIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Host Event Icons/General Icon PNG");


        newWorldEventIcon.GetComponent<ThisEvent>().thisWorldEvent = newWorldEvent;

        currentWorldEvents.Add(newWorldEvent);

        GameObject.Destroy(eventObj);

        if (eventPopupPanel.transform.GetChild(0).transform.childCount <= 2)
        {
            //Hide the panel as no events remain to be applied
            eventPopupPanel.SetActive(false);
            //Next turn button can now be pressed
            wsc.SetupToPlaying();
        }

        GetComponent<PhotonView>().RPC("SyncEventsRPC", PhotonTargets.AllBuffered, newWorldEvent.name, newWorldEvent.country);

    }

    //Called when clicked on ping
    public void ShowEventsInThisCountry(string country)
    {
        //Clear first
        foreach (Transform t in pingEventsContainer.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        foreach (WorldEvent e in currentWorldEvents)
        {
            if (e.country == country)
            {
                GameObject newEventCard = GameObject.Instantiate(newEventObj, pingEventsContainer.transform);
                newEventCard.SetActive(true);
                newEventCard.transform.GetChild(0).GetComponent<Text>().text = e.name;
                newEventCard.transform.GetChild(2).GetComponent<Text>().text = e.country;
                newEventCard.transform.GetChild(3).GetComponent<Text>().text = e.info;
                GameObject.Destroy(newEventCard.transform.GetChild(1).gameObject);
            }
        }
    }

    public void ShowEventInfo(WorldEvent worldEvent)
    {
        eventInfoPanel.SetActive(true);
        eventInfoPanel.transform.GetChild(0).GetComponent<Text>().text = worldEvent.name;
        eventInfoPanel.transform.GetChild(2).GetComponent<Text>().text = worldEvent.country;
        eventInfoPanel.transform.GetChild(3).GetComponent<Text>().text = worldEvent.info;
    }



    [PunRPC]
    public void GetEffectedNation(string country)
    {
        effectedNation = new Nation();
        foreach (Nation n in initializer.nationsList)
        {
            if (n.name == country)
                effectedNation = n;
        }
    }

    IEnumerator ShowNewEventVisual(List<WorldEvent> newEvents)
    {
        //Unhide the panel
        eventPopupPanel.SetActive(true);
        print("Set true");
        //Clear the panel of any children
        int clearCounter = 0;
        foreach(Transform t in newEventObj.transform.parent)
        {
            if (clearCounter > 0)
                GameObject.Destroy(t.gameObject);
            clearCounter++;
        }

        Button[] buttonsToActivate = new Button[newEvents.Count];
        int buttonArrayIndex = 0; 
        //For every new event
        foreach(WorldEvent we in newEvents)
        {
            //wait
            yield return new WaitForSeconds(1f);
            //New object for the card image on ui panel
            GameObject newObj = GameObject.Instantiate(newEventObj, newEventObj.transform.parent);
            newObj.SetActive(true);
            //Set this script to new obj
            newObj.GetComponent<ThisEvent>().eventsScript = GetComponent<EventsScript>();
            newObj.GetComponent<ThisEvent>().thisWorldEvent = we;
            //Set the text for the new obj
            newObj.transform.GetChild(0).GetComponent<Text>().text = we.name;
            newObj.transform.GetChild(1).GetComponent<Button>().interactable = false;
            newObj.transform.GetChild(3).GetComponent<Text>().text = we.country;
            newObj.transform.GetChild(4).GetComponent<Text>().text = we.info;


            buttonsToActivate[buttonArrayIndex] = newObj.transform.GetChild(1).GetComponent<Button>();

            buttonArrayIndex++;
        }

        foreach (Button btn in buttonsToActivate)
        {
            btn.interactable = true;
            print("Activet");
        }

        if (eventPopupPanel.transform.GetChild(0).transform.childCount < 2)
        {
            //Hide the panel as no events remain to be applied
            eventPopupPanel.SetActive(false);
            //Next turn button can now be pressed
            
        }
    }

    WorldEvent FindEventWithName(string eventName)
    {
        //New event
        WorldEvent newEvent = new WorldEvent();
        //Check each of the events that are in the game
        foreach(WorldEvent we in WorldEventsList)
        {
            //If got the name, return it
            if (we.name == eventName)
                return we;
        }

        Debug.LogError("Event with name '" + eventName + "' not found..");
        return newEvent;
    }

    [PunRPC]
    public void SyncEventsRPC(string eventName, string eventCountry)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            WorldEvent newWorldEvent = new WorldEvent();

            foreach (WorldEvent e in WorldEventsList)
            {
                if (eventName == e.name)
                {
                    newWorldEvent.name = e.name;
                    newWorldEvent.country = eventCountry;
                    newWorldEvent.tp = e.tp;
                }
            }

            GetComponent<ClientUIController>().ShowNewEventNotification();
            currentWorldEvents.Add(newWorldEvent);
        }

        
    }

    [PunRPC]
    public void NextTurnSyncEventsRPC()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            List<WorldEvent> eventsToRemove = new List<WorldEvent>();
            eventsToRemove.Clear();

            int index = currentWorldEvents.Count;

            foreach(WorldEvent e in eventsToRemove)
            {
                currentWorldEvents.Remove(e);
            }
        }
    }

 
}
