using Dummiesman;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZXing; //QR lib
using Unity.Jobs;
using ZXing.Common;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

public class QRScanner : MonoBehaviour
{
    private WebCamTexture backCam;
    private bool camAvailable = false;
    private IBarcodeReader barcodeReader;

    //Attach in editor
    [SerializeField] private RawImage background = null;
    [SerializeField] private AspectRatioFitter fit = null;
    [SerializeField] private Text errorText = null;

    //Variables to validate
    private bool reachableInternet;
    private bool cameraEnabled;

    private IEnumerator Start()
    {
        SceneHandler.LoadLoadingScene();
        //Check Internet connection
        if (!reachableInternet)
            errorText.text = "AugmentedPaper requires Internet connection!";
        yield return new WaitUntil(() => reachableInternet);

        //Check cam
        if (!cameraEnabled)
            errorText.text = "AugmentedPaper requires permission for the camera!";
        yield return new WaitUntil(() => cameraEnabled);

        setUpCamera();
    }

    /*
    private void Start()
    {
        ModelStringHandler.LoadDocument(@"http://users.atw.hu/count/Models/file_2.xml;4");
        Debug.Log(LoadedDocument.Instance.ArticleName);
        Debug.Log(LoadedDocument.Instance.DOI);
        foreach(var item in LoadedDocument.Instance.Authors)
        {
            Debug.Log($"{item.AuthorName} : {item.Affiliation}");
        }
        Debug.Log($"{LoadedDocument.Instance.LoadedFigure.MtlFilePath} : {LoadedDocument.Instance.LoadedFigure.ObjFilePath}");
    }
    */

    private WebCamDevice[] AllCameras
    {
        get => WebCamTexture.devices;
    }

    private void setUpCamera()
    {

        //search for the back camera
        foreach (WebCamDevice device in AllCameras)
        {
            if (!device.isFrontFacing)
            {
                backCam = new WebCamTexture(device.name, Screen.width, Screen.height);
            }
        }

        if (!backCam)
        {
            errorText.text = "There is no back camera on this device.";
            return;
        }

        backCam.Play();
        background.texture = backCam; //camera image to UI

        camAvailable = true; //everything is ok
        barcodeReader = new BarcodeReader();
        UIChanger.Instance.ShowUI(UIChanger.UIObjects.ScannerUI);
    }

    private void Update()
    {
        reachableInternet = !(Application.internetReachability == NetworkReachability.NotReachable);
        cameraEnabled = (AllCameras.Length > 0);

        if (!camAvailable)
            return;

        //aspect ratio
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        //mirroring camera
        float scaleY = backCam.videoVerticallyMirrored ? -1 : 1;
        background.rectTransform.localScale = new Vector3(1, scaleY, 1);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    /// <summary>
    /// Looking for a barcode on the current camera image
    /// </summary>
    public void CheckScreenForBarcode()
    {
        try
        {
            Result result = barcodeReader.Decode(backCam.GetPixels32(), backCam.width, backCam.height);
            if (result != null)
            {
                ModelStringHandler.LoadDocument(result.Text); //load the .augp document
                UIChanger.Instance.ShowUI(UIChanger.UIObjects.VerifyUI); //load the verification UI
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    /// <summary>
    /// Load the object from the previously scanned document. 
    /// </summary>
    public void LoadScannedDocumentObject() //this is attached to the button in the editor
    {
        if (LoadedDocument.Instance.ReadyToLoad)
        {
            GameObject loadedModel = ModelStringHandler.LoadDocumentObject();
            if (loadedModel)
            {
                SceneHandler.ShowLoadingScreen(true);
                SceneHandler.StartLoadARScene(loadedModel); //ha sikerült a betöltés, akkor töltsük be az AR scenet
            }
            else
            {
                SceneHandler.ShowLoadingScreen(false);
            }
        }
    }
}
