using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Shoot shootScript;
    // beh�vs mer info g�llande vapen f�r fireRate, Damage etc.

    void OnAttack(InputValue input)
    {
        RaycastHit hit = shootScript.ShootRay();
        CheckRay(hit);
    }

    void CheckRay(RaycastHit hit)
    {
        if(hit.collider != null)
        {
            if (hit.transform.tag == "Enemy")
            {
                //hit.transform.GetComponent<"script">.ReduceHealth("weapon damage")
            }
        }
    }
}
