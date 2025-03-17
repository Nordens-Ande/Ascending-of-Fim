using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Color originalColor;
    [SerializeField] public Color hoverColor;
   
    
    //h�mta den vanliga f�rgen p� knappen
    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            originalColor = button.image.color;
        }
    }

    //n�r musen g�r �ver knappen �ndra till hover f�rgen
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null)
        {
            button.image.color = hoverColor;
        }
    }

    //n�r musen l�mnar g� tillbaka till den vanliga f�gren
    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null)
        {
            button.image.color = originalColor;
        }
    }
}