using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ParadoxNotion.Services{

	///A custom logger
	public static class Logger {

		///A message that logs
		public struct Message{
			public LogType type;
			public string text;
			public string tag;
			public object context;
			public bool IsSameAs(Message b){
				return this.type == b.type && this.text == b.text && this.tag == b.tag && this.context == b.context;
			}
			public bool IsValid(){
				return !string.IsNullOrEmpty(text);
			}
		}

		public delegate bool LogHandler(Message message);
		private static List<LogHandler> subscribers = new List<LogHandler>();

		#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		static void Init(){ mainThread = System.Threading.Thread.CurrentThread; }
		private static System.Threading.Thread mainThread;
		#endif

		///Subscribe a listener to the logger
		public static void AddListener(LogHandler callback){ subscribers.Add(callback); }
		///Remove a listener from the logger
		public static void RemoveListener(LogHandler callback){ subscribers.Remove(callback); }

		///Log
		public static void Log(object message, string tag = null, object context = null){
			Internal_Log(LogType.Log, message, tag, context);
		}

		///Log Warning
		public static void LogWarning(object message, string tag = null, object context = null){
			Internal_Log(LogType.Warning, message, tag, context);
		}

		///Log Error
		public static void LogError(object message, string tag = null, object context = null){
			Internal_Log(LogType.Error, message, tag, context);
		}

		///Log Exception
		public static void LogException(System.Exception exception, string tag = null, object context = null){
			Internal_Log(LogType.Exception, exception, tag, context);
		}

		//...
		private static void Internal_Log(LogType type, object message, string tag, object context){
			if (subscribers != null && subscribers.Count > 0){
				var msg = new Message();
				msg.type = type;
				if (message is System.Exception){
					var exc = (System.Exception)message;
					msg.text = exc.Message + "\n" + exc.StackTrace.Split('\n').FirstOrDefault();
				} else {
					msg.text = message != null? message.ToString() : "NULL";
				}
				msg.tag = tag;
				msg.context = context;
				var handled = false;
				foreach(var call in subscribers){
					if (call(msg)){
						handled = true;
						break;
					}
				}
				//if log is handled, don't forward to unity console
				if (handled && type != LogType.Exception){
					return;
				}
			}

			tag = string.Format("<b>({0} {1})</b>", tag, type.ToString());

			#if UNITY_EDITOR
			if (System.Threading.Thread.CurrentThread != mainThread){
				UnityEditor.EditorApplication.delayCall += ()=>{ ForwardToUnity(type, message, tag, context); };
				return;
			}
			#endif

			ForwardToUnity(type, message, tag, context);
		}
		
		//forward the log to unity console
		private static void ForwardToUnity(LogType type, object message, string tag, object context){
			#if UNITY_2017_1_OR_NEWER
			if (message is System.Exception){
				Debug.unityLogger.LogException( (System.Exception)message );
			} else {
				Debug.unityLogger.Log(type, tag, message, context as UnityEngine.Object);
			}
			#else
			if (message is System.Exception){
				Debug.logger.LogException( (System.Exception)message );
			} else {
				Debug.logger.Log(type, tag, message, context as UnityEngine.Object);
			}
			#endif
		}
	}
}