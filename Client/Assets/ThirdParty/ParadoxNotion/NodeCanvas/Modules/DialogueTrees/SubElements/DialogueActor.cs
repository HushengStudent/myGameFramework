using UnityEngine;


namespace NodeCanvas.DialogueTrees{

	/// <summary>
	/// A DialogueActor Component.
	/// </summary>
    [AddComponentMenu("NodeCanvas/Dialogue Actor")]
    public class DialogueActor : MonoBehaviour, IDialogueActor {

    	[SerializeField]
    	protected string _name;
    	[SerializeField]
    	protected Texture2D _portrait;
    	[SerializeField]
    	protected Color _dialogueColor = Color.white;
    	[SerializeField]
    	protected Vector3 _dialogueOffset;

    	private Sprite _portraitSprite;

		new public string name{
			get {return _name;}
		}

		public Texture2D portrait{
			get {return _portrait;}
		}

		public Sprite portraitSprite{
			get
			{
				if (_portraitSprite == null && portrait != null)
					_portraitSprite = Sprite.Create(portrait, new Rect(0,0,portrait.width, portrait.height), new Vector2(0.5f, 0.5f));
				return _portraitSprite;
			}
		}

		public Color dialogueColor{
			get {return _dialogueColor;}
		}

		public Vector3 dialoguePosition{
			// get {return Vector3.Scale(transform.position + _dialogueOffset, transform.localScale);}
			get {return transform.TransformPoint(_dialogueOffset);}
		}

		//IDialogueActor.transform is implemented by inherited MonoBehaviour.transform


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		void Reset(){
			_name = gameObject.name;
		}
			
		void OnDrawGizmos(){
			Gizmos.DrawLine(transform.position, dialoguePosition);
		}

		#endif
	}
}
