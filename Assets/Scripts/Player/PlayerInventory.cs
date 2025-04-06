using UnityEngine;
using UnityEngine.InputSystem;

public interface IInventory
{
    void ChangeEquipped(GameObject item);
    void AddReplaceItem(GameObject item);
}

public class PlayerInventory : MonoBehaviour, IInventory
{
    [SerializeField][Min(1)] int MaxPrimaries;
    [SerializeField][Min(1)] int MaxSecondaries;
    [SerializeField] GameObject StarterWeapon;

    public GameObject equipped;
    private GameObject[] primary;
    private GameObject[] secondary;

    void Start()
    {
        primary = new GameObject[MaxPrimaries];
        secondary = new GameObject[MaxSecondaries];

        if (StarterWeapon != null)
        {
            equipped = StarterWeapon;
            AddReplaceItem(StarterWeapon);
        }
    }

    public void ChangeEquipped(GameObject item)
    {
        IWeapon weapon = item.GetComponent<IWeapon>();
        if (weapon == null)
            return;

        equipped = item;

        WeaponData data = weapon.GetWeaponData();

        switch (data.weaponType)
        {
            case WeaponData.Type.Primary:
                {
                    primary[0] = item;
                    break;
                }
            case WeaponData.Type.Secondary:
                {
                    secondary[0] = item;
                    break;
                }
        }
    }

    //public void AddReplaceItem(GameObject item)
    //{
    //    IInteractable interactable = item.GetComponent<IInteractable>();
    //    if (interactable == null)
    //        return;

    //    switch (interactable.GetType())
    //    {
    //        case IInteractable.Type.Primary:
    //            {
    //                for (int i = 0; i < primary.Length; i++)
    //                {
    //                    if (primary[i] == null)
    //                    {
    //                        primary[i] = item;
    //                        interactable.PickUp(transform);
    //                        equipped = item;
    //                    }
    //                    else  if (primary[i] == equipped)
    //                    {
    //                        IInteractable primInteract = primary[i].GetComponent<IInteractable>();
    //                        if (primInteract == null)
    //                            continue;

    //                        primInteract.Drop();

    //                        primary[i] = item;
    //                        interactable.PickUp(transform);
    //                        equipped = item;
    //                    }
    //                }
    //                break;
    //            }
    //    }

    //}

    public void AddReplaceItem(GameObject item)
    {
        IInteractable interactable = item.GetComponent<IInteractable>();
        if (interactable == null)
            return;

        switch (interactable.GetType())
        {
            case IInteractable.Type.Primary:
                {
                    for (int i = 0; i < primary.Length; i++)
                    {
                        if (primary[i] == null)
                        {
                            primary[i] = item;
                            interactable.PickUp(transform);
                            equipped = item;
                        }
                        else if (primary[i] == equipped)
                        {
                            IInteractable primInteract = primary[i].GetComponent<IInteractable>();
                            if (primInteract == null)
                                continue;

                            primInteract.Drop();

                            primary[i] = item;
                            interactable.PickUp(transform);
                            equipped = item;
                        }
                    }
                    break;
                }
        }

    }

    void Update()
    {
        
    }
}
