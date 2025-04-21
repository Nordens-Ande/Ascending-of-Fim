using TMPro;
using UnityEngine;

public class Ammo : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI ammoText;
    
  
    public void setAmmo(int newAmmo)
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

    public void addAmmo(int ammoToAdd)
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

    public void subtractAmmo(int ammoToSubtract)
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
