using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    int rayLength;
    [SerializeField] GameObject bulletOrigin; //placera child objectet som vapnet ska ha här, skottets/rayens origin.

    void Start()
    {
        rayLength = 10000;
    }

    Vector3 GetDirection()
    {
        Vector3 direction = bulletOrigin.transform.forward;
        direction.y = 0;
        return direction;
    }

    Ray BuildRay()
    {
        Ray ray = new Ray(bulletOrigin.transform.position, GetDirection());
        Debug.DrawRay(ray.origin, ray.direction * rayLength);
        return ray;
    }

    public RaycastHit ShootRay()
    {
        Ray ray = BuildRay();
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength))
        {
            return hit;
        }
        return new RaycastHit();
    }

}
