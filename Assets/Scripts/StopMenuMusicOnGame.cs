using UnityEngine;

public class StopMenuMusicOnGame : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
        }
    }
}
