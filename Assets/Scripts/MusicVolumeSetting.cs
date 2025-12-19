using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSetting : MonoBehaviour
{
    public Slider musicSlider;

    public string prefsKey = "vol_music";

    public float defaultValue = 1f;

    float appliedValue;

    void Start()
    {
        appliedValue = PlayerPrefs.GetFloat(prefsKey, defaultValue);

        musicSlider.SetValueWithoutNotify(appliedValue);

        AudioManager.Instance?.SetMusic(appliedValue);

        musicSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float v)
    {
        AudioManager.Instance?.SetMusic(v);
    }

    public void Apply()
    {
        appliedValue = musicSlider.value;

        PlayerPrefs.SetFloat(prefsKey, appliedValue);
        PlayerPrefs.Save();
    }

    public void Revert()
    {
        musicSlider.SetValueWithoutNotify(appliedValue);
        AudioManager.Instance?.SetMusic(appliedValue);
    }
}
