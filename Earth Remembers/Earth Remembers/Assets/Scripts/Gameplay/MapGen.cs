using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour {
    public GameObject dotPrefab, worldMask;


    // Use this for initialization
    void Start() {
        MaskScaling();

        GenDots(15, 15, 7);
    }

    //Scale the mask to the resolution
    private void MaskScaling()
    {
        float height = Camera.main.orthographicSize * 1.8f;
        float width = height * Screen.width / Screen.height;

        Sprite s = worldMask.GetComponent<SpriteRenderer>().sprite;

        float unitWidth = s.textureRect.width / s.pixelsPerUnit;
        float unitHeight = s.textureRect.height / s.pixelsPerUnit;

        worldMask.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(width / unitWidth, height / unitHeight);
    }

    void GenDots(int width, int height, int padding)
    {
        for(float x = 0 - padding; x < width - padding; x+=0.2f)
        {
            for(float y = 0 -3; y < height-3; y+=0.2f)
            {
                GameObject.Instantiate(dotPrefab, new Vector2(x, y), Quaternion.identity);
            }
        }
    }
}
