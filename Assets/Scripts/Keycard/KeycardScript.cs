using UnityEngine;
using System.Collections;

public class KeycardScript : MonoBehaviour
{
    [SerializeField] private float keycardRotationSpeed;
    private GameManager gameManager;

    public bool isRotating { get; set; }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        isRotating = true;
    }

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up * keycardRotationSpeed * (1 - Mathf.Exp(-keycardRotationSpeed * Time.deltaTime)));
        }
    }

    public void Equip()
    {
        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }
        isRotating = false;
        gameObject.SetActive(false);

        Debug.LogWarning("Keycard equipped");
        gameManager.PlayerFoundKeycard();
    }
}
