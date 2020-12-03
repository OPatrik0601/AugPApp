using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectScaling : MonoBehaviour
{
    private Slider scaleSlider = null;

    private void Start()
    {
        scaleSlider = gameObject.GetComponent<Slider>();
        scaleSlider.onValueChanged.AddListener(onScaleSliderValueChanged);
    }

    private void onScaleSliderValueChanged(float value)
    {
        if(ImageRecognition.Instance.CreatedGameObject)
            ImageRecognition.Instance.CreatedGameObject.transform.localScale = new Vector3(value, value, value);
    }

}
