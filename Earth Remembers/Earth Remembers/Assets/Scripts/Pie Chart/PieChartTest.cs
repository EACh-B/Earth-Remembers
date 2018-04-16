using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartTest : MonoBehaviour {

    public List<float> list = new List<float>();
    public string[] emissionNames;
    public Color[] colours;
    Image pie;
    bool firstTime = true;


    public void MakeGraph()
    {
        if (firstTime)
            SetUpKey();

        float total = 0;
        float zRot = 0;


        foreach (float f in list)
            total += f;

        for(int i = 0; i < list.Count; i++)
        {
            Image newPie = transform.GetChild(i).GetComponent<Image>();
            newPie.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            newPie.color = colours[i];
            newPie.fillAmount = list[i] / total;

            newPie.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, zRot));

            zRot -= newPie.fillAmount * 360;
        }
    }

    void SetUpKey()
    {
        
        firstTime = false;
        int counter = 0;
        foreach(Transform t in GameObject.Find("PieKey_Parent").transform)
        {
            t.GetChild(0).GetComponent<Image>().color = colours[counter];
            t.GetChild(1).GetComponent<Text>().text = emissionNames[counter];
            counter++;
        }
    }
}
