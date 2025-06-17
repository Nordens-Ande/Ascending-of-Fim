using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// This script is for the keycard indicator in the HUD, so it changes color when the player has picked up the keycard
public class KeycardUIScript : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Image backgroundRed;
    [SerializeField] UnityEngine.UI.Image backgroundGreen;


    void Start()
    {
        if (backgroundGreen != null && backgroundRed != null)
        {
            backgroundGreen.gameObject.SetActive(false);
            backgroundRed.gameObject.SetActive(true);
        } 
    }

   
    public void playerHasKeycard()
    {
        if (backgroundGreen != null && backgroundRed != null)
        {
            backgroundGreen.gameObject.SetActive(true);
            backgroundRed.gameObject.SetActive(false);

            Debug.LogWarning("Keycard found, UI updated to green background");
        }
    }

    public void playerDoNotHaveKeycard()
    {
        if (backgroundGreen != null && backgroundRed != null)
        {
            backgroundGreen.gameObject.SetActive(false);
            backgroundRed.gameObject.SetActive(true);
        }
    }

}
