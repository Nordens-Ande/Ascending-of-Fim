using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Shoot : MonoBehaviour
{
    int rayLength;
    [SerializeField] GameObject bulletOrigin; //placera child objectet som vapnet ska ha här, skottets/rayens origin.
    LayerMask layerMask;

    void Start()
    {
        rayLength = 10000;
        layerMask = ~LayerMask.GetMask("Weapon");
    }

    Vector3 GetDirection()
    {
        Vector3 direction = bulletOrigin.transform.forward;
        direction.y = 0;
        return direction;
    }

    Ray BuildRay(bool applyRandomness)
    {
        Vector3 direction = GetDirection();

        if(applyRandomness)
        {
            float offset = Random.Range(-0.3f, 0.3f); //offset only applied in x of direction
            direction.x += offset;
        }

        Ray ray = new Ray(bulletOrigin.transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * rayLength);
        return ray;
    }

    public List<RaycastHit> ShootRay(int amountOfBullets) // amountOfBullets used to make shotguns shoot multiple rays
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        List<Ray> rays = new List<Ray>();

        for(int i = 0; i < amountOfBullets; i++)
        {
            if (i > 0)
            {
                Ray ray = BuildRay(true); // applies a bit of randomness in direction
                rays.Add(ray);
            }
            else
            {
                Ray ray = BuildRay(false); // doesnt apply randomness in direction, and makes atleast one shotgun pellet go straight
                rays.Add(ray);
            }
        }

        foreach(Ray ray in rays)
        {
            RaycastHit hit;
            Physics.Raycast(ray, out hit, rayLength, layerMask);
            hits.Add(hit);
        }

        return hits;
    }
}
