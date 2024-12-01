using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    [Header("--- Level ---")]
    public int toalLevel = 3;
    public int currentLevel = 0;
    public bool isPause = false;

    private int mainMenuScene = 0;
    private string sceneName = "Level";

    private PlayerInputActions input;


    [SerializeField] private Animator animator;
    private int animLoadIn, animLoadOut;

    private float loadAnimTime = 1f, loadSceneTime = 1f;

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

        input = new PlayerInputActions();
    }

    private void Start()
    {
        if (CheckGameDataContinue()) MainMenuManager.Instance.SetContinueButton(true);
        else MainMenuManager.Instance.SetContinueButton(false);

        input.Enable();
        input.Menu.Pause.performed += _ => OnPauseGame();

        animLoadIn = Animator.StringToHash("loadin");
        animLoadOut = Animator.StringToHash("loadout");
    }

    public void OnPauseGame()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            isPause = !isPause;
            if (isPause)
            {
                Time.timeScale = 0f;
                MainMenuManager.Instance.SetActivePanel(MainMenuManager.Instance.settingPanel ,true);
                MainMenuManager.Instance.SetActivePanel(MainMenuManager.Instance.pauseTxt ,true);
                LockCursor(false);
            }
            else
            {
                Time.timeScale = 1f;
                MainMenuManager.Instance.CloseSettingPanel();
                MainMenuManager.Instance.SetActivePanel(MainMenuManager.Instance.pauseTxt, false);
                LockCursor(true);
            }
        }
        else
        {
            MainMenuManager.Instance.CloseSettingPanel();
        }
    }

    public async void LoadScene(int level)
    {
        if (level != 0) currentLevel = level;
        else currentLevel = 0;

        if (level > 1) SaveLoadManager.Instance.SaveGame(level);

        MainMenuManager.Instance.CloseMainMenuPanel(true);
        animator.SetTrigger(animLoadIn);
        await UniTask.WaitForSeconds(loadAnimTime);
        
        SceneManager.LoadScene(sceneName + " " + level);

        await UniTask.WaitForSeconds(loadSceneTime);

        SceneSetUp();
    }

    private void SceneSetUp()
    {
        animator.SetTrigger(animLoadOut);
        
        AudioManager.Instance.PlayThemeSound(currentLevel);
        if (currentLevel != mainMenuScene)
        {
            EnemyManager.Instance.SetUpEnemy(currentLevel);
            UIManager.Instance.SetLevelNumber(currentLevel);
            LockCursor(true);

            AudioManager.Instance.FindSFXSound();
        }
        else
        {
            if (CheckGameDataContinue()) MainMenuManager.Instance.SetContinueButton(true);
            else MainMenuManager.Instance.SetContinueButton(false);
            Time.timeScale = 1f;
            MainMenuManager.Instance.CloseMainMenuPanel(false);
            LockCursor(false);
        }
    }

    public void LockCursor(bool isLock)
    {
        Cursor.lockState = isLock ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private bool CheckGameDataContinue()
    {
        return SaveLoadManager.Instance.LoadLevel() != mainMenuScene;
    }
}
