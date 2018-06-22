using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using UnityEngine;
using UnityEngine.EventSystems;


namespace NodeCanvas.Tasks.Conditions{

	[Category("UGUI")]
	[Description("Returns true when the selected event is triggered on the selected agent.\nYou can use this for both GUI and 3D objects.\nPlease make sure that Unity Event Systems are setup correctly")]
	public class InterceptEvent : ConditionTask<Transform> {

		public EventTriggerType eventType;

		protected override string info{
			get {return string.Format("{0} on {1}", eventType.ToString(), agentInfo );}
		}

		protected override string OnInit(){
			RegisterEvent("On" + eventType.ToString());
			return null;
		}

		protected override bool OnCheck(){
			return false;
		}

		void OnPointerEnter(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerExit(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerDown(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerUp(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerClick(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnDrag(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnDrop(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnScroll(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnUpdateSelected(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnSelect(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnDeselect(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnMove(AxisEventData eventData){
			YieldReturn(true);
		}

		void OnSubmit(BaseEventData eventData){
			YieldReturn(true);
		}
	}
}