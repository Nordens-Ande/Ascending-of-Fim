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
        layerMask = ~(LayerMask.GetMask("Weapon") | LayerMask.GetMask("EnemyIgnore") | LayerMask.GetMask("EnemyLimbs") | LayerMask.GetMask("PlayerLimbs") | LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("FurnitureJumpScare"));
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
            float offset = Random.Range(-0.3f, 0.3f); //offset only applied in x of "direction"
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
            Ray ray = BuildRay(i > 0);
            rays.Add(ray);
        }

        foreach(Ray ray in rays)
        {
            RaycastHit[] allHits;
            allHits = Physics.RaycastAll(ray, rayLength, layerMask);
            System.Array.Sort(allHits, (a, b) => a.distance.CompareTo(b.distance));

            foreach(RaycastHit hit in allHits)
            {
                ShieldScript shield = hit.collider.GetComponent<ShieldScript>();
                if(shield != null && shield.owner == this.gameObject) //sortera ut spelarens eller fiendens egna sköld, 
                {
                    continue;
                }

                hits.Add(hit);
                break;
            }
        }
        return hits;
    }

    void Update()
    {
        layerMask = ~(LayerMask.GetMask("Weapon") | LayerMask.GetMask("EnemyIgnore") | LayerMask.GetMask("EnemyLimbs") | LayerMask.GetMask("PlayerLimbs") | LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("FurnitureJumpScare"));
    }
}
