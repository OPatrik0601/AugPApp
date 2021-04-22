using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackingStatusManager : MonoBehaviour
{
    private static TrackingStatusManager _instance = null;
    public static TrackingStatusManager Instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<TrackingStatusManager>();

            return _instance;
        }
    }

    [SerializeField] private GameObject notTrackedHelp = null;

    private void Awake() {
        notTrackedHelp.SetActive(false);
    }

    public void ChangeStatus(bool tracked) {
        notTrackedHelp.SetActive(!tracked);
    }
}

