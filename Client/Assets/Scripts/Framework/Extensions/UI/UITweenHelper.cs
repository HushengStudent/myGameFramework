/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/26 00:57:26
** desc:  Tween¶¯»­;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Framework
{
    public static class UITweenHelper
    {
        private static int _tweenId = 0;
        private static int TweenId
        {
            get
            {
                _tweenId++;
                return _tweenId;
            }
        }

        private static Dictionary<int, Tweener> _tweenerDict = new Dictionary<int, Tweener>();

        public static void TweenPlayBackwards(int tweenId)
        {
            if (_tweenerDict.TryGetValue(tweenId, out var tweener))
            {
                tweener.PlayBackwards();
            }
        }

        public static void KillTween(int tweenId)
        {
            if (_tweenerDict.TryGetValue(tweenId, out var tweener))
            {
                tweener.Kill();
                _tweenerDict.Remove(tweenId);
            }
        }

        public static int TweenLocalPosition(GameObject go, Vector3 from, Vector3 to, float duration
            , Action<GameObject> onComplete, Action<GameObject> onKill)
        {
            var localPosition = Vector3.zero;
            var tweener = DOTween.To(() => localPosition, x => localPosition = x, to, duration);
            tweener.onUpdate = () =>
            {
                go.transform.localPosition = localPosition;
            };
            tweener.onKill = () =>
            {
                onKill?.Invoke(go);
            };
            tweener.onComplete = () =>
            {
                onComplete?.Invoke(go);
            };
            tweener.PlayForward();
            var tweenId = TweenId;
            _tweenerDict[tweenId] = tweener;
            return tweenId;
        }

        public static int TweenLocalScale(GameObject go, Vector3 from, Vector3 to, float duration
            , Action<GameObject> onComplete, Action<GameObject> onKill)
        {
            var localScale = Vector3.zero;
            var tweener = DOTween.To(() => localScale, x => localScale = x, to, duration);
            tweener.onUpdate = () =>
            {
                go.transform.localScale = localScale;
            };
            tweener.onKill = () =>
            {
                onKill?.Invoke(go);
            };
            tweener.onComplete = () =>
            {
                onComplete?.Invoke(go);
            };
            tweener.PlayForward();
            var tweenId = TweenId;
            _tweenerDict[tweenId] = tweener;
            return tweenId;
        }

        public static int TweenLocalRotation(GameObject go, Vector3 from, Vector3 to, float duration
            , Action<GameObject> onComplete, Action<GameObject> onKill)
        {
            var localRotation = Vector3.zero;
            var tweener = DOTween.To(() => localRotation, x => localRotation = x, to, duration);
            tweener.onUpdate = () =>
            {
                go.transform.localRotation = Quaternion.Euler(localRotation);
            };
            tweener.onKill = () =>
            {
                onKill?.Invoke(go);
            };
            tweener.onComplete = () =>
            {
                onComplete?.Invoke(go);
            };
            tweener.PlayForward();
            var tweenId = TweenId;
            _tweenerDict[tweenId] = tweener;
            return tweenId;
        }
    }
}