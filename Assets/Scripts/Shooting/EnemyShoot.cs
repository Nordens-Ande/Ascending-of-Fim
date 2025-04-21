using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Shoot shootScript;
    int rayLength;
    bool lineOfSight;

    private void Start()
    {
        rayLength = 100;
    }

    private void Update()
    {
        if(CheckForLineOfSight()) // && firerate
        {
            RaycastHit hit = shootScript.ShootRay();
            CheckRay(hit);
        }
    }

    bool CheckForLineOfSight()
    {
        Ray ray = new Ray(transform.position, player.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, rayLength))
        {
            
        }

        if (hit.transform.tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckRay(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                //hit.transform.gameObject.GetComponent<PlayerHealth>().ApplyDamage(weaponData.damage);
            }
        }
    }
}
