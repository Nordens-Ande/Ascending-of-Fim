using System.Collections;
using UnityEngine;

public class CheckIfEnemyDead : MonoBehaviour
{
    [SerializeField] DropWeaponScript dropWeaponScript;
    [SerializeField] RagDollController ragDollController;
    [SerializeField] EnemyAIController enemyAiController;
    [SerializeField] EnemyMove enemyMove;
    [SerializeField] EnemyShoot enemyShoot;
    //references

    public void EnemyDead() // everything that happens when enemy dies here
    {
        dropWeaponScript.DropWeapon();
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
