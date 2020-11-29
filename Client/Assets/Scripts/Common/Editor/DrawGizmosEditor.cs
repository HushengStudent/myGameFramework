using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawGizmos))]
public class DrawGizmosEditor : Editor
{
    private readonly float _size = 1f;
    private Vector3 _fromPosition = Vector3.zero;
    private Vector3 _toPosition = Vector3.zero;

    protected virtual void OnSceneGUI()
    {
        Handles.color = Color.red;
        _fromPosition = Handles.FreeMoveHandle(_fromPosition, Quaternion.identity
           , _size, Vector3.zero, Handles.SphereHandleCap);

        Handles.color = Color.yellow;
        _toPosition = Handles.FreeMoveHandle(_toPosition, Quaternion.identity
            , _size, Vector3.zero, Handles.SphereHandleCap);

        if (Event.current.type == EventType.Repaint)
        {
            var trans = ((DrawGizmos)target).transform;
            Handles.color = Color.red;
            Handles.SphereHandleCap(0, trans.position, trans.rotation * Quaternion.LookRotation(Vector3.right)
                , _size, EventType.Repaint);

            Handles.color = Color.green;
            Handles.DrawLine(_fromPosition, _toPosition);
        }
    }
}
