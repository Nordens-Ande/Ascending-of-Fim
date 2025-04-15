using UnityEngine;

public class CheckIfEnemyDead : MonoBehaviour
{
    [SerializeField] DropWeaponScript dropWeaponScript;
    //references

    public void EnemyDead() // everything that happens when enemy dies here
    {
        dropWeaponScript.DropWeapon();
        Destroy(gameObject);
    }
}
