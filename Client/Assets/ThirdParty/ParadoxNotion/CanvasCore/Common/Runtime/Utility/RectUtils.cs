﻿using UnityEngine;

namespace ParadoxNotion
{

    ///Some common rect utilities
    public static class RectUtils
    {

        //Get a rect that encapsulates all provided rects
        public static Rect GetBoundRect(params Rect[] rects) {
            var xMin = float.PositiveInfinity;
            var xMax = float.NegativeInfinity;
            var yMin = float.PositiveInfinity;
            var yMax = float.NegativeInfinity;

            for ( var i = 0; i < rects.Length; i++ ) {
                xMin = Mathf.Min(xMin, rects[i].xMin);
                xMax = Mathf.Max(xMax, rects[i].xMax);
                yMin = Mathf.Min(yMin, rects[i].yMin);
                yMax = Mathf.Max(yMax, rects[i].yMax);
            }

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        //Get a rect that encapsulates all provided positions
        public static Rect GetBoundRect(params Vector2[] positions) {
            var xMin = float.PositiveInfinity;
            var xMax = float.NegativeInfinity;
            var yMin = float.PositiveInfinity;
            var yMax = float.NegativeInfinity;

            for ( var i = 0; i < positions.Length; i++ ) {
                xMin = Mathf.Min(xMin, positions[i].x);
                xMax = Mathf.Max(xMax, positions[i].x);
                yMin = Mathf.Min(yMin, positions[i].y);
                yMax = Mathf.Max(yMax, positions[i].y);
            }

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        ///Rect a fully encapsulated b?
        public static bool Encapsulates(this Rect a, Rect b) {
            return a.x < b.x && a.xMax > b.xMax && a.y < b.y && a.yMax > b.yMax;
        }

        ///Expands rect by margin
        public static Rect ExpandBy(this Rect rect, float margin) {
            return rect.ExpandBy(margin, margin);
        }

        ///Expands rect by x-y margin
        public static Rect ExpandBy(this Rect rect, float xMargin, float yMargin) {
            return rect.ExpandBy(xMargin, yMargin, xMargin, yMargin);
        }

        ///Expands rect by x-y margin
        public static Rect ExpandBy(this Rect rect, float left, float top, float right, float bottom) {
            return Rect.MinMaxRect(rect.xMin - left, rect.yMin - top, rect.xMax + right, rect.yMax + bottom);
        }

        //Transforms rect from one container to another container rect
        public static Rect TransformSpace(this Rect rect, Rect oldContainer, Rect newContainer) {
            var result = new Rect();
            result.xMin = Mathf.Lerp(newContainer.xMin, newContainer.xMax, Mathf.InverseLerp(oldContainer.xMin, oldContainer.xMax, rect.xMin));
            result.xMax = Mathf.Lerp(newContainer.xMin, newContainer.xMax, Mathf.InverseLerp(oldContainer.xMin, oldContainer.xMax, rect.xMax));
            result.yMin = Mathf.Lerp(newContainer.yMin, newContainer.yMax, Mathf.InverseLerp(oldContainer.yMin, oldContainer.yMax, rect.yMin));
            result.yMax = Mathf.Lerp(newContainer.yMin, newContainer.yMax, Mathf.InverseLerp(oldContainer.yMin, oldContainer.yMax, rect.yMax));
            return result;
        }

        //Transforms vector from one container to another container rect
        public static Vector2 TransformSpace(this Vector2 vector, Rect oldContainer, Rect newContainer) {
            var result = new Vector2();
            result.x = Mathf.Lerp(newContainer.xMin, newContainer.xMax, Mathf.InverseLerp(oldContainer.xMin, oldContainer.xMax, vector.x));
            result.y = Mathf.Lerp(newContainer.yMin, newContainer.yMax, Mathf.InverseLerp(oldContainer.yMin, oldContainer.yMax, vector.y));
            return result;
        }
    }
}