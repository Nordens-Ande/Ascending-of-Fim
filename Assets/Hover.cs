using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Color originalColor;
    [SerializeField] public Color hoverColor;
   
    
    //hämta den vanliga färgen på knappen
    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            originalColor = button.image.color;
        }
    }

    //när musen går över knappen ändra till hover färgen
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null)
        {
            button.image.color = hoverColor;
        }
    }

    //när musen lämnar gå tillbaka till den vanliga fägren
    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null)
        {
            button.image.color = originalColor;
        }
    }
}