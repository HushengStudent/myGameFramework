/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:45:42
** desc:  UI复用;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public delegate void OnLoopItemUpdate(int index, GameObject go);

    public enum LoopDirection
    {
        Vertical = 0,
        Horizontal = 1
    }

    public class LoopScrollRect : MonoBehaviour
    {
        private int _count;
        private int _needCount;
        private RectTransform _rectTrans;
        private ScrollRect _scrollRect;
        private RectTransform _scrollTrans;
        private float _extents = 0;
        private Vector2 _size = new Vector2(10, 10);
        private Vector2 _center = new Vector2(-1, -1);
        private List<RectTransform> _elements = new List<RectTransform>();

        public delegate void OnLoopItemUpdate(GameObject item, int index);
        public OnLoopItemUpdate onLoopItemUpdate;

        [SerializeField]
        private int _fixedCount = 1;
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private Vector2 _itemSize = new Vector2(100, 100);
        [SerializeField]
        private Vector2 _itemOffset = new Vector2(10, -10);
        [SerializeField]
        private LoopDirection _direction = LoopDirection.Vertical;

        public bool InitScrollRect()
        {
            if (_prefab == null)
            {
                LogUtil.LogUtility.PrintError("[LoopScrollRect]prefab is empty!");
                return false;
            }
            if (_fixedCount <= 0)
            {
                _fixedCount = 1;
            }
            if (_elements.Count > 0)
            {
                LogUtil.LogUtility.PrintError("[LoopScrollRect]Init repeated!");
            }
            _scrollRect = transform.GetComponentInParent<ScrollRect>();
            if (_scrollRect == null)
            {
                LogUtil.LogUtility.PrintError("[LoopScrollRect]scrollRect is empty!");
                return false;
            }
            _scrollTrans = _scrollRect.transform.GetComponent<RectTransform>();
            _size = _scrollTrans.rect.size;
            //设置Mask;
            Mask mask = transform.GetComponentInParent<Mask>();
            if (mask != null)
            {
                RectTransform rect = mask.transform.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0, 1);
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.sizeDelta = _size;
                rect.anchoredPosition = new Vector3(0, 0, 0);
            }
            _rectTrans = transform.GetComponent<RectTransform>();
            _rectTrans.pivot = new Vector2(0, 1);
            _rectTrans.anchorMin = new Vector2(0f, 1f);
            _rectTrans.anchorMax = new Vector2(0f, 1f);
            _rectTrans.sizeDelta = _size;
            _rectTrans.anchoredPosition = new Vector3(0, 0, 0);
            if (_direction == LoopDirection.Horizontal)
            {
                _needCount = (int)Mathf.Ceil(_size.x / _itemSize.x + 1) * _fixedCount;
            }
            else if (_direction == LoopDirection.Vertical)
            {
                _needCount = (int)Mathf.Ceil(_size.y / _itemSize.y + 1) * _fixedCount;
            }
            _scrollRect.onValueChanged.AddListener(delegate { UpdateLoop(false); });
            _count = 0;
            _elements.Clear();
            return true;
        }

        public void SetScrollRectCount(int number)
        {
            _prefab.SetActive(false);
            int needNum = Mathf.Min(number, _needCount);
            int nowNum = _elements.Count;
            _count = number;
            for (int i = nowNum; i < needNum; i++)
            {
                GameObject item = GameObject.Instantiate(_prefab) as GameObject;
                item.SetActive(true);
                item.transform.SetParent(_prefab.transform.parent);
                item.transform.localScale = _prefab.transform.localScale;
                item.transform.localPosition = _prefab.transform.localPosition;
                RectTransform rect = item.transform.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1f);
                rect.anchorMax = new Vector2(0, 1f);
                rect.pivot = new Vector2(0, 1);
                _elements.Add(rect);
            }
            int maxCount = Mathf.Min(_needCount, _elements.Count);
            for (int i = maxCount - 1; i >= needNum; i--)
            {
                GameObject.DestroyImmediate(_elements[i].gameObject);
                _elements.Remove(_elements[i]);
            }
            InitLoop();
            UpdateLoop(true);
        }

        private void InitLoop()
        {
            int row = 1;
            int col = 1;
            Vector2 startPos = new Vector2(_itemOffset.x, _itemOffset.y);
            int count = _elements.Count;
            if (_direction == LoopDirection.Horizontal)
            {
                row = _fixedCount;
                col = (int)Mathf.Ceil((float)count / (float)row);
                _extents = (float)(col * _itemSize.x) * 0.5f;
            }
            else if (_direction == LoopDirection.Vertical)
            {
                col = _fixedCount;
                row = (int)Mathf.Ceil((float)count / (float)col);
                _extents = (float)(row * _itemSize.y) * 0.5f;
            }
            for (int i = 0; i < count; i++)
            {
                RectTransform trans = _elements[i];
                int x = 0, y = 0;
                if (_direction == LoopDirection.Vertical)
                {
                    x = i / col;
                    y = i % col;
                }
                else if (_direction == LoopDirection.Horizontal)
                {
                    x = i % row;
                    y = i / row;
                }
                trans.anchoredPosition = new Vector2(startPos.x + y * _itemSize.x, startPos.y - x * _itemSize.y);
                if (i <= _count)
                {
                    trans.gameObject.SetActive(true);
                    UpdateRect(trans.anchoredPosition);
                    UpdateItem(trans, i, i);
                }
                else
                {
                    trans.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateLoop(bool force)
        {
            if ((!force) || (Mathf.Abs(_center.x + 1) <= float.Epsilon && Mathf.Abs(_center.y + 1) <= float.Epsilon))
            {
                _center = -_rectTrans.anchoredPosition;
                _center.x = _center.x + _size.x / 2;
                _center.y = _center.y - _size.y / 2;
            }
            bool need2Update = true;
            while (need2Update)
            {
                need2Update = false;
                for (int i = 0; i < _elements.Count; i++)
                {
                    RectTransform trans = _elements[i];
                    Vector2 pos = trans.anchoredPosition;
                    float distance = 0;
                    if (_direction == LoopDirection.Horizontal)
                    {
                        distance = trans.anchoredPosition.x - _center.x;
                    }
                    else
                    {
                        distance = trans.anchoredPosition.y - _center.y;
                    }
                    bool toUpdate = false;
                    bool dirty = true;
                    if (distance < -_extents)
                    {
                        if (_direction == LoopDirection.Horizontal)
                        {
                            pos.x += _extents * 2f;
                            toUpdate = true;
                        }
                        else
                        {
                            pos.y += _extents * 2f;
                        }
                    }
                    else if (distance > _extents)
                    {
                        if (_direction == LoopDirection.Horizontal)
                        {
                            pos.x -= _extents * 2f;
                        }
                        else
                        {
                            pos.y -= _extents * 2f;
                            toUpdate = true;
                        }
                    }
                    else
                    {
                        dirty = false;
                    }
                    if (dirty)
                    {
                        int realIndex = GetIndex(pos);
                        if (realIndex < _count && realIndex >= 0)
                        {
                            if (toUpdate) UpdateRect(pos);
                            trans.anchoredPosition = pos;
                            need2Update = true;
                            UpdateItem(trans, i, realIndex);
                        }
                    }
                }
                if (!force)
                {
                    need2Update = false;
                }
            }
        }

        void UpdateRect(Vector2 pos)
        {
            if (_direction == LoopDirection.Horizontal)
            {
                _rectTrans.sizeDelta = new Vector2(pos.x + _itemSize.x, _fixedCount * _itemSize.y);
            }
            else
            {
                _rectTrans.sizeDelta = new Vector2(_fixedCount * _itemSize.x, -pos.y + _itemSize.y);
            }
        }

        void UpdateItem(Transform item, int index, int realIndex)
        {
            if (onLoopItemUpdate != null)
            {
                onLoopItemUpdate(item.gameObject, realIndex);
            }
        }

        int GetIndex(Vector2 pos)
        {
            int x = (int)Mathf.Ceil(-(pos.y - _itemOffset.y) / _itemSize.y);
            int y = (int)Mathf.Ceil((pos.x - _itemOffset.x) / _itemSize.x);
            int realIndex;
            if (_direction == LoopDirection.Vertical)
            {
                realIndex = x * _fixedCount + y;
            }
            else
            {
                realIndex = x + _fixedCount * y;
            }
            return realIndex;
        }

        void OnDestroy()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i] != null && _elements[i].gameObject != null)
                {
                    GameObject.DestroyImmediate(_elements[i].gameObject);
                }
            }
            _elements.Clear();
        }

        [ContextMenu("TestLoopScrollRect")]
        public void TestLoopScrollRect()
        {
            if (Application.isPlaying)
            {
                onLoopItemUpdate = (go, index) =>
                {
                    LogUtil.LogUtility.Print("===>>>[curIndex]:" + index);
                };
                if (InitScrollRect())
                {
                    SetScrollRectCount(66);
                }
            }
        }
    }
}
