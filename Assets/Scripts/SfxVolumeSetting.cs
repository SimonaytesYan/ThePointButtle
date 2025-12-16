using UnityEngine;
using UnityEngine.UI;

public class SfxVolumeSetting : MonoBehaviour
{
    public Slider sfxSlider;

    public string prefsKey = "vol_sfx";

    public float defaultValue = 1f;

    float appliedValue;

    void Start()
    {
        appliedValue = PlayerPrefs.GetFloat(prefsKey, defaultValue);

        sfxSlider.SetValueWithoutNotify(appliedValue);

        AudioManager.Instance?.SetSfx(appliedValue);

        sfxSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float v)
    {
        AudioManager.Instance?.SetSfx(v);
    }

    public void Apply()
    {
        appliedValue = sfxSlider.value;

        PlayerPrefs.SetFloat(prefsKey, appliedValue);
        PlayerPrefs.Save();
    }

    public void Revert()
    {
        sfxSlider.SetValueWithoutNotify(appliedValue);
        AudioManager.Instance?.SetSfx(appliedValue);
    }
}
