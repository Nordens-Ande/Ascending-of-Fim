using UnityEngine;

//Denna klass hanterar shadern som kurvar marken. Denna controllerna är till för att kunna ändra värdena på shadern.
[RequireComponent(typeof(Renderer))]
public class CurvatureController : MonoBehaviour
{
    [Range(-1, 1)] public float curvature = 0.1f;
    public Color planeColor;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        //Uppdaterar värderna på shader materialet.
        mat.SetFloat("_CurveAmount", Mathf.Clamp(curvature, -1, 1));
        mat.SetColor("_Color", planeColor);
    }
}

