using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Timer timer;
    [SerializeField] private FirebaseLeaders leaders;
    [SerializeField] private TMP_InputField userNameInput;
    
    public bool PlayerMovement { get; private set; }
    public bool CameraMovement { get; private set; }
    public bool LevelEnd { get; private set; }
    public bool GamePaused { get; private set; }
    public int Score { get; private set; }

    private bool windowFocus = true;

    private void Start()
    {
        PlayerMovement = true;
        CameraMovement = true;
        windowFocus = Application.isFocused;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            TestSceneSelection();
        }

        if (Input.GetKeyDown((KeyCode.Escape)) || (!Application.isFocused && windowFocus))
        {
            windowFocus = false;
            TogglePause();
        }
    }

    private static void TestSceneSelection()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }

    private void TogglePause()
    {
        if (!GamePaused && !LevelEnd)
        {
            GamePaused = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            uiManager.SetPauseScreen(true);
            return;
        }
        
        windowFocus = true;
        GamePaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        uiManager.SetPauseScreen(false);
    }

    public void SetPlayerMovement(bool state)
    {
        PlayerMovement = false;
    }
    public void IncreaseScore()
    {
        Score++;
    }
    public void EndLevel()
    {
        SetPlayerMovement(false);
        CameraMovement = false;
        LevelEnd = true;

        if (leaders != null && userNameInput != null)
        {
            String playerName;
            playerName = userNameInput.text == "" ? "Anonomyous" : userNameInput.text;
                
            leaders.PlayerFinished(playerName, timer.TimerCount);
        }

        if (uiManager != null)
            uiManager.SetEndScreen(true);
        
        if (timer != null)
            timer.StopTimer();
    }
}
