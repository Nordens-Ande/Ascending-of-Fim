using UnityEngine;
using UnityEngine.InputSystem;

public interface IInventory
{
    void ChangeEquipped(GameObject item);
    void AddReplaceItem(GameObject item);
}

public class PlayerInventory : MonoBehaviour, IInventory
{
    [SerializeField] int MaxPrimaries;
    [SerializeField] int MaxSecondaries;
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

    }

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
                        if (primary[i] == equipped)
                        {

                        }

                        primary[i] = primary[i] == null ? item : primary[i];
                    }
                        


                    break;
                }
        }

    }

    void Update()
    {
        
    }
}
