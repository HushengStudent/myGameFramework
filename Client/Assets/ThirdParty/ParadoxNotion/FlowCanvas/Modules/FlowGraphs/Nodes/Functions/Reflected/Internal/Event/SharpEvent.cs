using System.Reflection;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    //Encapsulation of EventInfo. This acts similar to how UnityEventBase does for Unity
    abstract public class SharpEvent
    {

        public object instance { get; private set; }
        public EventInfo eventInfo { get; private set; }

        ///Create SharpEvent<T> wrapper for target EventInfo
        public static SharpEvent Create(EventInfo eventInfo) {
            if ( eventInfo == null ) { return null; }
            var wrapper = (SharpEvent)typeof(SharpEvent<>).RTMakeGenericType(eventInfo.EventHandlerType).CreateObjectUninitialized();
            wrapper.eventInfo = eventInfo;
            return wrapper;
        }

        ///Set target instance of event
        public void SetInstance(object instance) {
            this.instance = instance;
        }

        ///Start listening to a reflected delegate event using this wrapper
        public void StartListening(ReflectedDelegateEvent reflectedEvent, ReflectedDelegateEvent.DelegateEventCallback callback) {
            if ( reflectedEvent == null || callback == null ) { return; }
            reflectedEvent.Add(callback);
            eventInfo.AddEventHandler(instance, reflectedEvent.AsDelegate());
        }

        ///Stop listening from a reflected delegate event using this wrapper
        public void StopListening(ReflectedDelegateEvent reflectedEvent, ReflectedDelegateEvent.DelegateEventCallback callback) {
            if ( reflectedEvent == null || callback == null ) { return; }
            reflectedEvent.Remove(callback);
            eventInfo.RemoveEventHandler(instance, reflectedEvent.AsDelegate());
        }
    }

    ///Typeof(T) is the event handler type
    public class SharpEvent<T> : SharpEvent
    {

    }

}