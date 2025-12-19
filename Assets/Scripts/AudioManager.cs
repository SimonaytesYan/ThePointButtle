using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer mixer;
    public string masterParam = "MasterVolume";
    public string musicParam  = "MusicVolume";
    public string sfxParam    = "SFXVolume";

    public AudioSource musicSource;
    public AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float master = PlayerPrefs.GetFloat("vol_master", 1f);
        float music  = PlayerPrefs.GetFloat("vol_music", 1f);
        float sfx    = PlayerPrefs.GetFloat("vol_sfx", 1f);

        SetMaster(master);
        SetMusic(music);
        SetSfx(sfx);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) 
        { 
            return; 
        }
        if (musicSource.clip == clip && musicSource.isPlaying) 
        { 
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) 
        {
            return;
        }
        sfxSource.PlayOneShot(clip, volume);
    }


    public void SetMaster(float value) => SetVolume(masterParam, value);
    public void SetMusic(float value)  => SetVolume(musicParam, value);
    public void SetSfx(float value)    => SetVolume(sfxParam, value);

    void SetVolume(string param, float value01)
    {
        if (mixer == null) 
        { 
            return;
        }

        value01 = Mathf.Clamp(value01, 0.0001f, 1f);
        float db = Mathf.Log10(value01) * 20f;   
        mixer.SetFloat(param, db);
    }
}
