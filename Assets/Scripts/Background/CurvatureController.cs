using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class CurvatureController : MonoBehaviour
{
    [Range(-1, 1)]
    public float curvature = 0.1f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // e.g. make it pulse over time
        //float dynamicCurv = curvature + 0.05f * Mathf.Sin(Time.time * 2);
        mat.SetFloat("_CurveAmount", curvature);
    }
}

