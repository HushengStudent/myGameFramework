using System;


namespace NodeCanvas.Framework
{
    ///Marks the BBParameter possible to only pick values from a blackboard
    [AttributeUsage(AttributeTargets.Field)]
	public class BlackboardOnlyAttribute : Attribute{}

    ///Marks the BBParameter as input
    [AttributeUsage(AttributeTargets.Field)]
	public class InputParameterAttribute : Attribute{}

    ///Marks the BBParameter as output
    [AttributeUsage(AttributeTargets.Field)]
	public class OutputParameterAttribute : Attribute{}
}