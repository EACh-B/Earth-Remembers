using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SliderSnapScript : MonoBehaviour, IPointerUpHandler {
    Slider thisSlider, mitSlider;
    ThisPlayerScript thisPlayerScript;
    WorldStatusController wsc;
    public Text textObject;
    int budget;
    public int oldValue, sliderID, selectedNationId;
    float originalValue;
    float currentValueForEnterComparison, currentValue;
    Dictionary<int, float> oldSliderValues = new Dictionary<int, float>();

	// Use this for initialization
	void Start () {
        currentValue = 0;
        thisSlider = GetComponent<Slider>();
        originalValue = thisSlider.value;
        thisPlayerScript = GameObject.Find("Controller").GetComponent<ThisPlayerScript>();
        wsc = GameObject.Find("Controller").GetComponent<WorldStatusController>();

        GetThisSliderType();

        for(int i = 0; i < 3; i++)
        {
            oldSliderValues.Add(i, 0);
        }

        mitSlider = GameObject.Find("Mitigation_Slider").GetComponent<Slider>();
	}

    private void Update()
    {
        selectedNationId = thisPlayerScript.selectedNationIndex;

        if (wsc.state == WorldStatusController.STATE.PLAYING && sliderID != 1)
            thisSlider.interactable = true;
        else
            thisSlider.interactable = false;


    }

    void GetThisSliderType()
    {
        switch (transform.gameObject.name)
        {
            case "NonClimate_Slider":
                GetComponent<SliderSnapScript>().enabled = false;
                thisSlider.interactable = false;
                textObject = transform.parent.GetChild(3).gameObject.GetComponent<Text>();
                break;
            case "CCA_Slider":
                sliderID = 2;
                textObject = transform.parent.GetChild(3).gameObject.GetComponent<Text>();
                break;
            case "Mitigation_Slider":
                textObject = transform.parent.GetChild(3).gameObject.GetComponent<Text>();
                sliderID = 3;
                break;
            case "ICF_Slider":
                sliderID = 4;
                textObject = transform.parent.GetChild(3).gameObject.GetComponent<Text>();
                break;
            case "GreenTech_Slider":
                sliderID = 5;
                break;
            default:
                Debug.LogError("Failed to identify slider ID");
                break;
        }
    }

    //Called when value changed
    public void SnapValue()
    {
        //Snap to 0 or 5
        //thisSlider.value = 5 * (Mathf.Floor(Mathf.Abs(thisSlider.value / 5)));
    }

    void GetBudget()
    {
        
    }

    //Called when them mouse button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        //IF value has changed
        if(thisSlider.value != oldSliderValues[selectedNationId])
        {
            //Get difference between new value and old            
            float changeValue = thisSlider.value - oldSliderValues[selectedNationId];
            //Set new value to old
            oldSliderValues[selectedNationId] = (int)thisSlider.value;

            if(sliderID != 5)
                //Update the spending UI
             UpdateValues(changeValue);
            if(sliderID == 5 || sliderID == 4)
            {
                UpdateGreenTechValues();
            }
        }
    }

    private void UpdateValues(float changeValue)
    {
        textObject = transform.parent.GetChild(4).gameObject.GetComponent<Text>();
        
        //Get current value
        float.TryParse(textObject.gameObject.GetComponent<InputField>().text, out currentValue);
        //Update value with the selected percentage of budget
        float test = ((float)thisPlayerScript.currentSelectedNation.budgetMax / 100) * thisSlider.value;
        float inputtedValue = test;
        float budgetEffectValue = inputtedValue - currentValue;

        print("Change value = " + changeValue + "\nInput Value = " + inputtedValue);
        print("Current value = " + currentValue);

        if (thisPlayerScript.currentSelectedNation.budgetAvailable >= budgetEffectValue)
        {
            currentValue = inputtedValue;
            thisPlayerScript.currentSelectedNation.budgetAvailable -= budgetEffectValue;

            print(thisPlayerScript.currentSelectedNation.budgetAvailable + "tab " + budgetEffectValue);
            //Update the text 
            textObject.gameObject.GetComponent<InputField>().text = currentValue.ToString();
            switch (sliderID)
            {
                case 1:
                    thisPlayerScript.currentSelectedNation.nationalSpending = inputtedValue;
                    break;
                case 2:
                    thisPlayerScript.currentSelectedNation.climateAdaptationSpent = inputtedValue;
                    break;
                case 3:
                    thisPlayerScript.currentSelectedNation.climateMitigationSpent = inputtedValue;
                    break;
                case 4:
                    thisPlayerScript.currentSelectedNation.icfSpent = inputtedValue;
                    break;
            }
        }
        else
        {
            if (thisPlayerScript.currentSelectedNation.budgetAvailable > 0)
            {
                print("rejected");
                currentValue = thisPlayerScript.currentSelectedNation.budgetAvailable + (originalValue/100) * thisPlayerScript.currentSelectedNation.budgetMax;
                thisSlider.value = (currentValue / (float)thisPlayerScript.currentSelectedNation.budgetMax) * 100;

                thisPlayerScript.currentSelectedNation.budgetAvailable = 0;
                textObject.gameObject.GetComponent<InputField>().text = currentValue.ToString();
                switch (sliderID)
                {
                    case 1:
                        thisPlayerScript.currentSelectedNation.nationalSpending = currentValue;
                        break;
                    case 2:
                        thisPlayerScript.currentSelectedNation.climateAdaptationSpent = currentValue;
                        break;
                    case 3:
                        thisPlayerScript.currentSelectedNation.climateMitigationSpent = currentValue;
                        break;
                    case 4:
                        thisPlayerScript.currentSelectedNation.icfSpent = currentValue;
                        break;
                }
            }
            else
            {
                print("Adjust me slidy");
                thisSlider.value = (float.Parse(textObject.gameObject.GetComponent<InputField>().text) / (float)thisPlayerScript.currentSelectedNation.budgetMax) * 100;
            }
        }
        //Updat the slider for non climate spending
        GameObject.Find("Controller").GetComponent<ClientUIController>().SetNonClimateSlider();

        originalValue = thisSlider.value;
    }


    public void InputFieldValueEntered()
    {
        print("Current val" + currentValue);
        textObject = transform.parent.GetChild(3).gameObject.GetComponent<Text>();
        float enteredValue = float.Parse(textObject.gameObject.GetComponent<InputField>().text);
        float budgetEffectValue = (int)enteredValue - currentValue;

        print(budgetEffectValue+"$M");
        print("Entereed budget " + enteredValue + "\nParent = " + transform.parent.GetChild(1).gameObject.name);

        if (enteredValue >= 0 && budgetEffectValue <= thisPlayerScript.currentSelectedNation.budgetAvailable)
        {
            currentValue = (int)enteredValue;
            gameObject.GetComponent<Slider>().value = (enteredValue / thisPlayerScript.currentSelectedNation.budgetMax) * 100;
            thisPlayerScript.currentSelectedNation.budgetAvailable -= budgetEffectValue;

            switch (sliderID)
            {
                case 1:
                    thisPlayerScript.currentSelectedNation.nationalSpending = enteredValue;
                    break;
                case 2:
                    thisPlayerScript.currentSelectedNation.climateAdaptationSpent = enteredValue;
                    break;
                case 3:
                    thisPlayerScript.currentSelectedNation.climateMitigationSpent = enteredValue;
                    break;
                case 4:
                    thisPlayerScript.currentSelectedNation.icfSpent = enteredValue;
                    break;
            }
        }

        else
        {
            textObject.gameObject.GetComponent<InputField>().text = currentValue.ToString();
        }
    }

    void UpdateGreenTechValues()
    {

        if (thisSlider.value > 0)
        {
            float spentVal = (thisPlayerScript.currentSelectedNation.climateMitigationSpent * GetComponent<Slider>().value) / 100;
            thisPlayerScript.currentSelectedNation.climateTechSpent = spentVal;
            print("Green Slider val = " + spentVal);
        }
        else
            thisPlayerScript.currentSelectedNation.climateTechSpent = 0;


    }

    public void ResetSlider()
    {
        thisSlider.value = 0;
        if(textObject != null)
            textObject.gameObject.GetComponent<InputField>().text = "0";
    }

    
}
