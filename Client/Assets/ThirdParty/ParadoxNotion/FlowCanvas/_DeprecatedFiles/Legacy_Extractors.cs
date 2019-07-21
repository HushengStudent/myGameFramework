using UnityEngine;
using System;

namespace FlowCanvas.Nodes
{

    [Obsolete]
    public class ExtractVector2 : ExtractorNode<Vector2, float, float>
    {
        public override void Invoke(Vector2 vector, out float x, out float y) {
            x = vector.x;
            y = vector.y;
        }
    }

    [Obsolete]
    public class ExtractVector3 : ExtractorNode<Vector3, float, float, float>
    {
        public override void Invoke(Vector3 vector, out float x, out float y, out float z) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
    }

    [Obsolete]
    public class ExtractVector4 : ExtractorNode<Vector4, float, float, float, float>
    {
        public override void Invoke(Vector4 vector, out float x, out float y, out float z, out float w) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = vector.w;
        }
    }

    [Obsolete]
    public class ExtractQuaternion : ExtractorNode<Quaternion, float, float, float, float, Vector3>
    {
        public override void Invoke(Quaternion quaternion, out float x, out float y, out float z, out float w, out Vector3 eulerAngles) {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
            eulerAngles = quaternion.eulerAngles;
        }
    }

    [Obsolete]
    public class ExtractRect : ExtractorNode<Rect, Vector2, float, float, float, float>
    {
        public override void Invoke(Rect rect, out Vector2 center, out float xMin, out float xMax, out float yMin, out float yMax) {
            center = rect.center;
            xMin = rect.xMin;
            xMax = rect.xMax;
            yMin = rect.yMin;
            yMax = rect.yMax;
        }
    }

    [Obsolete]
    public class ExtractColor : ExtractorNode<Color, float, float, float, float>
    {
        public override void Invoke(Color color, out float r, out float g, out float b, out float a) {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }
    }

    [Obsolete]
    public class ExtractRaycastHit : ExtractorNode<RaycastHit, GameObject, float, Vector3, Vector3>
    {
        public override void Invoke(RaycastHit hit, out GameObject gameObject, out float distance, out Vector3 normal, out Vector3 point) {
            gameObject = hit.collider != null ? hit.collider.gameObject : null;
            distance = hit.distance;
            normal = hit.normal;
            point = hit.point;
        }
    }

    [Obsolete]
    public class ExtractRaycastHit2D : ExtractorNode<RaycastHit2D, GameObject, float, float, Vector3, Vector3>
    {
        public override void Invoke(RaycastHit2D hit, out GameObject gameObject, out float distance, out float fraction, out Vector3 normal, out Vector3 point) {
            gameObject = hit.collider != null ? hit.collider.gameObject : null;
            distance = hit.distance;
            fraction = hit.fraction;
            normal = hit.normal;
            point = hit.point;
        }
    }

    [Obsolete]
    public class ExtractRay : ExtractorNode<Ray, Vector3, Vector3>
    {
        public override void Invoke(Ray ray, out Vector3 origin, out Vector3 direction) {
            origin = ray.origin;
            direction = ray.direction;
        }
    }

    [Obsolete]
    public class ExtractBounds : ExtractorNode<Bounds, Vector3, Vector3, Vector3, Vector3, Vector3>
    {
        public override void Invoke(Bounds bounds, out Vector3 center, out Vector3 extents, out Vector3 max, out Vector3 min, out Vector3 size) {
            center = bounds.center;
            extents = bounds.extents;
            max = bounds.max;
            min = bounds.min;
            size = bounds.size;
        }
    }

    [Obsolete]
    public class ExtractCollision : ExtractorNode<Collision, ContactPoint[], ContactPoint, GameObject, Vector3>
    {
        public override void Invoke(Collision collision, out ContactPoint[] contacts, out ContactPoint firstContact, out GameObject gameObject, out Vector3 velocity) {
            contacts = collision.contacts;
            firstContact = collision.contacts[0];
            gameObject = collision.gameObject;
            velocity = collision.relativeVelocity;
        }
    }

    [Obsolete]
    public class ExtractCollision2D : ExtractorNode<Collision2D, ContactPoint2D[], ContactPoint2D, GameObject, Vector2>
    {
        public override void Invoke(Collision2D collision, out ContactPoint2D[] contacts, out ContactPoint2D firstContact, out GameObject gameObject, out Vector2 velocity) {
            contacts = collision.contacts;
            firstContact = collision.contacts[0];
            gameObject = collision.gameObject;
            velocity = collision.relativeVelocity;
        }
    }

    [Obsolete]
    public class ExtractContactPoint : ExtractorNode<ContactPoint, Vector3, Vector3, Collider, Collider>
    {
        public override void Invoke(ContactPoint contactPoint, out Vector3 normal, out Vector3 point, out Collider colliderA, out Collider colliderB) {
            normal = contactPoint.normal;
            point = contactPoint.point;
            colliderA = contactPoint.thisCollider;
            colliderB = contactPoint.otherCollider;
        }
    }

    [Obsolete]
    public class ExtractContactPoint2D : ExtractorNode<ContactPoint2D, Vector2, Vector2, Collider2D, Collider2D>
    {
        public override void Invoke(ContactPoint2D contactPoint, out Vector2 normal, out Vector2 point, out Collider2D colliderA, out Collider2D colliderB) {
            normal = contactPoint.normal;
            point = contactPoint.point;
            colliderA = contactPoint.collider;
            colliderB = contactPoint.otherCollider;
        }
    }

    [Obsolete]
    public class ExtractAnimationCurve : ExtractorNode<AnimationCurve, Keyframe[], float, WrapMode, WrapMode>
    {
        public override void Invoke(AnimationCurve curve, out Keyframe[] keys, out float length, out WrapMode postWrapMode, out WrapMode preWrapMode) {
            keys = curve.keys;
            length = curve.length;
            postWrapMode = curve.postWrapMode;
            preWrapMode = curve.preWrapMode;
        }
    }

    [Obsolete]
    public class ExtractKeyFrame : ExtractorNode<Keyframe, float, float, float, float>
    {
        public override void Invoke(Keyframe key, out float inTangent, out float outTangent, out float time, out float value) {
            inTangent = key.inTangent;
            outTangent = key.outTangent;
            time = key.time;
            value = key.value;
        }
    }
}