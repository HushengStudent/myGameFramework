using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

namespace ParadoxNotion.Services{

	///Simple Thread helper for both runtime and editor
	public static class Threader{

		public static Thread StartAction(Action function, Action callback = null){
			var thread = new Thread( new ThreadStart(function) );
			Begin( thread, callback );
			return thread;
		}

		public static Thread StartAction<T1>(Action<T1> function, T1 parameter1, Action callback = null){
			var thread = new Thread( ()=>
			{
				function(parameter1);
			});
			Begin( thread, callback );
			return thread;
		}	

		public static Thread StartAction<T1, T2>(Action<T1, T2> function, T1 parameter1, T2 parameter2, Action callback = null){
			var thread = new Thread( ()=>
			{
				function(parameter1, parameter2);
			});
			Begin( thread, callback );
			return thread;
		}	

		public static Thread StartAction<T1, T2, T3>(Action<T1, T2, T3> function, T1 parameter1, T2 parameter2, T3 parameter3, Action callback = null){
			var thread = new Thread( ()=>
			{
				function(parameter1, parameter2, parameter3);
			});
			Begin( thread, callback );
			return thread;
		}	

		///----------------------------------------------------------------------------------------------

		public static Thread StartFunction<TResult>(Func<TResult> function, Action<TResult> callback = null){
			TResult result = default(TResult);
			var thread = new Thread( ()=>
			{
				result = function();
			});
			Begin( thread, ()=>{ callback(result); } );
			return thread;
		}	

		public static Thread StartFunction<TResult, T1>(Func<T1, TResult> function, T1 parameter1, Action<TResult> callback = null){
			TResult result = default(TResult);
			var thread = new Thread( ()=>
			{
				result = function(parameter1);
			});
			Begin( thread, ()=>{ callback(result); } );
			return thread;
		}

		public static Thread StartFunction<TResult, T1, T2>(Func<T1, T2, TResult> function, T1 parameter1, T2 parameter2, Action<TResult> callback = null){
			TResult result = default(TResult);
			var thread = new Thread( ()=>
			{
				result = function(parameter1, parameter2);
			});
			Begin( thread, ()=>{ callback(result); } );
			return thread;
		}

		public static Thread StartFunction<TResult, T1, T2, T3>(Func<T1, T2, T3, TResult> function, T1 parameter1, T2 parameter2, T3 parameter3, Action<TResult> callback = null){
			TResult result = default(TResult);
			var thread = new Thread( ()=>
			{
				result = function(parameter1, parameter2, parameter3);
			});
			Begin( thread, ()=>{ callback(result); } );
			return thread;
		}

		///----------------------------------------------------------------------------------------------

		//This intermediate method exists to seperate editor and runtime usage.
		static void Begin(Thread thread, Action callback){
			
			thread.Start();

			#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode){
				threadUpdaters.Add( ThreadUpdater(thread, callback) );
				return;
			}
			#endif

			MonoManager.current.StartCoroutine( ThreadUpdater(thread, callback) );
		}

		///----------------------------------------------------------------------------------------------

#if UNITY_EDITOR
		private static List<IEnumerator> threadUpdaters = new List<IEnumerator>();
		[UnityEditor.InitializeOnLoadMethod]
		static void Initialize(){
			UnityEditor.EditorApplication.update += OnEditorUpdate;
		}

		//So that threads work in Editor too
		static void OnEditorUpdate(){
			if (threadUpdaters.Count > 0){
				for (var i = 0; i < threadUpdaters.Count; i++){
					var e = threadUpdaters[i];
					if (!e.MoveNext()){
						threadUpdaters.RemoveAt(i);
					}
				}
			}
		}
#endif


		///----------------------------------------------------------------------------------------------

		//Use IEnumerators and unity coroutines to handle updating the thread.
		private static IEnumerator ThreadUpdater(Thread thread, Action callback){

			while (thread.IsAlive){
				yield return null;
			}

			//This yield is not required.
			//It's for consistency matter when writing code so that we know there will always be a mmin 1 frame delay.
			yield return null;

			if ( (thread.ThreadState & ThreadState.AbortRequested) != ThreadState.AbortRequested ){
				// thread.Join();
				if (callback != null){
					callback();
				}
			}
		}
	}

}
