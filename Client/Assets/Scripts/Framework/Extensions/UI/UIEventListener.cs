/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/26 23:05:19
** desc:  UGUI扩展;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public class UIEventListener : MonoBehaviour,
                                IPointerClickHandler,
                                IPointerDownHandler,
                                IPointerEnterHandler,
                                IPointerExitHandler,
                                IPointerUpHandler,
                                ISelectHandler,
                                IUpdateSelectedHandler,
                                IDeselectHandler,
                                IDragHandler,
                                IEndDragHandler,
                                IDropHandler,
                                IScrollHandler,
                                IMoveHandler
    {
        public delegate void UIEventDelegate(BaseEventData data, GameObject go);

        public UIEventDelegate onClick;
        public UIEventDelegate onDown;
        public UIEventDelegate onEnter;
        public UIEventDelegate onExit;
        public UIEventDelegate onUp;
        public UIEventDelegate onSelect;
        public UIEventDelegate onUpdateSelect;
        public UIEventDelegate onDeSelect;
        public UIEventDelegate onDrag;
        public UIEventDelegate onDragEnd;
        public UIEventDelegate onDrop;
        public UIEventDelegate onScroll;
        public UIEventDelegate onMove;

        public void OnPointerClick(PointerEventData eventData) { if (onClick != null) onClick(eventData, gameObject); }
        public void OnPointerDown(PointerEventData eventData) { if (onDown != null) onDown(eventData, gameObject); }
        public void OnPointerEnter(PointerEventData eventData) { if (onEnter != null) onEnter(eventData, gameObject); }
        public void OnPointerExit(PointerEventData eventData) { if (onExit != null) onExit(eventData, gameObject); }
        public void OnPointerUp(PointerEventData eventData) { if (onUp != null) onUp(eventData, gameObject); }
        public void OnSelect(BaseEventData eventData) { if (onSelect != null) onSelect(eventData, gameObject); }
        public void OnUpdateSelected(BaseEventData eventData) { if (onUpdateSelect != null) onUpdateSelect(eventData, gameObject); }
        public void OnDeselect(BaseEventData eventData) { if (onDeSelect != null) onDeSelect(eventData, gameObject); }
        public void OnDrag(PointerEventData eventData) { if (onDrag != null) onDrag(eventData, gameObject); }
        public void OnEndDrag(PointerEventData eventData) { if (onDragEnd != null) onDragEnd(eventData, gameObject); }
        public void OnDrop(PointerEventData eventData) { if (onDrop != null) onDrop(eventData, gameObject); }
        public void OnScroll(PointerEventData eventData) { if (onScroll != null) onScroll(eventData, gameObject); }
        public void OnMove(AxisEventData eventData) { if (onMove != null) onMove(eventData, gameObject); }

        static public UIEventListener Get(GameObject go)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null) listener = go.AddComponent<UIEventListener>();
            return listener;
        }
    }
}
