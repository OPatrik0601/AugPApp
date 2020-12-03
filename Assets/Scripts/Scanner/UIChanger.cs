using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChanger : MonoBehaviour
{
    private static UIChanger _instance = null;
    public static UIChanger Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<UIChanger>();

            return _instance;
        }
    }


    [SerializeField] private GameObject beforeLoadUI = null;
    [SerializeField] private GameObject scannerUI = null;
    [SerializeField] private GameObject verifyUI = null;

    [SerializeField] private Text articleFound = null;
    [SerializeField] private Text documentInfo = null;
    [SerializeField] private GameObject loadButton = null;

    void Start()
    {
        ShowUI(UIObjects.BeforeLoadUI);
    }

    /// <summary>
    /// Show ONLY the requested UI
    /// </summary>
    public void ShowUI(UIObjects uiObject)
    {
        beforeLoadUI.SetActive(false);
        scannerUI.SetActive(false);
        verifyUI.SetActive(false);
        switch (uiObject)
        {
            case UIObjects.BeforeLoadUI:
                beforeLoadUI.SetActive(true);
                break;
            case UIObjects.ScannerUI:
                scannerUI.SetActive(true);
                break;
            case UIObjects.VerifyUI:
                verifyUI.SetActive(true);
                if(LoadedDocument.Instance.ReadyToLoad)
                {
                    articleFound.text = "An article has been found!";
                    string authors = "";
                    LoadedDocument.Instance.Authors.ForEach(author => authors += $"{author.AuthorName}\n");
                    documentInfo.text = $"Title:\n{LoadedDocument.Instance.ArticleName}\n\nAuthor(s):\n{authors}";
                    loadButton.SetActive(true);
                } else
                {
                    articleFound.text = "No figure has been found with that QR-code.";
                    documentInfo.text = $"";
                    loadButton.SetActive(false);
                }
                break;
        }
    }

    public void BackToTheScanner() //this is attached in the editor to the button
    {
        ShowUI(UIObjects.ScannerUI);
    }

    public enum UIObjects
    {
        BeforeLoadUI,
        ScannerUI,
        VerifyUI
    }
}
