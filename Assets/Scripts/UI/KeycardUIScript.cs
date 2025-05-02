using UnityEngine;

public class KeycardUIScript : MonoBehaviour
{

    [SerializeField] GameObject backgroundRed;
    [SerializeField] GameObject backgroundGreen;


    void Start()
    {
        if (backgroundGreen != null && backgroundRed != null)
        {
            backgroundGreen.SetActive(false);
            backgroundRed.SetActive(true);
        } 
    }

   
    public void playerHasKeycard()
    {
        if (backgroundGreen != null && backgroundRed != null)
        {
            backgroundGreen.SetActive(true);
            backgroundRed.SetActive(false);
        }
    }

    public void playerDoNotHaveKeycard()
    {
        if (backgroundGreen != null && backgroundRed != null)
        {
            backgroundGreen.SetActive(false);
            backgroundRed.SetActive(true);
        }
    }

}
