using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Timer timer;

    public bool PlayerMovement { get; private set; }
    public bool LevelEnd { get; private set; }
    public int Score { get; private set; }

    private void Start()
    {
        PlayerMovement = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }
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
        LevelEnd = true;
        uiManager.SetEndScreen(true);
        timer.StopTimer();
    }
}
