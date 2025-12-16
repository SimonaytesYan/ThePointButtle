using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayButton : MonoBehaviour
{
    public string loadingSceneName = "Loading";

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(loadingSceneName);
    }
}
