using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public enum Type
    {
        Primary,
        Secondary,
        Throwable,
    }
    Type GetType();

    void PickUp(Transform inventoryTransform);
    void Drop();
}

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] GameObject Inventory;
    [SerializeField] float PickupRange = 2f;

    private List<GameObject> nearbyItems = new List<GameObject>();
    private IInventory inventory;

    void Start()
    {
        inventory = GetComponent<IInventory>();
        if (Inventory == null || inventory == null)
            Debug.Log("Failed Inventory");
    }

    private void OnInteract(InputValue inputValue)
    {
        DetectNearbyItems();
        inventory.AddReplaceItem(nearbyItems[0]);
        nearbyItems.RemoveAt(0);
    }

    private void DetectNearbyItems()
    {
        nearbyItems.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, PickupRange);

        foreach (Collider collided in colliders)
        {
            IInteractable interactable = collided.GetComponent<IInteractable>();
            if (interactable != null)
            {
                nearbyItems.Add(collided.gameObject);
            }
        }
    }

    void Update()
    {
        //Kanske lägga DetectNearbyItems() här istället?
    }
}
