using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCanvas.Nodes{

	public struct ReflectedMethodRegistrationOptions{
		public bool callable;
		public bool exposeParams;
		public int exposedParamsCount;
	}
}