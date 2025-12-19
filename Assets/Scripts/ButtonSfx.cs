using UnityEngine;

public class ButtonSfx : MonoBehaviour
{
    public AudioClip clickClip;

    public void PlayClick()
    {
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlaySfx(clickClip);
        }   
    }
}
