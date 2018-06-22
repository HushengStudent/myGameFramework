using UnityEngine;


namespace NodeCanvas.DialogueTrees {

    /// <summary>
    /// An interface to use for DialogueActors within a DialogueTree.
    /// </summary>
	public interface IDialogueActor{
		string name {get;}
		Texture2D portrait {get;}
		Sprite portraitSprite {get;}
		Color dialogueColor {get;}
		Vector3 dialoguePosition {get;}
		Transform transform {get;}
	}

	

    ///A basic rather limited implementation of IDialogueActor
    [System.Serializable]
	public class ProxyDialogueActor : IDialogueActor {
		
		private string _name;
		private Transform _transform;
		
		public string name{
			get {return _name;}
		}

		public Texture2D portrait{
			get {return null;}
		}

		public Sprite portraitSprite{
			get {return null;}
		}

		public Color dialogueColor{
			get {return Color.white;}
		}

		public Vector3 dialoguePosition{
			get{return Vector3.zero;}
		}

		public Transform transform{
			get {return _transform;}
		}

		public ProxyDialogueActor(string name, Transform transform){
			this._name = name;
			this._transform = transform;
		}
	}
}