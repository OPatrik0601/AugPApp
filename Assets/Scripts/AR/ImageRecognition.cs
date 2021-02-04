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
        changeScale(loadedObject);
        loadedObject.gameObject.SetActive(false); //disable the original, tracked GO

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
                    if(CreatedGameObject.GetComponent<ARTrackedImage>() != null)
                    {
                        Destroy(CreatedGameObject.GetComponent<ARTrackedImage>());
                    }

                    CreatedGameObject.transform.position = tracked.gameObject.transform.position; //update the position of the new GameObject
                }
            }
        };
    }

    /// <summary>
    /// Resizes the GameObject, so objects with bigger models will be visible as well
    /// </summary>
    /// <param name="obj">GameObject to resize</param>
    private void changeScale(GameObject obj)
    {
        const float maxSize = 3.0f; //maximum size that a dimension can reach
        const float baseScale = 0.025f; //the default scale that a GameObject should have if the dimension values are smaller than the maxSize
        float biggestDifference = 0; //the biggest dimension that is higher than the maxSize
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>()) //check all the renderers
        {
            Vector3 sizeCalculated = renderer.bounds.size; //the size of the model (so the max with maxSize=3 is (3.0, 3.0, 3.0))
            Debug.Log($"The calculated size is {sizeCalculated}");
            if (sizeCalculated.x > maxSize && sizeCalculated.x > biggestDifference)
            {
                biggestDifference = sizeCalculated.x;
            }
            if (sizeCalculated.y > maxSize && sizeCalculated.y > biggestDifference)
            {
                biggestDifference = sizeCalculated.y;
            }
            if (sizeCalculated.z > maxSize && sizeCalculated.z > biggestDifference)
            {
                biggestDifference = sizeCalculated.z;
            }
        }

        Vector3 newValue = new Vector3(baseScale, baseScale, baseScale);
        if (biggestDifference > 0)
        {
            float ratio = maxSize / biggestDifference; //the correction ratio for the new size
            newValue *= ratio; //multiple the baseScale Vector with it
            Debug.Log($"New scale: {newValue}, ratio: {ratio}");
        }
        ObjectScaling.Instance.ChangeSliderBaseValues(newValue.x); //we could use the y, z coordinates as well, they're the same
        obj.transform.localScale = newValue; //resize the GameObject
    }

}
