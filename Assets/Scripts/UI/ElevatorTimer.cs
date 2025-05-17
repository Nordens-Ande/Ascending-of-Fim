using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 60f;
    [SerializeField] private UnityEvent onTimerEnd;

    private float remainingTime;
    private bool isRunning = false;

    void Start()
    {
        ResetTimer();
        StartTimer();
    }

  
    void Update()
    {
        if (!isRunning || remainingTime <= 0f) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
            onTimerEnd?.Invoke();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        remainingTime = startTime;
        Update();  // Ensure the UI is updated immediately
    }

    public void SetStartTime(float time)
    {
        startTime = time;
        ResetTimer();
    }

}
