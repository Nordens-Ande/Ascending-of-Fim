using TMPro;
using UnityEngine;

public class Ammo : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI ammoText;


    // This class is only for the visual aspect of the ammo
    private void Start() // Setting ammo to 0 at the start
    {
        if (ammoText != null)
        {
            ammoText.text = "0";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void setAmmo(int newAmmo) // Method to set ammo amount to x amount
    {
        if (ammoText != null)
        {
            ammoText.text = newAmmo.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void addAmmo(int ammoToAdd) // Method to add x amount to ammo
    {
        if(ammoText != null)
        {
            int currentAmmo = int.Parse(ammoText.text);
            currentAmmo = currentAmmo + ammoToAdd;
            setAmmo(currentAmmo);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void subtractAmmo(int ammoToSubtract) // Method to subtract from current ammo
    {
        if (ammoText != null)
        {
            int currentAmmo = int.Parse(ammoText.text);
            currentAmmo = currentAmmo - ammoToSubtract;
            setAmmo(currentAmmo);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }
}
