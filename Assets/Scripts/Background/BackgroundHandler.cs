using UnityEngine;

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
        curvatureController.curvature = currentFloor * curvatureMagnitude;
        colorLerp.height = currentFloor * colorMagnitude;

        transform.position = new Vector3 (0, -heightFloor * Mathf.Abs(currentFloor - 1), 0);
        //Vector3 groundPos = curvatureController.gameObject.transform.position;
        //groundPos = new Vector3(groundPos.x, currentFloor * heightFloor, groundPos.y);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    curvatureController.curvature = currentFloor * curvatureMagnitude;
    //    colorLerp.height = currentFloor * colorMagnitude;

    //    transform.position = new Vector3(0, -heightFloor * Mathf.Abs(currentFloor - 1), 0);
    //    //Vector3 groundPos = curvatureController.gameObject.transform.position;
    //    //groundPos = new Vector3(groundPos.x, currentFloor * heightFloor, groundPos.y);
    //}
}
