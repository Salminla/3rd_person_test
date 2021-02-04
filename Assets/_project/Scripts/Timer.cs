using UnityEngine;

public class Timer
{
    public float TimerCount { get; private set; }
    private bool timerActive = false;
    
    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (timerActive)
        {
            TimerCount += Time.deltaTime;
        }
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
    public void ResetTimer()
    {
        TimerCount = 0f;
    }
}
