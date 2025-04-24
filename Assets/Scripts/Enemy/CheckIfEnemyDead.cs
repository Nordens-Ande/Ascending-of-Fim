using System.Collections;
using UnityEngine;

public class CheckIfEnemyDead : MonoBehaviour
{
    [SerializeField] RagDollController ragDollController;
    [SerializeField] EnemyAIController enemyAiController;
    [SerializeField] EnemyMove enemyMove;
    [SerializeField] EnemyShoot enemyShoot;
    [SerializeField] EnemyWeaponInventory enemyWeaponInventory;
    [SerializeField] HUDHandler hudHandler;

    [SerializeField] Collider hitboxCollider;
    //references

    public void EnemyDead() // everything that happens when enemy dies here
    {
        enemyWeaponInventory.EnemyDead(false, true);
        enemyWeaponInventory.UnEquip();
        enemyAiController.enabled = false;
        enemyMove.StopMoving();
        enemyMove.enabled = false;
        enemyShoot.enabled = false;
        DisablePlayerEnemyCollision();
        ragDollController.BecomeRagDoll();
        StartCoroutine(DestroyGameObject());
    }

    void DisablePlayerEnemyCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider playerHitbox = null;
        foreach(Collider collider in player.GetComponentsInChildren<Collider>())
        {
            if (collider.CompareTag("PlayerHitbox")) // makes sure its the hitbox collider and not a limb / ragdoll collider
            {
                playerHitbox = collider;
                break;
            }
        }

        if(playerHitbox == null) return;

        Physics.IgnoreCollision(hitboxCollider, playerHitbox);
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
