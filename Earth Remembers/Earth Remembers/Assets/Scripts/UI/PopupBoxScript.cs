using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupBoxScript : MonoBehaviour, IDragHandler {


    private void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
    //Closing teh box
    public void CloseBox()
    {
        Destroy(transform.gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Input.mousePosition.x,(Input.mousePosition.y - transform.localScale.y/2), Input.mousePosition.z);
    }
}
