using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageRecognition : MonoBehaviour
{
    private static ImageRecognition _instance = null;
    public static ImageRecognition Instance
    {
        get
        {
            if (_instance == null)
                _instance = Object.FindObjectOfType<ImageRecognition>();

            return _instance;
        }
    }
    public GameObject CreatedGameObject { get; private set; } = null;
    public void SetObject(GameObject loadedObject)
    {
        //the original, tracked GameObject rotates towards the camera by default, so we have to make a copy of it that doesn't rotate that way.
        loadedObject.gameObject.SetActive(false); //disable the original, tracked GO
        loadedObject.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);

        GetComponent<ARTrackedImageManager>().trackedImagePrefab = loadedObject;
        GetComponent<ARTrackedImageManager>().trackedImagesChanged += (arg) =>
        {
            if (!CreatedGameObject)
            {
                if (arg.added.Count > 0)
                {
                    CreatedGameObject = GameObject.Instantiate(loadedObject.gameObject); //make a copy of the original
                    CreatedGameObject.SetActive(true);
                    CreatedGameObject.GetComponent<Renderer>().receiveShadows = false;
                    CreatedGameObject.transform.rotation = loadedObject.transform.rotation;
                    Destroy(CreatedGameObject.GetComponent<ARTrackedImage>()); //don't track the new object
                }
            }

            if (CreatedGameObject)
            {
                foreach (ARTrackedImage tracked in arg.updated)
                {
                    //for some reason Unity keeps attaching the ArTrackedImage to the GameObject, and it's making it rotate in a weird way sometimes
                    if(CreatedGameObject.GetComponent<ARTrackedImage>() != null)
                    {
                        Destroy(CreatedGameObject.GetComponent<ARTrackedImage>());
                    }

                    CreatedGameObject.transform.position = tracked.gameObject.transform.position; //update the position of the new GameObject
                }
            }
        };
    }
}
