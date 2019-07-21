namespace FlowCanvas
{

    ///Implement this interface to get a callback when creating the editor menu.
    ///The OnMenu method should be wrapped in an #if UNITY_EDITOR
    public interface IEditorMenuCallbackReceiver
    {

#if UNITY_EDITOR
        void OnMenu(UnityEditor.GenericMenu menu, UnityEngine.Vector2 pos, Port sourcePort, object dropInstance);
#endif

    }

}