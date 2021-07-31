/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/27 00:52:05
** desc:  超级文本;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class SuperText : Text, IPointerClickHandler
    {
        private string _outputText;

        private static readonly Regex _imageRegex = new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />", RegexOptions.Singleline);
        private static readonly Regex _hypertextRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

        private readonly List<int> _imageVertexIndexList = new List<int>();
        private readonly List<HypertextInfo> _hypertextInfoList = new List<HypertextInfo>();

        protected readonly List<Image> _imageList = new List<Image>();
        protected static readonly StringBuilder _textBuilder = new StringBuilder();

        [Serializable]
        public class HypertextClickEvent : UnityEvent<string> { }

        [SerializeField]
        private HypertextClickEvent _onHypertextClick = new HypertextClickEvent();

        public HypertextClickEvent OnHypertextClick
        {
            get { return _onHypertextClick; }
            set { _onHypertextClick = value; }
        }

        public static Func<string, Sprite> LoadSpriteFunc;

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            UpdateImage();
        }

        protected void UpdateImage()
        {
            _outputText = GetOutputText(text);
            _imageVertexIndexList.Clear();
            foreach (Match match in _imageRegex.Matches(_outputText))
            {
                var imageIndex = match.Index;
                var endIndex = imageIndex * 4 + 3;
                _imageVertexIndexList.Add(endIndex);

                _imageList.RemoveAll(tempImage => tempImage == null);
                if (_imageList.Count == 0)
                {
                    GetComponentsInChildren(_imageList);
                }
                if (_imageVertexIndexList.Count > _imageList.Count)
                {
                    var resources = new DefaultControls.Resources();
                    var go = DefaultControls.CreateImage(resources);
                    go.layer = gameObject.layer;
                    var rectTransform = go.transform as RectTransform;
                    if (rectTransform)
                    {
                        rectTransform.SetParent(base.rectTransform);
                        rectTransform.localPosition = Vector3.zero;
                        rectTransform.localRotation = Quaternion.identity;
                        rectTransform.localScale = Vector3.one;
                    }
                    _imageList.Add(go.GetComponent<Image>());
                }

                var spriteName = match.Groups[1].Value;
                var size = float.Parse(match.Groups[2].Value);
                var image = _imageList[_imageVertexIndexList.Count - 1];
                if (image.sprite == null || image.sprite.name != spriteName)
                {
                    image.sprite = LoadSpriteFunc != null ? LoadSpriteFunc(spriteName) :
                        Resources.Load<Sprite>(spriteName);
                }
                image.rectTransform.sizeDelta = new Vector2(size, size);
                image.enabled = true;
            }

            for (var i = _imageVertexIndexList.Count; i < _imageList.Count; i++)
            {
                if (_imageList[i])
                {
                    _imageList[i].enabled = false;
                }
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            var orignText = m_Text;
            m_Text = _outputText;
            base.OnPopulateMesh(toFill);
            m_Text = orignText;

            var vert = new UIVertex();
            for (var i = 0; i < _imageVertexIndexList.Count; i++)
            {
                var endIndex = _imageVertexIndexList[i];
                var rectTransform = _imageList[i].rectTransform;
                var sizeDelta = rectTransform.sizeDelta;
                if (endIndex < toFill.currentVertCount)
                {
                    toFill.PopulateUIVertex(ref vert, endIndex);
                    rectTransform.anchoredPosition = new Vector2(vert.position.x + sizeDelta.x / 2, vert.position.y + sizeDelta.y / 2);
                    toFill.PopulateUIVertex(ref vert, endIndex - 3);

                    var position = vert.position;
                    for (int j = endIndex, m = endIndex - 3; j > m; j--)
                    {
                        toFill.PopulateUIVertex(ref vert, endIndex);
                        vert.position = position;
                        toFill.SetUIVertex(vert, j);
                    }
                }
            }

            if (_imageVertexIndexList.Count != 0)
            {
                _imageVertexIndexList.Clear();
            }

            foreach (var hypertextInfo in _hypertextInfoList)
            {
                hypertextInfo.BoxList.Clear();
                if (hypertextInfo.StartIndex >= toFill.currentVertCount)
                {
                    continue;
                }

                toFill.PopulateUIVertex(ref vert, hypertextInfo.StartIndex);
                var position = vert.position;
                var bounds = new Bounds(position, Vector3.zero);
                for (int i = hypertextInfo.StartIndex, m = hypertextInfo.EndIndex; i < m; i++)
                {
                    if (i >= toFill.currentVertCount)
                    {
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert, i);
                    position = vert.position;
                    if (position.x < bounds.min.x)
                    {
                        hypertextInfo.BoxList.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(position, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(position);
                    }
                }
                hypertextInfo.BoxList.Add(new Rect(bounds.min, bounds.size));
            }
        }

        protected virtual string GetOutputText(string outputText)
        {
            _textBuilder.Length = 0;
            _hypertextInfoList.Clear();
            var indexText = 0;
            foreach (Match match in _hypertextRegex.Matches(outputText))
            {
                _textBuilder.Append(outputText.Substring(indexText, match.Index - indexText));
                _textBuilder.Append("<color=green>");

                var group = match.Groups[1];
                var hypertextInfo = new HypertextInfo
                {
                    StartIndex = _textBuilder.Length * 4,
                    EndIndex = (_textBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
                    Name = group.Value
                };
                _hypertextInfoList.Add(hypertextInfo);

                _textBuilder.Append(match.Groups[2].Value);
                _textBuilder.Append("</color>");
                indexText = match.Index + match.Length;
            }
            _textBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
            return _textBuilder.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPosition);

            foreach (var hypertextInfo in _hypertextInfoList)
            {
                var boxes = hypertextInfo.BoxList;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(localPosition))
                    {
                        _onHypertextClick?.Invoke(hypertextInfo.Name);
                        return;
                    }
                }
            }
        }

        private class HypertextInfo
        {
            public int StartIndex;
            public int EndIndex;
            public string Name;
            public readonly List<Rect> BoxList = new List<Rect>();
        }
    }
}