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
        layerMask = ~(LayerMask.GetMask("Weapon") | LayerMask.GetMask("EnemyIgnore") | LayerMask.GetMask("EnemyLimbs") | LayerMask.GetMask("PlayerLimbs") | LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("FurnitureJumpScare") | LayerMask.GetMask("Keycard"));
    }

    Vector3 GetDirection() //returns the direction the ray should go
    {
        Vector3 direction = bulletOrigin.transform.forward;
        direction.y = 0;
        return direction;
    }

    Ray BuildRay(bool applyRandomness)
    {
        Vector3 direction = GetDirection();

        if(applyRandomness) //apply offset, making the ray shoot a bit to the side, for the shotgun spread
        {
            float offset = Random.Range(-0.18f, 0.18f); //offset only applied in x of "direction"
            direction += bulletOrigin.transform.right * offset;
        }

        Ray ray = new Ray(bulletOrigin.transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * rayLength);
        return ray;
    }

    public List<RaycastHit> ShootRay(int amountOfBullets) // amountOfBullets used to make shotguns shoot multiple rays
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        List<Ray> rays = new List<Ray>();

        for(int i = 0; i < amountOfBullets; i++) //shoot a raycast for every bullet shot, 1 bullet for pistols and rifle, 8 shoots for shotgun
        {
            Ray ray = BuildRay(i > 0); //determine if the ray getting "built" should have an offset in the, creates a spread for the shotgun
            rays.Add(ray);
        }

        foreach(Ray ray in rays)
        {
            RaycastHit[] allHits;
            allHits = Physics.RaycastAll(ray, rayLength, layerMask); // get all colliders the ray hit, layermask sorts out dead enemies, weapons on the ground etc
            System.Array.Sort(allHits, (a, b) => a.distance.CompareTo(b.distance)); //sort them by distance

            foreach(RaycastHit hit in allHits)
            {
                ShieldScript shield = hit.collider.GetComponent<ShieldScript>();
                if(shield != null && shield.owner == this.gameObject) //sort out the enemy or players own shield because the ray starts
                                                                      //infront of the enemy or player and the shield would block the ray
                {
                    continue;
                }

                hits.Add(hit); // add the first collider hit to a list every iteration
                break;
            }
        }
        return hits; // return what colliders were hit
    }

    void Update()
    {
        layerMask = ~(LayerMask.GetMask("Weapon") | LayerMask.GetMask("EnemyIgnore") | LayerMask.GetMask("EnemyLimbs") | LayerMask.GetMask("PlayerLimbs") | LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("FurnitureJumpScare") | LayerMask.GetMask("Keycard"));
    }
}
