using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Category("Input")]
	public class CheckMousePick2D : ConditionTask{

		public ParadoxNotion.ButtonKeys buttonKey;
		public LayerMask mask = -1;

		[BlackboardOnly]
		public BBParameter<GameObject> saveGoAs;
		[BlackboardOnly]
		public BBParameter<float> saveDistanceAs;
		[BlackboardOnly]
		public BBParameter<Vector3> savePosAs;

		private int buttonID;
		private RaycastHit2D hit;

		protected override string info{
			get
			{
				var finalString= buttonKey.ToString() + " Click";
				if (!savePosAs.isNone)
					finalString += "\nSavePos As " + savePosAs;
				if (!saveGoAs.isNone)
					finalString += "\nSaveGo As " + saveGoAs;
				return finalString;
			}
		}

		protected override bool OnCheck(){

			buttonID = (int)buttonKey;
			if (Input.GetMouseButtonDown(buttonID)){
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, mask);
				if (hit.collider != null){
					savePosAs.value = hit.point;
					saveGoAs.value = hit.collider.gameObject;
					saveDistanceAs.value = hit.distance;
					return true;
				}
			}
			return false;
		}
	}
}