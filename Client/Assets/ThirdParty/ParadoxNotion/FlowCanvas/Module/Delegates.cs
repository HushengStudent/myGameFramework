namespace FlowCanvas
{

    ///Delegate for Flow
    public delegate void FlowHandler(Flow f);
	///Delegate for Values
	[ParadoxNotion.Design.SpoofAOT]
	public delegate T ValueHandler<T>();
	///Delegate for object casted Values only
	public delegate object ValueHandlerObject();
	///Delegate for Flow Loop Break
	public delegate void FlowBreak();
	///Delegate for Flow Function Return
	public delegate void FlowReturn(object value);	
}