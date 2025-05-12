using BarthaSzabolcs.IsometricAiming;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    [SerializeField] RagDollController ragDollController;
    [SerializeField] EquipWeapon equipWeapon;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] IsometricAiming playerLook;

    bool isPlayerDead = false;

    public void PlayerDead()
    {
        if(isPlayerDead) return;
        isPlayerDead = true;
        //equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().Unequip();
        equipWeapon.UnEquip();
        playerMove.enabled = false;
        playerLook.enabled = false;
        ragDollController.BecomeRagDoll();
    }

    public void PlayerRevive()
    {
        if(!isPlayerDead) return;
        isPlayerDead=false;
        playerMove.enabled = true;
        playerLook.enabled = true;
        ragDollController.NoLongerRagDoll();
    }
}
