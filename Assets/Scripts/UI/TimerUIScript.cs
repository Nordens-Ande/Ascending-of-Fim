using TMPro;
using UnityEngine;

public class TimerUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;

    private bool isRunning = true;

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        PlayerStats.elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(PlayerStats.elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(PlayerStats.elapsedTime % 60f);

        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void StopTimer()
    {
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        PlayerStats.elapsedTime = 0f;
    }

    public float getTime()
    {
        return PlayerStats.elapsedTime;
    }
}
