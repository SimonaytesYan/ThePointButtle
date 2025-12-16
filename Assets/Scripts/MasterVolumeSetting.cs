using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeSetting : MonoBehaviour
{
    public Slider masterSlider;

    public string prefsKey = "vol_master";

    public float defaultValue = 1f;

    float appliedValue;

    void Start()
    {
        appliedValue = PlayerPrefs.GetFloat(prefsKey, defaultValue);

        masterSlider.SetValueWithoutNotify(appliedValue);

        AudioManager.Instance?.SetMaster(appliedValue);

        masterSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float v)
    {
        AudioManager.Instance?.SetMaster(v);
    }

    public void Apply()
    {
        appliedValue = masterSlider.value;

        PlayerPrefs.SetFloat(prefsKey, appliedValue);
        PlayerPrefs.Save();
    }

    public void Revert()
    {
        masterSlider.SetValueWithoutNotify(appliedValue);
        AudioManager.Instance?.SetMaster(appliedValue);
    }
}
