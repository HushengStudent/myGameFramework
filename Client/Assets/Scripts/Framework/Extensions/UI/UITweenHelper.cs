/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/26 00:57:26
** desc:  Tween¶¯»­;
*********************************************************************************/

using System.Collections;
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
            Tweener tweener;
            if(_tweenerDict.TryGetValue(tweenId,out tweener))
            {
                tweener.PlayBackwards();
            }
        }

        public static void KillTween(int tweenId)
        {
            Tweener tweener;
            if (_tweenerDict.TryGetValue(tweenId, out tweener))
            {
                tweener.Kill();
                _tweenerDict.Remove(tweenId);
            }
        }

        public static int TweenLocalPosition(GameObject go, Vector3 from, Vector3 to, float duration
            , Action<GameObject> onComplete, Action<GameObject> onKill)
        {
            Vector3 localPosition = Vector3.zero;
            Tweener tweener = DOTween.To(() => localPosition, x => localPosition = x, to, duration);
            tweener.onUpdate = () =>
            {
                go.transform.localPosition = localPosition;
            };
            tweener.onKill = () =>
            {
                if (onKill != null)
                    onKill(go);
            };
            tweener.onComplete = () =>
            {
                if (onComplete != null)
                    onComplete(go);
            };
            tweener.PlayForward();
            int tweenId = TweenId;
            _tweenerDict[tweenId] = tweener;
            return tweenId;
        }

        public static int TweenLocalScale(GameObject go, Vector3 from, Vector3 to, float duration
            , Action<GameObject> onComplete, Action<GameObject> onKill)
        {
            Vector3 localScale = Vector3.zero;
            Tweener tweener = DOTween.To(() => localScale, x => localScale = x, to, duration);
            tweener.onUpdate = () =>
            {
                go.transform.localScale = localScale;
            };
            tweener.onKill = () =>
            {
                if (onKill != null)
                    onKill(go);
            };
            tweener.onComplete = () =>
            {
                if (onComplete != null)
                    onComplete(go);
            };
            tweener.PlayForward();
            int tweenId = TweenId;
            _tweenerDict[tweenId] = tweener;
            return tweenId;
        }

        public static int TweenLocalRotation(GameObject go, Vector3 from, Vector3 to, float duration
            , Action<GameObject> onComplete, Action<GameObject> onKill)
        {
            Vector3 localRotation = Vector3.zero;
            Tweener tweener = DOTween.To(() => localRotation, x => localRotation = x, to, duration);
            tweener.onUpdate = () =>
            {
                go.transform.localRotation = Quaternion.Euler(localRotation);
            };
            tweener.onKill = () =>
            {
                if (onKill != null)
                    onKill(go);
            };
            tweener.onComplete = () =>
            {
                if (onComplete != null)
                    onComplete(go);
            };
            tweener.PlayForward();
            int tweenId = TweenId;
            _tweenerDict[tweenId] = tweener;
            return tweenId;
        }
    }
}