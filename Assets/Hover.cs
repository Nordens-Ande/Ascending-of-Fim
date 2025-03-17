using UnityEngine;
using UnityEngine.UI;

public class Hover : MonoBehaviour
{

    private Color hoverColor = Color.red;
    private Color defaultColor;
    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        if(buttonImage != null)
        {
            defaultColor = buttonImage.color;
        }
    }

    private void OnMouseEnter()
    {
        if(buttonImage != null)
        {
            buttonImage.color = hoverColor;
        }
        
    }

    private void OnMouseLeave()
    {
        if (buttonImage != null)
        {
            buttonImage.color = defaultColor;
        }
    }
}
