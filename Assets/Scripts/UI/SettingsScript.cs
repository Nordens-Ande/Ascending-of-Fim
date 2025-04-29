using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] Toggle toggleHUDTime;
    [SerializeField] Toggle toogleHUDScore;
    [SerializeField] Toggle toggleHUDAmmo;
    [SerializeField] Toggle toggleHUDMoney;
    [SerializeField] Toggle toggleHUDHealthpoints;
    [SerializeField] Toggle toggleHUDAnnouncement;

    [Space]

    [SerializeField] GameObject HUDTime;
    [SerializeField] GameObject HUDScore;
    [SerializeField] GameObject HUDAmmo;
    [SerializeField] GameObject HUDMoney;
    [SerializeField] GameObject HUDHealthpoints;
    [SerializeField] GameObject HUDAnnouncement;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
