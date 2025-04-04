using TMPro;
using UnityEngine;

public class TimerUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;

    private float elapsedTime = 0f;
    private bool isRunning = true;

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

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
        elapsedTime = 0f;
    }
}
