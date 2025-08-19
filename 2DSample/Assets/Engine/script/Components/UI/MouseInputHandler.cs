using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class MouseInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public keyboardMenuInterface parentMenu;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (parentMenu != null)
            {
                parentMenu.setSelection(this.gameObject);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Not needed for now, as selection follows the mouse.
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (parentMenu != null)
            {
                parentMenu.doOption();
            }
        }
    }
}
