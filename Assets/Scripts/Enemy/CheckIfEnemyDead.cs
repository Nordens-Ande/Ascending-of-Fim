using System.Collections;
using UnityEngine;

public class CheckIfEnemyDead : MonoBehaviour
{
    [SerializeField] DropWeaponScript dropWeaponScript;
    [SerializeField] RagDollController ragDollController;
    [SerializeField] EnemyAIController enemyAiController;
    [SerializeField] EnemyMove enemyMove;
    [SerializeField] EnemyShoot enemyShoot;
    [SerializeField] EnemyWeaponInventory enemyWeaponInventory;
    //references

    public void EnemyDead() // everything that happens when enemy dies here
    {
        Debug.Log("enemyDead called");
        enemyWeaponInventory.EnemyDead(false, true); // sets IsEquipped to false, and enemyDead to true
        Debug.Log("1");
        dropWeaponScript.DropWeapon();
        Debug.Log("2");
        enemyAiController.enabled = false;
        enemyMove.enabled = false;
        enemyShoot.enabled = false;
        ragDollController.BecomeRagDoll();
        StartCoroutine(DestroyGameObject());
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
