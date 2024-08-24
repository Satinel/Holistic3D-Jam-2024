using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Canvas _optionsCanvas;
    [SerializeField] GameObject _closeMenuButton;
    [SerializeField] GameObject _quitPrompt;
    [SerializeField] GameObject _quitGameButton;


    public void EnableOptionsCanvas()
    {
        _optionsCanvas.enabled = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_closeMenuButton);
    }

    public void DisableOptionsCanvas()
    {
        _optionsCanvas.enabled = false;
    }

    public void OpenOptions()
    {
        _optionsCanvas.gameObject.SetActive(true);
        EnableOptionsCanvas();
    }

    public void CloseOptions()
    {
        _optionsCanvas.gameObject.SetActive(false);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PromptQuit()
    {
        _quitPrompt.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_quitGameButton);
    }

    public void CancelQuit()
    {
        _quitPrompt.SetActive(false);
        EnableOptionsCanvas();
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }
}
