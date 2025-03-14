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
        if (StarterWeapon != null)
        {

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


    }

    void Update()
    {
        
    }
}
