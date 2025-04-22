using UnityEngine;

[RequireComponent(typeof(Light))]
public class ElevatorLight : MonoBehaviour
{
    public float colorChangeSpeed = 1f;
    private Light pointLight;

    void Start()
    {
        pointLight = GetComponent<Light>();
    }

    void Update()
    {
        float hue = Mathf.Repeat(Time.time * colorChangeSpeed, 1f);
        pointLight.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
