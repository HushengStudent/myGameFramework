using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawGizmos))]
public class DrawGizmosEditor : Editor
{
    readonly float _size = 1f;

    Vector3 _positionFrom = Vector3.zero;

    Vector3 _positionTo = Vector3.zero;

    protected virtual void OnSceneGUI()
    {
        Handles.color = Color.red;
        _positionFrom = Handles.FreeMoveHandle(_positionFrom, Quaternion.identity
           , _size, Vector3.zero, Handles.SphereHandleCap);
        Handles.color = Color.yellow;
        _positionTo = Handles.FreeMoveHandle(_positionTo, Quaternion.identity
            , _size, Vector3.zero, Handles.SphereHandleCap);

        if (Event.current.type == EventType.Repaint)
        {
            var trans = ((DrawGizmos)target).transform;
            Handles.color = Color.red;
            Handles.SphereHandleCap(0, trans.position, trans.rotation * Quaternion.LookRotation(Vector3.right)
                , _size, EventType.Repaint);
            Handles.color = Color.green;
            Handles.DrawLine(_positionFrom, _positionTo);

        }
    }
}
