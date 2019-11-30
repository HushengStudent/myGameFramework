/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:45:42
** desc:  UI复用;
*********************************************************************************/

using System.Collections.Generic;
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

    public class ScrollRectEx : MonoBehaviour
    {
        private int _count;
        private int _needCount;
        private RectTransform _rectTrans;
        private RectTransform _scrollTrans;
        private float _extents = 0;
        private Vector2 _size = new Vector2(10, 10);
        private Vector2 _center = new Vector2(-1, -1);
        private List<RectTransform> _items = new List<RectTransform>();

        public delegate void OnLoopItemUpdate(GameObject item, int index);
        public OnLoopItemUpdate onLoopItemUpdate;

        [Header("行/列数")]
        [SerializeField]
        private int _directionCount = 1;
        [Header("模板")]
        [SerializeField]
        private GameObject _prefab;
        [Header("ScrollRect")]
        private ScrollRect _scrollRect;
        [Header("模板大小")]
        [SerializeField]
        private Vector2 _itemSize = new Vector2(100, 100);
        [Header("模板间隔")]
        [SerializeField]
        private Vector2 _itemOffset = new Vector2(10, -10);
        [Header("滚动方向")]
        [SerializeField]
        private LoopDirection _direction = LoopDirection.Vertical;

        public bool InitScrollRect()
        {
            if (_prefab == null)
            {
                LogHelper.PrintError("[LoopScrollRect]prefab is empty!");
                return false;
            }
            if (_directionCount <= 0)
            {
                _directionCount = 1;
            }
            if (_items.Count > 0)
            {
                LogHelper.PrintError("[LoopScrollRect]Init repeated!");
            }
            if (_scrollRect == null)
            {
                LogHelper.PrintError("[LoopScrollRect]scrollRect is empty!");
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
                _needCount = (int)Mathf.Ceil(_size.x / _itemSize.x + 1) * _directionCount;
            }
            else if (_direction == LoopDirection.Vertical)
            {
                _needCount = (int)Mathf.Ceil(_size.y / _itemSize.y + 1) * _directionCount;
            }
            _scrollRect.onValueChanged.AddListener(delegate { UpdateLoop(false); });
            _count = 0;
            _items.Clear();
            return true;
        }

        public void SetScrollRectCount(int number)
        {
            _prefab.SetActive(false);
            int needNum = Mathf.Min(number, _needCount);
            int nowNum = _items.Count;
            _count = number;
            for (int i = nowNum; i < needNum; i++)
            {
                GameObject item = Instantiate(_prefab) as GameObject;
                item.SetActive(true);
                item.transform.SetParent(_prefab.transform.parent);
                item.transform.localScale = _prefab.transform.localScale;
                item.transform.localPosition = _prefab.transform.localPosition;
                RectTransform rect = item.transform.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1f);
                rect.anchorMax = new Vector2(0, 1f);
                rect.pivot = new Vector2(0, 1);
                _items.Add(rect);
            }
            int maxCount = Mathf.Min(_needCount, _items.Count);
            for (int i = maxCount - 1; i >= needNum; i--)
            {
                Destroy(_items[i].gameObject);
                _items.Remove(_items[i]);
            }
            InitLoop();
            UpdateLoop(true);
        }

        private void InitLoop()
        {
            int row = 1;
            int col = 1;
            Vector2 startPos = new Vector2(_itemOffset.x, _itemOffset.y);
            int count = _items.Count;
            if (_direction == LoopDirection.Horizontal)
            {
                row = _directionCount;
                col = (int)Mathf.Ceil((float)count / (float)row);
                _extents = (float)(col * _itemSize.x) * 0.5f;
            }
            else if (_direction == LoopDirection.Vertical)
            {
                col = _directionCount;
                row = (int)Mathf.Ceil((float)count / (float)col);
                _extents = (float)(row * _itemSize.y) * 0.5f;
            }
            for (int i = 0; i < count; i++)
            {
                RectTransform trans = _items[i];
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
                for (int i = 0; i < _items.Count; i++)
                {
                    RectTransform trans = _items[i];
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
                _rectTrans.sizeDelta = new Vector2(pos.x + _itemSize.x, _directionCount * _itemSize.y);
            }
            else
            {
                _rectTrans.sizeDelta = new Vector2(_directionCount * _itemSize.x, -pos.y + _itemSize.y);
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
                realIndex = x * _directionCount + y;
            }
            else
            {
                realIndex = x + _directionCount * y;
            }
            return realIndex;
        }

        void OnDestroy()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null && _items[i].gameObject != null)
                {
                    GameObject.Destroy(_items[i].gameObject);
                }
            }
            _items.Clear();
        }

        [ContextMenu("TestLoopScrollRect")]
        public void TestLoopScrollRect()
        {
            if (Application.isPlaying)
            {
                onLoopItemUpdate = (go, index) =>
                {
                    LogHelper.Print($"===>>>[curIndex]:{index}");
                };
                if (InitScrollRect())
                {
                    SetScrollRectCount(66);
                }
            }
        }
    }
}
