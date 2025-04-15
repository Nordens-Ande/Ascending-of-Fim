using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    int health;

    [SerializeField] DropWeaponScript dropWeaponScript;

    void Start()
    {
        health = 50;
    }

    public void ApplyDamage(int damage)
    {
        Debug.Log("damage taken");
        health -= damage;
        Debug.Log(health);
    }

    void Update()
    {
        if(health < 0)
        {
            dropWeaponScript.DropWeapon();
            Debug.Log("DropWeapon called");
            Destroy(gameObject);
        }
    }
}
