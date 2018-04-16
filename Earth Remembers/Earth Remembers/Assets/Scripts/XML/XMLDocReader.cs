using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class XMLDocReader : MonoBehaviour {
    //Xml Doc dragged in inspector
    public TextAsset nationsXMLDoc, tippingPointsXMLDoc, exportsXMLDoc;
    //For the reader
    public string nationsData, tPData, exportsData;

    // Use this for initialization
    void Awake () {
        exportsData = exportsXMLDoc.text;
        nationsData = nationsXMLDoc.text;
        tPData = tippingPointsXMLDoc.text;
    }

    public List<Nation> ParseNationsXML(string xmlData)
    {
        //New nations list to store
        List<Nation> nationList = new List<Nation>();
        //New xml document declaration
        XmlDocument xmlDoc = new XmlDocument();
        //Load with new string reader
        xmlDoc.Load(new StringReader(xmlData));

        //XML pattern to load nodes
        string xmlDocPattern = "//nations/nation";
        //Create list of nodes to parse
        XmlNodeList newNodeList = xmlDoc.SelectNodes(xmlDocPattern);
        int i = 0;
        //For each node in the node list stored above
        foreach(XmlNode node in newNodeList)
        {
            print("Test  = " + i);
            i++;
            //Get each individual node for xml text
            XmlNode name = node.FirstChild;
            XmlNode alliance = name.NextSibling;
            XmlNode gdp = alliance.NextSibling;
            XmlNode Pop1 = gdp.NextSibling;
            XmlNode Pop2 = Pop1.NextSibling;
            XmlNode Pop3 = Pop2.NextSibling;
            XmlNode Pop4 = Pop3.NextSibling;
            XmlNode Pop5 = Pop4.NextSibling;
            XmlNode Pop6 = Pop5.NextSibling;
            XmlNode Pop7 = Pop6.NextSibling;
            XmlNode Pop8 = Pop7.NextSibling;
            XmlNode Pop9 = Pop8.NextSibling;
            XmlNode Pop10 = Pop9.NextSibling;
            XmlNode tpf1 = Pop10.NextSibling;
            XmlNode tpf2 = tpf1.NextSibling;
            XmlNode tpf3 = tpf2.NextSibling;
            XmlNode tpf4 = tpf3.NextSibling;
            XmlNode tpf5 = tpf4.NextSibling;
            XmlNode tpf6 = tpf5.NextSibling;
            XmlNode tpf7 = tpf6.NextSibling;
            XmlNode tpf8 = tpf7.NextSibling;
            XmlNode tpf9 = tpf8.NextSibling;
            XmlNode tpf10 = tpf9.NextSibling;
            XmlNode sr1 = tpf10.NextSibling;
            XmlNode sr2 = sr1.NextSibling;
            XmlNode sr3 = sr2.NextSibling;
            XmlNode sr4 = sr3.NextSibling;
            XmlNode sr5 = sr4.NextSibling;
            XmlNode sr6 = sr5.NextSibling;
            XmlNode sr7 = sr6.NextSibling;
            XmlNode sr8 = sr7.NextSibling;
            XmlNode sr9 = sr8.NextSibling;
            XmlNode sr10 = sr9.NextSibling;
            XmlNode alphaK = sr10.NextSibling;
            XmlNode capital = alphaK.NextSibling;
            XmlNode ICFWithdraw = capital.NextSibling;


            //New nation
            float[] population = new float[] { float.Parse(Pop1.InnerText), float.Parse(Pop2.InnerText), float.Parse(Pop3.InnerText),
            float.Parse(Pop4.InnerText),float.Parse(Pop5.InnerText),float.Parse(Pop6.InnerText),float.Parse(Pop7.InnerText),
            float.Parse(Pop8.InnerText),float.Parse(Pop9.InnerText),float.Parse(Pop10.InnerText)};

            float[] tfp = new float[] {float.Parse(tpf1.InnerText), float.Parse(tpf2.InnerText), float.Parse(tpf3.InnerText)
            , float.Parse(tpf4.InnerText) , float.Parse(tpf5.InnerText) , float.Parse(tpf6.InnerText) , float.Parse(tpf7.InnerText)
            , float.Parse(tpf8.InnerText) , float.Parse(tpf9.InnerText) , float.Parse(tpf10.InnerText) };

            float[] sr = new float[] { float.Parse(sr1.InnerText), float.Parse(sr2.InnerText), float.Parse(sr3.InnerText)
            , float.Parse(sr4.InnerText), float.Parse(sr5.InnerText), float.Parse(sr6.InnerText), float.Parse(sr7.InnerText)
            , float.Parse(sr8.InnerText), float.Parse(sr9.InnerText), float.Parse(sr10.InnerText)};
            
            Nation newNation = new Nation((Nation.Alliances)System.Enum.Parse(typeof(Nation.Alliances), alliance.InnerText), name.InnerText,
                float.Parse(gdp.InnerText), population, tfp, sr, float.Parse(alphaK.InnerText), float.Parse(capital.InnerText));
            
            //Set the new instance's statistics and data
            if (ICFWithdraw.InnerText == "yes")
            {
                newNation.canWithdraw = true;
            }
            else
            {
                newNation.canWithdraw = false;
            }

            //Add to the nation list
            nationList.Add(newNation);
        }
        //Return complete list of nations
        return nationList;
    }

    public List<TippingPoint> ParseTPXML(string xmlData)
    {
        //New nations list to store
        List<TippingPoint> tPList = new List<TippingPoint>();
        //New xml document declaration
        XmlDocument xmlDoc = new XmlDocument();
        //Load with new string reader
        xmlDoc.Load(new StringReader(xmlData));

        //XML pattern to load nodes
        string xmlDocPattern = "//tippingpoints/tp";
        //Create list of nodes to parse
        XmlNodeList newNodeList = xmlDoc.SelectNodes(xmlDocPattern);

        //For each node in the node list stored above
        foreach (XmlNode node in newNodeList)
        {
            //Get each individual node for xml text
            XmlNode name = node.FirstChild;
            XmlNode description = name.NextSibling;

            //New nation
            TippingPoint newTP = new TippingPoint();
            //Set the new instance's statistics and data
            newTP.name = name.InnerText;
            newTP.description = description.InnerText;
            //Add to the nation list
            tPList.Add(newTP);
        }
        //Return complete list of nations
        return tPList;
    }
}
