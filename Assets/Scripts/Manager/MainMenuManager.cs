using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; set; }

    private int mainMenuScene = 0;
    private string sceneName = "Level";

    [Header("--- Main Menu ---")]
    public GameObject mainMenuPanel;
    public GameObject settingPanel;

    public Button closeSettingButton;

    public List<Button> menuButton;

    public GameObject pauseTxt;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        GameManager.Instance.LoadScene(1);
        SaveLoadManager.Instance.SaveGame(0);
    }

    public void ContinueGame()
    {
        int level = SaveLoadManager.Instance.LoadLevel();
        GameManager.Instance.LoadScene(level);
    }

    public void SetContinueButton(bool isContinue)
    {
        menuButton[1].interactable = isContinue ? true : false;
    }

    public void SettingGame()
    {
        SetActivePanel(settingPanel, true);
        ToggleButtonInteraction(false);
    }

    public void CloseSettingPanel()
    {
        SetActivePanel(settingPanel, false);
        ToggleButtonInteraction(true);

        if (SaveLoadManager.Instance.LoadLevel() == 0) menuButton[1].interactable = false;
    }

    public void CloseMainMenuPanel(bool isClosed)
    {
        SetActivePanel(mainMenuPanel, !isClosed);
    }

    public void SetActivePanel(GameObject panel, bool isActive)
    {
        panel.SetActive(isActive);
    }

    private void ToggleButtonInteraction(bool isInteractable)
    {
        foreach (Button button in menuButton)
        {
            button.interactable = isInteractable;
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
