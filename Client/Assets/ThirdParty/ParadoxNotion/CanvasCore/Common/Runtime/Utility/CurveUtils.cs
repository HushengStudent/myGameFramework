using UnityEngine;

namespace ParadoxNotion
{

    public static class CurveUtils
    {

        const float POS_CHECK_RES = 100f;
        const float POS_CHECK_DISTANCE = 10f;

        ///Get position on curve from, to, by t
        public static Vector2 GetPosAlongCurve(Vector2 from, Vector2 to, Vector2 fromTangent, Vector2 toTangent, float t) {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            Vector2 result = uuu * from;
            result += 3 * uu * t * ( from + fromTangent );
            result += 3 * u * tt * ( to + toTangent );
            result += ttt * to;
            return result;
        }

        ///Is target position along from, to curve
        public static bool IsPosAlongCurve(Vector2 from, Vector2 to, Vector2 fromTangent, Vector2 toTangent, Vector2 targetPosition) {
            float norm = 0;
            return IsPosAlongCurve(from, to, fromTangent, toTangent, targetPosition, out norm);
        }


        ///Is target position along from, to curve
        public static bool IsPosAlongCurve(Vector2 from, Vector2 to, Vector2 fromTangent, Vector2 toTangent, Vector2 targetPosition, out float norm) {
            if ( ParadoxNotion.RectUtils.GetBoundRect(from, to).ExpandBy(POS_CHECK_DISTANCE).Contains(targetPosition) ) {
                for ( var i = 0f; i <= POS_CHECK_RES; i++ ) {
                    var checkPos = GetPosAlongCurve(from, to, fromTangent, toTangent, i / POS_CHECK_RES);
                    if ( Vector2.Distance(targetPosition, checkPos) < POS_CHECK_DISTANCE ) {
                        norm = i / POS_CHECK_RES;
                        return true;
                    }
                }
            }
            norm = 0;
            return false;
        }

        //Resolve relevant tangency
        public static void ResolveTangents(Vector2 from, Vector2 to, float rigidMlt, PlanarDirection direction, out Vector2 fromTangent, out Vector2 toTangent) {
            var fromRect = new Rect(0, 0, 1, 1);
            var toRect = new Rect(0, 0, 1, 1);
            fromRect.center = from;
            toRect.center = to;
            ResolveTangents(from, to, fromRect, toRect, rigidMlt, direction, out fromTangent, out toTangent);
        }

        //Resolve relevant tangency
        public static void ResolveTangents(Vector2 from, Vector2 to, Rect fromRect, Rect toRect, float rigidMlt, PlanarDirection direction, out Vector2 fromTangent, out Vector2 toTangent) {
            var tangentX = Mathf.Abs(from.x - to.x) * rigidMlt;
            tangentX = Mathf.Max(tangentX, 25);

            var tangentY = Mathf.Abs(from.y - to.y) * rigidMlt;
            tangentY = Mathf.Max(tangentY, 25);

            switch ( direction ) {
                case ( PlanarDirection.Horizontal ): {
                        fromTangent = new Vector2(tangentX, 0);
                        toTangent = new Vector2(-tangentX, 0);
                    }
                    return;

                case ( PlanarDirection.Vertical ): {
                        fromTangent = new Vector2(0, tangentY);
                        toTangent = new Vector2(0, -tangentY);
                    }
                    return;

                case ( PlanarDirection.Auto ): {
                        var resultFrom = default(Vector2);
                        if ( from.x <= fromRect.xMin ) {
                            resultFrom = new Vector2(-tangentX, 0);
                        }

                        if ( from.x >= fromRect.xMax ) {
                            resultFrom = new Vector2(tangentX, 0);
                        }

                        if ( from.y <= fromRect.yMin ) {
                            resultFrom = new Vector2(0, -tangentY);
                        }

                        if ( from.y >= fromRect.yMax ) {
                            resultFrom = new Vector2(0, tangentY);
                        }

                        var resultTo = default(Vector2);
                        if ( to.x <= toRect.xMin ) {
                            resultTo = new Vector2(-tangentX, 0);
                        }

                        if ( to.x >= toRect.xMax ) {
                            resultTo = new Vector2(tangentX, 0);
                        }

                        if ( to.y <= toRect.yMin ) {
                            resultTo = new Vector2(0, -tangentY);
                        }

                        if ( to.y >= toRect.yMax ) {
                            resultTo = new Vector2(0, tangentY);
                        }

                        fromTangent = resultFrom;
                        toTangent = resultTo;
                    }
                    return;
            }

            fromTangent = default(Vector2);
            toTangent = default(Vector2);
        }


    }
}