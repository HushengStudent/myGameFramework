/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/26 23:05:19
** desc:  UGUI扩展;
*********************************************************************************/

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

        public void OnPointerClick(PointerEventData eventData) { onClick?.Invoke(eventData, gameObject); }
        public void OnPointerDown(PointerEventData eventData) { onDown?.Invoke(eventData, gameObject); }
        public void OnPointerEnter(PointerEventData eventData) { onEnter?.Invoke(eventData, gameObject); }
        public void OnPointerExit(PointerEventData eventData) { onExit?.Invoke(eventData, gameObject); }
        public void OnPointerUp(PointerEventData eventData) { onUp?.Invoke(eventData, gameObject); }
        public void OnSelect(BaseEventData eventData) { onSelect?.Invoke(eventData, gameObject); }
        public void OnUpdateSelected(BaseEventData eventData) { onUpdateSelect?.Invoke(eventData, gameObject); }
        public void OnDeselect(BaseEventData eventData) { onDeSelect?.Invoke(eventData, gameObject); }
        public void OnDrag(PointerEventData eventData) { onDrag?.Invoke(eventData, gameObject); }
        public void OnEndDrag(PointerEventData eventData) { onDragEnd?.Invoke(eventData, gameObject); }
        public void OnDrop(PointerEventData eventData) { onDrop?.Invoke(eventData, gameObject); }
        public void OnScroll(PointerEventData eventData) { onScroll?.Invoke(eventData, gameObject); }
        public void OnMove(AxisEventData eventData) { onMove?.Invoke(eventData, gameObject); }

        static public UIEventListener Get(GameObject go)
        {
            var listener = go.GetComponent<UIEventListener>();
            if (listener == null)
            {
                listener = go.AddComponent<UIEventListener>();
            }
            return listener;
        }

        private void OnDestroy()
        {
            onClick = null;
            onDown = null;
            onEnter = null;
            onExit = null;
            onUp = null;
            onSelect = null;
            onUpdateSelect = null;
            onDeSelect = null;
            onDrag = null;
            onDragEnd = null;
            onDrop = null;
            onScroll = null;
            onMove = null;
        }
    }
}
