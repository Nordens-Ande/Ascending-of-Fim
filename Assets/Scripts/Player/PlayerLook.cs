using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerLook : MonoBehaviour
{
    //[SerializeField] public int mouseSensitivity;
    [SerializeField] GameObject rotationObject;
    Vector2 mouse;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rot = Mathf.Atan2(transform.position.z - mouse.y, transform.position.x - mouse.x);

        rotationObject.transform.rotation = Quaternion.Euler(0, rot, 0);
    }

    private void OnLook(InputValue inputValue)
    {
        mouse = inputValue.Get<Vector2>();
    }
}
