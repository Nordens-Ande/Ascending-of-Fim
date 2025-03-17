using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

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

        //en lyssnare, som v�ntar p� n�r knappen blir tryckt p�
        button.onClick.AddListener(OnButtonClick);
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


    //f�r en bugg som l�mnade knappanar som "intryckta" n�r man bytte meny
    private void OnButtonClick()
    {
        // Revert to the original color when the button is clicked
        if (button != null)
        {
            button.image.color = originalColor;
        }
    }
}