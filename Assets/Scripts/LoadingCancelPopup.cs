using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingCancelPopup : MonoBehaviour
{
    public GameObject popup;  

    public string menuSceneName = "Menu";
    public string playerSceneName = "SampleScene";

    bool popupOpened = false;

    void Start()
    {
        if (popup != null) 
        {
            popup.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!popupOpened)
                OpenPopup();
            else
                ClosePopup();
        }
    }

    void OpenPopup()
    {
        popupOpened = true;
        popup.SetActive(true);

        Time.timeScale = 0f;
    }

    void ClosePopup()
    {
        popupOpened = false;
        popup.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ConfirmExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(playerSceneName);
    }

    public void CancelExit()
    {
        ClosePopup();
        SceneManager.LoadScene(menuSceneName);
    }
}
