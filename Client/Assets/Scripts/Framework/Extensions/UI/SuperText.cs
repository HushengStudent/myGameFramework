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

        //图片正则;
        private static readonly Regex _imageRegex = new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />", RegexOptions.Singleline);
        //超链正则;
        private static readonly Regex _hypertextRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
        //图片信息;
        private readonly List<ImageInfo> _imageInfoList = new List<ImageInfo>();
        //超链信息;
        private readonly List<HypertextInfo> _hypertextInfoList = new List<HypertextInfo>();

        protected readonly List<Image> _imageList = new List<Image>();

        protected static readonly StringBuilder _textBuilder = new StringBuilder();
        protected static readonly StringBuilder _tempTextBuilder = new StringBuilder();

        [Serializable]
        public class HypertextClickEvent : UnityEvent<string> { }
        [Serializable]
        public class ImageClickEvent : UnityEvent<string> { }

        [SerializeField]
        private HypertextClickEvent _onHypertextClick = new HypertextClickEvent();
        [SerializeField]
        private ImageClickEvent _onImageClick = new ImageClickEvent();

        public HypertextClickEvent OnHypertextClick
        {
            get { return _onHypertextClick; }
            set { _onHypertextClick = value; }
        }

        public ImageClickEvent OnImageClick
        {
            get { return _onImageClick; }
            set { _onImageClick = value; }
        }

        public static Func<string, Sprite> LoadSpriteFunc;

        protected override void Awake()
        {
            OnHypertextClick.AddListener((name) =>
            {
                Debug.LogError(name);
            });
            OnImageClick.AddListener((spriteName) =>
            {
                Debug.LogError(spriteName);
            });
        }

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            _outputText = GetOutputText(text);
            UpdateHyper();
            UpdateImage();
        }

        protected void UpdateImage()
        {
            var tempText = GetTextWithoutHyper(text);
            ProcessTextImage(tempText);

            _imageList.RemoveAll(image => image == null);
            if (_imageList.Count == 0)
            {
                GetComponentsInChildren(_imageList);
            }

            foreach(var image in _imageList)
            {
                image.enabled = false;
                image.raycastTarget = true;
            }

            while (_imageInfoList.Count > _imageList.Count)
            {
                var resources = new DefaultControls.Resources();
                var go = DefaultControls.CreateImage(resources);
                go.layer = gameObject.layer;
                var rt = go.transform as RectTransform;
                if (rt)
                {
                    rt.SetParent(rectTransform);
                    rt.localPosition = Vector3.zero;
                    rt.localRotation = Quaternion.identity;
                    rt.localScale = Vector3.one;
                }
                var image = go.GetComponent<Image>();
                image.enabled = false;
                image.raycastTarget = true;
                _imageList.Add(image);
            }

            for (int i = 0; i < Math.Min(_imageList.Count, _imageInfoList.Count); i++)
            {
                var info = _imageInfoList[i];
                var image = _imageList[i];
                image.gameObject.AddOrGetComponent<UIEventListener>().onClick = (eventData, gameObject) =>
                {
                    _onImageClick?.Invoke(info.SpriteName);
                };
                if (image.sprite == null || image.sprite.name != info.SpriteName)
                {
                    image.sprite = LoadSpriteFunc != null ? LoadSpriteFunc(info.SpriteName) :
                        Resources.Load<Sprite>(info.SpriteName);
                }
                image.rectTransform.sizeDelta = new Vector2(info.Size * info.Width, info.Size);
                image.enabled = true;
            }
        }

        protected void UpdateHyper()
        {
            var tempText = GetTextWithoutImage(text);
            ProcessTextHyper(tempText);
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            var orignText = m_Text;
            m_Text = _outputText;
            base.OnPopulateMesh(toFill);
            m_Text = orignText;

            var vert = new UIVertex();

            for (var i = 0; i < _imageInfoList.Count; i++)
            {
                var imageInfo = _imageInfoList[i];
                var endIndex = imageInfo.EndIndex;
                var size = imageInfo.Size;
                var width = imageInfo.Width;

                var image = _imageList[i];
                var rectTransform = image.rectTransform;

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

            if (_imageInfoList.Count != 0)
            {
                _imageInfoList.Clear();
            }

            //超链包围盒;
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

        protected virtual string GetOutputText(string outputText, string colorName = "green")
        {
            _textBuilder.Length = 0;

            var indexText = 0;
            foreach (Match match in _hypertextRegex.Matches(outputText))
            {
                var str = outputText.Substring(indexText, match.Index - indexText);
                _textBuilder.Append(str);
                var value = match.Groups[2].Value;
                _textBuilder.Append($"<color={colorName}>");
                _textBuilder.Append(value);
                _textBuilder.Append("</color>");
                indexText = match.Index + match.Length;
            }
            _textBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
            return _textBuilder.ToString();
        }

        protected virtual string GetTextWithoutHyper(string outputText)
        {
            _tempTextBuilder.Length = 0;
            var indexText = 0;
            foreach (Match match in _hypertextRegex.Matches(outputText))
            {
                var str = outputText.Substring(indexText, match.Index - indexText);
                str = ProcessTextString(str);
                _tempTextBuilder.Append(str);

                var value = ProcessTextString(match.Groups[2].Value);

                _tempTextBuilder.Append(value);
                indexText = match.Index + match.Length;
            }
            _tempTextBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
            var result = _tempTextBuilder.ToString();
            return result;
        }

        protected virtual string ProcessTextHyper(string outputText)
        {
            _tempTextBuilder.Length = 0;
            _hypertextInfoList.Clear();
            var indexText = 0;
            foreach (Match match in _hypertextRegex.Matches(outputText))
            {
                var str = outputText.Substring(indexText, match.Index - indexText);
                str = ProcessTextString(str);
                _tempTextBuilder.Append(str);

                var name = match.Groups[1].Value;
                var value = ProcessTextString(match.Groups[2].Value);
                var hypertextInfo = new HypertextInfo
                {
                    StartIndex = _tempTextBuilder.Length * 4,
                    EndIndex = (_tempTextBuilder.Length + value.Length) * 4,
                    Name = name
                };
                _hypertextInfoList.Add(hypertextInfo);

                _tempTextBuilder.Append(value);
                indexText = match.Index + match.Length;
            }
            _tempTextBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
            var result = _tempTextBuilder.ToString();
            return result;
        }

        protected virtual string GetTextWithoutImage(string outputText)
        {
            _tempTextBuilder.Length = 0;
            var indexText = 0;
            foreach (Match match in _imageRegex.Matches(outputText))
            {
                var str = outputText.Substring(indexText, match.Index - indexText);
                str = ProcessTextString(str);
                _tempTextBuilder.Append(str);

                _tempTextBuilder.Append("烫");//占位;

                indexText = match.Index + match.Length;
            }
            var result = _tempTextBuilder.ToString();
            return result;
        }

        protected virtual string ProcessTextImage(string outputText)
        {
            _tempTextBuilder.Length = 0;
            _imageInfoList.Clear();
            var indexText = 0;
            foreach (Match match in _imageRegex.Matches(outputText))
            {
                var str = outputText.Substring(indexText, match.Index - indexText);
                str = ProcessTextString(str);
                _tempTextBuilder.Append(str);

                var imageIndex = _tempTextBuilder.Length * 4;
                var endIndex = imageIndex + 3;

                _tempTextBuilder.Append("烫");

                var spriteName = match.Groups[1].Value;
                var size = float.Parse(match.Groups[2].Value);
                var width = float.Parse(match.Groups[3].Value);

                var imageInfo = new ImageInfo
                {
                    ImageIndex = imageIndex,
                    EndIndex = endIndex,
                    SpriteName = spriteName,
                    Size = size,
                    Width = width
                };
                _imageInfoList.Add(imageInfo);
                indexText = match.Index + match.Length;
            }
            var result = _tempTextBuilder.ToString();
            return result;
        }

        private string ProcessTextString(string str)
        {
            return str.Replace("\n", "").Replace("\t", "");
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

        private class ImageInfo
        {
            public string SpriteName;
            public float Size;
            public float Width;
            public int ImageIndex;
            public int EndIndex;
        }
    }
}