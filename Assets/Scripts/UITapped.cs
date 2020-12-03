using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UITapped : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int eventId = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch(eventId)
        {
            case 1: //Back to the scanner
                SceneHandler.StartLoadScannerScene();
                break;
        }
    }
}
