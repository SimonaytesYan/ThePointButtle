using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderPercentText : MonoBehaviour
{
    public Slider slider;
    public TMP_Text percentText;

    float lastValue = -1f;

    void Update()
    {
        if (slider == null || percentText == null) 
        {
            return;
        }
        
        float v = slider.value;

        if (!Mathf.Approximately(v, lastValue))
        {
            lastValue = v;
            int percent = Mathf.RoundToInt(v * 100f);
            percentText.text = percent + "";
        }
    }
}
