using UnityEngine;

//Denna klass hanterar hela bakgrunds milön på spelet. Här uppdaterar och hanterar vi värdena på SkyboxColorLerp och CurvatureController.
public class BackgroundHandler : MonoBehaviour
{
    [SerializeField] SkyboxColorLerp colorLerp;
    [SerializeField] CurvatureController curvatureController;
    
    [Space]

    public int maxFloor;
    public int currentFloor;
    public float heightFloor;
    public float curvatureMagnitude;
    public float colorMagnitude;

    void Start()
    {
        UpdateValues();
    }

    //En metod som uppdaterar värdena hos Skybox och marken.
    public void UpdateValues()
    {
        curvatureController.curvature = currentFloor * curvatureMagnitude;
        colorLerp.height = currentFloor * colorMagnitude;

        transform.position = new Vector3(0, -heightFloor * Mathf.Abs(currentFloor - 1), 0);
    }

    void Update()
    {
        UpdateValues();
    }
}
