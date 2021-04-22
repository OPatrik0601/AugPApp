using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputDetection : MonoBehaviour
{
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    private float currentObjectSize;

    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
                currentObjectSize = ObjectScaling.Instance.CurrentSize;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    checkSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                checkSwipe();
            }
        }

        if (Input.GetKey(KeyCode.Escape)) //back button on Android
        {
            if (ArticleInfos.Instance.IsShown)
                ArticleInfos.Instance.ClickedToggle();
            else
            {
                BackToScanner();
            }
        }
    }

    void checkSwipe()
    {
        //Check if Vertical swipe
        if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }

    public void BackToScanner()
    {
        SceneHandler.StartLoadScannerScene();
    }

    private float verticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    private float horizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    private void OnSwipeUp()
    {
        //ObjectScaling.Instance.ChangeSliderValue(1);
    }

    private void OnSwipeDown()
    {
        //ObjectScaling.Instance.ChangeSliderValue(-1);
    }

    private void OnSwipeLeft()
    {
        if(ImageRecognition.Instance.CreatedGameObject != null)
        {
            if (currentObjectSize != ObjectScaling.Instance.CurrentSize) //the user is changing the object size on the UI (by swiping), so skip the check
                return;

            ImageRecognition.Instance.CreatedGameObject.transform.Rotate(0, -5, 0);
        }
    }

    private void OnSwipeRight()
    {
        if (ImageRecognition.Instance.CreatedGameObject != null)
        {
            if (currentObjectSize != ObjectScaling.Instance.CurrentSize) //the user is changing the object size on the UI (by swiping), so skip the check
                return;

            ImageRecognition.Instance.CreatedGameObject.transform.Rotate(0, 5, 0);
        }
    }
}
