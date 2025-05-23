﻿using UnityEngine;
using UnityEngine.InputSystem;

public class EquipKeycard : MonoBehaviour
{
    [Header("Ray settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLengt;
    [SerializeField] private Vector3 rayOffset; //f�r att flytta Ray upp�t s� att den hamnar r�tt med Fim
    [SerializeField] private LayerMask keycardMask;
    [SerializeField] public Transform orientationObject;
    private RaycastHit topRayHitInfo;

    public GameObject Keycard;
    private KeycardScript keycardScript;
    private GameManager gameManager;
    
    public static EquipKeycard Instance { get; private set; } //Singleton pattern
    public bool hasKeycard { get; private set; }

    private void Start()
    {
        hasKeycard = false;
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        
    }

    public void OnInteract(InputValue inputValue)
    {
        Equip();
    }

    private void RayCastHandler()
    {
        Ray topRay = new Ray(transform.position + rayOffset, orientationObject.forward);

        Debug.DrawRay(transform.position + rayOffset, orientationObject.forward * rayLengt, Color.green);

        Physics.Raycast(topRay, out topRayHitInfo, rayLengt, keycardMask); //F�r att kalla ut Rayen
    }

    private void Equip()
    {
        RayCastHandler();

        if (topRayHitInfo.collider != null && topRayHitInfo.collider.CompareTag("Keycard"))
        {
            keycardScript = topRayHitInfo.collider.GetComponent<KeycardScript>();
            Keycard = topRayHitInfo.collider.gameObject;
        }
        if(keycardScript != null)
        {
            keycardScript.Equip();
            hasKeycard = true;
            gameManager.PlayerFoundKeycard(); //KOPPLAD!
        }
        

        Debug.Log("isequipped");
    }

}
