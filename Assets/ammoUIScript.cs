using TMPro;
using UnityEngine;

public class ammoUIScript : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI ammoString;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    void Update()
    {
        ammoString.text = "9";
    }
}
