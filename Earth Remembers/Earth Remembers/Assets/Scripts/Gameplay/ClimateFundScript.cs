using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClimateFundScript : MonoBehaviour {
    public Slider ICFSlider;
    public Text fundRemainingText, withdrawText, currentNationText;

    WorldStatusController wsc;
    ThisPlayerScript tps;

    private void Start()
    {
        wsc = GetComponent<WorldStatusController>();
        tps = GetComponent<ThisPlayerScript>();
    }

    public void SetUpICFView()
    {
        currentNationText.text = tps.currentSelectedNation.name;
        fundRemainingText.text = "$" + wsc.ICF + "m remaining";
        withdrawText.text = "$" + ICFSlider.value + "m";
        ICFSlider.maxValue = wsc.ICF;
    }
	
    public void ICFSliderChanged()
    {
        withdrawText.text = "$" + ICFSlider.value + "m";
    }

    public void WithDrawICF()
    {
        float sliderValue = ICFSlider.value;


        tps.currentSelectedNation.budgetMax += (int)sliderValue;
        tps.currentSelectedNation.budgetAvailable += (int)sliderValue;
        wsc.UpdateICF(sliderValue);
        

        ICFSlider.value = 0;
        GetComponent<ClientUIController>().Click_ReturnToNationView();
    }
}
