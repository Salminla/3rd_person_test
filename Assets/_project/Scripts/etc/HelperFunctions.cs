using UnityEngine;

public class HelperFunctions
{
    public static string TimeConvertToString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        string seconds = Mathf.FloorToInt(time % 60).ToString("00");
        string milliSeconds = ((time % 1) * 99).ToString("00");
        return minutes.ToString("00") + ":" + seconds + ":" + milliSeconds;
    }
}
