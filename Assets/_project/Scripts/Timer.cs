using UnityEngine;

public class Timer : MonoBehaviour
{
    public float TimerCount { get; private set; }
    private bool timerActive;
    
    void Start()
    {
        StartTimer();
    }

    void Update()
    {
        if (timerActive)
            TimerCount += Time.deltaTime;
    }

    public void StartTimer()
    {
        ResetTimer();
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }
    public void ContinueTimer()
    {
        timerActive = true;
    }
    public void ResetTimer()
    {
        TimerCount = 0f;
    }

    public string GetTimeString()
    {
        float minutes = Mathf.FloorToInt(TimerCount / 60);
        string seconds = Mathf.FloorToInt(TimerCount % 60).ToString("00");
        string milliSeconds = ((TimerCount % 1) * 99).ToString("00");
        return minutes.ToString("00") + ":" + seconds + ":" + milliSeconds;
    }
}
