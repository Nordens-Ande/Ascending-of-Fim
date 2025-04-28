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

    [SerializeField] GameObject hitboxObject;
    //references

    public void EnemyDead() // everything that happens when enemy dies here
    {
        enemyWeaponInventory.EnemyDead(false, true);
        enemyWeaponInventory.UnEquip();
        enemyAiController.enabled = false;
        enemyMove.StopMoving();
        enemyMove.enabled = false;
        enemyShoot.enabled = false;
        ChangeHitboxLayer();
        ragDollController.BecomeRagDoll();
        StartCoroutine(DestroyGameObject());
    }

    void ChangeHitboxLayer() // changes the layer for the enemy hitbox object, the new layer gets ignored by all "bullets" and collision between other enemies and the player
                             // just turning of hitbox caused issues with physics
    {
        hitboxObject.layer = LayerMask.NameToLayer("EnemyIgnore");
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
