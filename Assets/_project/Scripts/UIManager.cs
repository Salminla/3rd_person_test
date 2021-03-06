using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject endSreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Timer timer;
    [SerializeField] private TMP_Text uiTimerText;
    [SerializeField] private TMP_Text endTimeText;
    [SerializeField] private GameObject reticuleNormal;
    [SerializeField] private GameObject reticuleTarget;

    public Text debugText;
    public Text debugText2;

    public Text interactPrompt;
    private bool istimerNotNull;
    
    public bool ReticuleTarget { get; private set; }

    private void Start()
    {
        istimerNotNull = timer != null;
    }

    private void Update()
    {
        if (istimerNotNull)
            UpdateTimer();
    }

    private void UpdateTimer()
    {
        uiTimerText.text = timer.GetTimeString();
    }

    public void SetEndScreen(bool state)
    {
        gameUI.gameObject.SetActive(!state);
        endSreen.gameObject.SetActive(state);
        endTimeText.text = timer.GetTimeString();
    }

    public void SetPauseScreen(bool state)
    {
        pauseScreen.gameObject.SetActive(state);
    }

    public void SwitchReticule()
    {
        if (!ReticuleTarget)
        {
            reticuleTarget.gameObject.SetActive(true);
            ReticuleTarget = true;
        }
        else
        {
            reticuleTarget.gameObject.SetActive(false);
            ReticuleTarget = false;
        }
    }
    //Legacy code
    #region legacy

    public void SetInteractPrompt(bool state)
    {
        interactPrompt.gameObject.SetActive(state);
    }

    public void SetDebugUI(int num, string content)
    {
        if (num == 1)
        {
            debugText.text = content;
        }
        else if (num == 2)
        {
            debugText2.text = content;
        }
        else
        {
            Debug.LogError("Invalid debug UI num!");
        }
    }

    #endregion
}
