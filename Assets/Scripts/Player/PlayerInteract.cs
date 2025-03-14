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
    void GetType();

    void Interact();
    void PickUp(Transform inventoryTransform);
    void Drop();
}

public class PlayerInteract : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnInteract(InputValue inputValue)
    {

    }

    void Update()
    {
        
    }
}
