using UnityEngine;

public class SkyboxColorLerp : MonoBehaviour
{
    [Header("Player/Camera to track height")]
    public float height;

    [Header("Height range")]
    public float minHeight = 0f;
    public float maxHeight = 100f;

    [Header("Skybox Colors")]
    [SerializeField] Color skyColorStart = new Color(0.4f, 0.6f, 1f); // Blue
    [SerializeField] Color skyColorEnd = Color.black;

    [SerializeField] Color groundColorStart = new Color(0.4f, 0.6f, 1f); // Darker Blue-Grey
    [SerializeField] Color groundColorEnd = Color.black;

    private void Update()
    {
        float t = Mathf.InverseLerp(minHeight, maxHeight, height);
        Color skyColor = Color.Lerp(skyColorStart, skyColorEnd, t);
        Color groundColor = Color.Lerp(groundColorStart, groundColorEnd, t);

        RenderSettings.skybox.SetColor("_SkyTint", skyColor);
        RenderSettings.skybox.SetColor("_GroundColor", groundColor);
        // Optional: adjust exposure or atmosphere thickness if needed
        // RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(1.0f, 0.1f, t));
    }
}
