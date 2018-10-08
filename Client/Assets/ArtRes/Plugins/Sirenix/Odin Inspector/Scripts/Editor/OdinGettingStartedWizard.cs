namespace Sirenix.OdinInspector.Demos
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Serialization;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Serialization;

    [HideMonoScript]
    public class OdinGettingStartedWizard : ScriptableObject
    {
        [NonSerialized]
        public bool ShowEditor;

        [HideLabel]
        [PreviewField(75, OdinInspector.ObjectFieldAlignment.Left)]
        [HorizontalGroup("Split", 80)]
        [ShowIf("ShowEditor", false),]
        public Texture2D Icon;

        [VerticalGroup("Split/a")]
        [ShowIf("ShowEditor", false)]
        public string Title;

        [VerticalGroup("Split/a")]
        [ShowIf("ShowEditor", false)]
        public float BtnWidth = 100;

        [VerticalGroup("Split/a")]
        [ShowIf("ShowEditor", false)]
        public int SubTitleFontSize = 22;

        [VerticalGroup("Split/a")]
        [ShowIf("ShowEditor", false)]
        public int TitleFontSize = 22;

        [ShowIf("ShowEditor", false)]
        [HideReferenceObjectPicker]
        public List<Section> Examples = new List<Section>();

        [HideIf("ShowEditor")]
        [OnInspectorGUI, PropertyOrder(-10)]
        private void OnInspectorGUI()
        {
            var title = new GUIStyle(SirenixGUIStyles.SectionHeaderCentered);
            var subTitle = new GUIStyle(SirenixGUIStyles.SectionHeader);
            title.fontSize = this.TitleFontSize;
            subTitle.fontSize = this.SubTitleFontSize;

            GUIHelper.RequestRepaint();
            // Draw dark BG
            EditorGUI.DrawRect(GUIHelper.GetCurrentLayoutRect().Expand(5, 5, 4, 20), SirenixGUIStyles.DarkEditorBackground);

            // Draw title
            GUILayout.BeginVertical(new GUIStyle() { padding = new RectOffset(20, 20, 20, 20) });
            if (this.Title != null)
            {
                GUILayout.Label(new GUIContent(this.Title, this.Icon), title);
            }
            GUILayout.EndVertical();

            var padding = new GUIStyle() { padding = new RectOffset(20, 20, 20, 20) };
            foreach (var item in this.Examples)
            {
                if (item.VerticalResources) { GUILayout.BeginVertical(padding); } else { GUILayout.BeginHorizontal(padding); }
                {
                    // bg
                    EditorGUI.DrawRect(GUIHelper.GetCurrentLayoutRect().Padding(-10, -10, 5, 5), SirenixGUIStyles.BoxBackgroundColor);

                    // Draw texts
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(item.Title, subTitle);
                        GUILayout.Label(item.SubTitle, SirenixGUIStyles.MultiLineLabel);
                    }
                    GUILayout.EndVertical();

                    // Draw buttons
                    if (item.VerticalResources)
                    {
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginVertical(item.VerticalResources ? GUILayoutOptions.ExpandWidth() : GUILayoutOptions.Width(this.BtnWidth));
                    }
                    {
                        for (int i = 0; i < item.Resources.Count; i++)
                        {
                            if (i != 0 && item.VerticalResources)
                            {
                                GUILayout.FlexibleSpace();
                            }
                            item.Resources[i].Draw();
                        }
                    }
                    if (item.VerticalResources)
                    {
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.EndVertical();
                    }
                }
                if (item.VerticalResources) { GUILayout.EndVertical(); } else { GUILayout.EndHorizontal(); }
            }

            if (!(GUIHelper.CurrentWindow as OdinEditorWindow))
            {
                GUIHelper.PushColor(Color.green);
                if (GUILayout.Button("Open this in a new window", GUILayoutOptions.Height(30)))
                {
                    OdinEditorWindow.InspectObject(this).position = GUIHelper.GetEditorWindowRect().AlignCenter(570, 700);
                    Selection.objects = new UnityEngine.Object[0];
                }
                GUIHelper.PopColor();
            }

            GUILayout.FlexibleSpace();
        }

        [ShowIf("ShowEditor")]
        [Button(ButtonSizes.Large), PropertyOrder(-10)]
        private void OpenEditor()
        {
            OdinEditorWindow.InspectObject(this);
        }

        [Serializable]
        public class Section
        {
            [HideInInspector]
            public bool VerticalResources;

            [HideLabel]
            [VerticalGroup("Split/Text")]
            [SuffixLabel("Title", true)]
            public string Title;

            [HideLabel]
            [Multiline(4)]
            [VerticalGroup("Split/Text")]
            public string SubTitle;

            [HorizontalGroup("Split", 200)]
            [VerticalGroup("Split/Info")]
            [CustomValueDrawer("DrawResource")]
            [ListDrawerSettings(OnTitleBarGUI = "DrawListStyle")]
            public List<SectionResource> Resources = new List<SectionResource>();

            private SectionResource DrawResource(SectionResource value, GUIContent label)
            {
                if (GUILayout.Button(value.ButtonName))
                {
                    OdinEditorWindow.InspectObjectInDropDown(value, GUIHelper.GetCurrentLayoutRect(), 370);
                }

                return value;
            }

            private void DrawListStyle()
            {
                if (SirenixEditorGUI.ToolbarButton(this.VerticalResources ? EditorIcons.List : EditorIcons.Char1))
                {
                    this.VerticalResources = !this.VerticalResources;
                }
            }
        }

        [Serializable]
        public class SectionResource
        {
            [HideLabel]
            [EnumToggleButtons]
            public SectionResourceType Type;

            public string ButtonName;

            public bool HighlightButton = false;

            [AssetsOnly]
            [HideIf("Type", SectionResourceType.Url, false)]
            [HideIf("Type", SectionResourceType.OpenEditorWindow, false)]
            public UnityEngine.Object UnityObject;

            public string StringValue;

            [HideIf("Type", SectionResourceType.Url, false)]
            public Vector2 windowSize = new Vector2(700, 500);

            public void Draw()
            {
                bool disable =
                    (this.Type == SectionResourceType.OpenObject && this.UnityObject == null && !File.Exists(this.StringValue)) ||
                    (this.Type == SectionResourceType.Url && string.IsNullOrEmpty(this.StringValue) ||
                    (this.Type == SectionResourceType.OpenEditorWindow && (string.IsNullOrEmpty(this.StringValue) || (AssemblyUtilities.GetTypeByCachedFullName(this.StringValue) == null && !File.Exists(this.StringValue)))));

                GUIHelper.PushGUIEnabled(!disable);

                if (this.HighlightButton && !disable)
                {
                    GUIHelper.PushColor(SirenixGUIStyles.HighlightedButtonColor);
                }

                if (string.IsNullOrEmpty(this.ButtonName))
                {
                    GUILayout.Space(10);
                }
                else
                {
                    if (GUILayout.Button(new GUIContent(" " + ButtonName + " "), GUILayout.Height(19)))
                    {
                        switch (this.Type)
                        {
                            case SectionResourceType.OpenObject:
                                this.OpenObject();
                                break;

                            case SectionResourceType.OpenEditorWindow:
                                this.OpenEditorWindow();
                                break;

                            case SectionResourceType.Url:
                                this.OpenUrl();
                                break;
                        }
                    }
                }

                if (this.HighlightButton && !disable)
                {
                    GUIHelper.PopColor();
                }

                GUIHelper.PopGUIEnabled();
            }

            private void OpenObject()
            {
                var obj = this.UnityObject;

                if (obj == null)
                {
                    obj = AssetDatabase.LoadAssetAtPath(this.StringValue, typeof(UnityEngine.Object));
                }

                EditorGUIUtility.PingObject(obj);
                if (obj as ScriptableObject)
                {
                    var window = OdinEditorWindow.InspectObject(obj);
                    window.position = GUIHelper.GetEditorWindowRect().AlignCenter(windowSize.x, windowSize.y);
                }
                else
                {
                    UnityEditorEventUtility.DelayAction(() =>
                    {
                        AssetDatabase.OpenAsset(obj);
                        if (obj as SceneAsset)
                        {
                            UnityEditorEventUtility.DelayAction(() =>
                            {
                                GameObject.FindObjectsOfType<Transform>()
                                    .Where(x => x.parent == null && x.childCount > 0)
                                    .OrderByDescending(x => x.GetSiblingIndex())
                                    .Select(x => x.transform.GetChild(0).gameObject)
                                    .ForEach(x => EditorGUIUtility.PingObject(x));
                            });
                        }
                    });
                }
            }

            private void OpenEditorWindow()
            {
                var type = AssemblyUtilities.GetTypeByCachedFullName(this.StringValue);
                if (type != null)
                {
                    if (type.InheritsFrom<UnityEditor.EditorWindow>())
                    {
                        var window = EditorWindow.GetWindow(type);
                        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(windowSize.x, windowSize.y);
                    }
                    else
                    {
                        var window = OdinEditorWindow.InspectObject(Activator.CreateInstance(type));
                        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(windowSize.x, windowSize.y);
                    }
                }
                else if (File.Exists(this.StringValue))
                {
                    var obj = AssetDatabase.LoadAssetAtPath(this.StringValue, typeof(UnityEngine.Object));
                    var window = OdinEditorWindow.InspectObject(obj);
                    window.position = GUIHelper.GetEditorWindowRect().AlignCenter(windowSize.x, windowSize.y);
                }
            }

            private void OpenUrl()
            {
                Application.OpenURL(this.StringValue);
            }
        }

        public enum SectionResourceType
        {
            OpenObject,
            OpenEditorWindow,
            Url
        }
    }
}