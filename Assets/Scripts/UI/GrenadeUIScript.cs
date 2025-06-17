using TMPro;
using UnityEngine;

// This script is for editing UI grenade numbers, the methodnames are self-explanatory
public class GrenadeUIScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI grenadeText;


    private void Start()
    {
    
    }

    public void setGrenades(int newAmmo)
    {
        if (grenadeText != null)
        {
            grenadeText.text = newAmmo.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void addGrenades(int grenadeToAdd)
    {
        if (grenadeText != null)
        {
            int currentGrenade = int.Parse(grenadeText.text);
            currentGrenade = currentGrenade + grenadeToAdd;
            setGrenades(currentGrenade);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void subtractGrenades(int GrenadeToSubtract)
    {
        if (grenadeText != null)
        {
            int currentGrenade = int.Parse(grenadeText.text);
            currentGrenade = currentGrenade - GrenadeToSubtract;
            setGrenades(currentGrenade);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }
}
