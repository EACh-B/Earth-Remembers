using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawScript : MonoBehaviour
{
    public GameObject continentDot;
    public float xMin, xMax, yMin, yMax;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void DrawTheDots()
    {
        for (float i = xMin; i < xMax; i += .5f)
        {
            for (float j = yMin; j < yMax; j += .5f)
            {
                GameObject.Instantiate(continentDot, new Vector2(i, j), Quaternion.identity);
            }
        }
    }
}
