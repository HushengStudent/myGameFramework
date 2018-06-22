using System;
using System.Collections.Generic;


namespace NodeCanvas.DialogueTrees{

	///Send along with a OnSubtitlesRequest event. Holds info about the actor speaking, the statement that being said as well as a callback to be called when dialogue is done showing
	public class SubtitlesRequestInfo{

		///The actor speaking
		public IDialogueActor actor;
		///The statement said
		public IStatement statement;
		///Call this to Continue the DialogueTree
		public Action Continue;

		public SubtitlesRequestInfo(IDialogueActor actor, IStatement statement, Action callback){
			this.actor = actor;
			this.statement = statement;
			this.Continue = callback;
		}
	}

	///Send along with a OnMultipleChoiceRequest event. Holds information of the options, time available as well as a callback to be called providing the selected option
	public class MultipleChoiceRequestInfo{

		///The actor related. This is usually the actor that will also say the options
		public IDialogueActor actor;
		///The available choice option. Key: The statement, Value: the child index of the option
		public Dictionary<IStatement, int> options;
		///The available time for a choice
		public float availableTime;
		///Should the previous statement be shown along the options?
		public bool showLastStatement;
		///Call this with to select the option to continue with in the DialogueTree
		public Action<int> SelectOption;

		public MultipleChoiceRequestInfo(IDialogueActor actor, Dictionary<IStatement, int> options, float availableTime, bool showLastStatement, Action<int> callback){
			this.actor = actor;
			this.options = options;
			this.availableTime = availableTime;
			this.showLastStatement = showLastStatement;
			this.SelectOption = callback;
		}

		public MultipleChoiceRequestInfo(IDialogueActor actor, Dictionary<IStatement, int> options, float availableTime, Action<int> callback){
			this.actor = actor;
			this.options = options;
			this.availableTime = availableTime;
			this.SelectOption = callback;
		}
	}
}