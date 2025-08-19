using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// 处理UI元素上的鼠标输入事件。
    /// 需要挂载在可交互的UI元素上（例如，列表中的一个按钮或条目）。
    /// </summary>
    public class MouseInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // 对其所属的父菜单（实现了keyboardMenuInterface接口）的引用。
        public keyboardMenuInterface parentMenu;

        /// <summary>
        /// 当鼠标指针进入此UI元素的区域时调用。
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 如果父菜单存在，则通知它将当前选项设置为本UI元素。
            if (parentMenu != null)
            {
                parentMenu.setSelection(this.gameObject);
            }
        }

        /// <summary>
        /// 当鼠标指针离开此UI元素的区域时调用。
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 目前不需要特别处理，因为高亮选择是跟随鼠标实时变化的。
        }

        /// <summary>
        /// 当在此UI元素上发生点击时调用。
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 如果父菜单存在，则通知它执行确认操作。
            if (parentMenu != null)
            {
                parentMenu.doOption();
            }
        }
    }
}
