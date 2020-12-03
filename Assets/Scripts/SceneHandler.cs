using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SceneHandler
{
    public const string ARSceneName = "AR";
    public const string ScannerSceneName = "Scanner";
    public const string LoadingSceneName = "Loading";

    private static GameObject loadingCanvas = null;

    /// <summary>
    /// Load the AR scene
    /// </summary>
    /// <param name="loadedObject">The loaded GameObject</param>
    public static void StartLoadARScene(GameObject loadedObject)
    {
        //the 'DontDestroyOnLoad' wouldn't work in this scenario
        AsyncOperation async = SceneManager.LoadSceneAsync(ARSceneName, LoadSceneMode.Additive); //load new scene as additive
        async.completed += (obj) =>
        {
            SceneManager.MoveGameObjectToScene(loadedObject, SceneManager.GetSceneByName(ARSceneName)); //loadedObject to the new (AR) scene
            SceneManager.UnloadSceneAsync(ScannerSceneName); //unload the scanner scene
            ShowLoadingScreen(false);

            ImageRecognition.Instance.SetObject(loadedObject);
        };
    }

    /// <summary>
    /// Load the scanner scene
    /// </summary>
    public static void StartLoadScannerScene()
    {
        SceneManager.LoadSceneAsync(ScannerSceneName, LoadSceneMode.Single);
    }

    public static void LoadLoadingScene()
    {
        if (!loadingCanvas)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(LoadingSceneName, LoadSceneMode.Additive);
            async.completed += (obj) =>
            {
                loadingCanvas = GameObject.FindWithTag("CanvasTag");
                ShowLoadingScreen(false);
            };
        }
    }

    public static void ShowLoadingScreen(bool show)
    {
        if(loadingCanvas)
        {
            loadingCanvas.SetActive(show);
        }
    }
}
