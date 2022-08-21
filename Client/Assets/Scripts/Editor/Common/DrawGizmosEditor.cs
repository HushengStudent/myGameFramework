using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawGizmos))]
public class DrawGizmosEditor : Editor
{
    private readonly float _size = 1f;
    private Vector3 _from = Vector3.zero;
    private Vector3 _to = Vector3.zero;

    protected virtual void OnSceneGUI()
    {
        Handles.color = Color.red;
        _from = Handles.FreeMoveHandle(_from, Quaternion.identity, _size, Vector3.zero, Handles.SphereHandleCap);

        Handles.color = Color.yellow;
        _to = Handles.FreeMoveHandle(_to, Quaternion.identity, _size, Vector3.zero, Handles.SphereHandleCap);

        if (Event.current.type == EventType.Repaint)
        {
            var trans = ((DrawGizmos)target).transform;
            Handles.color = Color.red;
            Handles.SphereHandleCap(0, trans.position, trans.rotation * Quaternion.LookRotation(Vector3.right), _size, EventType.Repaint);

            Handles.color = Color.green;
            Handles.DrawLine(_from, _to);
        }
    }
}
