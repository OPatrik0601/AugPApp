using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectScaling : MonoBehaviour
{
    private static ObjectScaling _instance = null;
    public static ObjectScaling Instance
    {
        get
        {
            if (_instance == null)
                _instance = Object.FindObjectOfType<ObjectScaling>();

            return _instance;
        }
    }

    private Slider scaleSlider = null;

    private void Awake()
    {
        scaleSlider = gameObject.GetComponent<Slider>();
        scaleSlider.onValueChanged.AddListener(onScaleSliderValueChanged);
    }

    private void onScaleSliderValueChanged(float value)
    {
        if(ImageRecognition.Instance.CreatedGameObject)
            ImageRecognition.Instance.CreatedGameObject.transform.localScale = new Vector3(value, value, value);
    }

    /// <summary>
    /// Change the slider minvalue, maxvalue & current value
    /// </summary>
    /// <param name="normalSize">The current value, one dimension of the scale property</param>
    public void ChangeSliderValues(float normalSize)
    {
        float maximum = normalSize * 1.9f; //maximum is 190%
        float minimum = normalSize * 0.1f; //minimum is 10%
        float currentValue = normalSize;
        scaleSlider.minValue = minimum;
        scaleSlider.maxValue = maximum;
        scaleSlider.value = currentValue;
    }

}
