using UnityEngine;


public class TimeData : UIEventData
{
    public float CurrentTime { get; set; }
    public float MaxTime { get; set; }

    public TimeData(float currentTime, float maxTime)
    {
        CurrentTime = currentTime;
        MaxTime = maxTime;
    }

    public string GetTimeString()
    {
        var minutes = Mathf.FloorToInt(CurrentTime / 60);
        var seconds = Mathf.FloorToInt(CurrentTime % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}