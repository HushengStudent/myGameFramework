using UnityEngine;
using System.Collections;
using System;

namespace NodeCanvas.Framework{

	//An attribute to help with URLs in the welcome window. Thats all
	[AttributeUsage(AttributeTargets.Class)]
	public class GraphInfoAttribute : Attribute{
		public string packageName;
		public string docsURL;
		public string resourcesURL;
		public string forumsURL;

	}
}