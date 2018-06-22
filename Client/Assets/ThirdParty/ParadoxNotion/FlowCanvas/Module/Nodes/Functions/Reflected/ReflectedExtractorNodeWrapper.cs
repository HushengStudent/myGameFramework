using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
    //Wrap public readable properties and fields for selected type
    [DoNotList]
    [Description("Chose and expose any number of fields or properties of the type. If you only require a single field / property, it's better to get that field / property directly without an Extractor.")]
    [Icon(runtimeIconTypeCallback: "GetRuntimeIconType")]
    public class ReflectedExtractorNodeWrapper<T> : FlowNode
    {
        private static Dictionary<string, MemberInfo> _memberInfos;
        private static List<string> _instanceMemberNames;
        private static List<string> _staticMemberNames;

        private static void FillInfos()
        {
            if(_memberInfos != null) return;
            _memberInfos         = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);
            _instanceMemberNames = new List<string>();
            _staticMemberNames   = new List<string>();
            var targetType       = typeof(T);
            var fields           = targetType.RTGetFields();
            var properties       = targetType.RTGetProperties();

            foreach (var field in fields)
            {
                if (field == null || !field.IsPublic || field.IsObsolete()) {continue;}
                _memberInfos[field.Name] = field;
                (field.IsStatic? _staticMemberNames : _instanceMemberNames).Add(field.Name);
            }

            foreach (var prop in properties)
            {
                if (prop == null || prop.IsIndexerProperty() || prop.IsObsolete()) {continue;}
                var getter = prop.RTGetGetMethod();
                if (getter == null || !getter.IsPublic) {continue;}
                _memberInfos[prop.Name] = getter;
                (getter.IsStatic? _staticMemberNames : _instanceMemberNames).Add(prop.Name);
            }
        }

        public System.Type GetRuntimeIconType()
        {
            return typeof(T);
        }

        [SerializeField]
        private bool _isStatic;
        [SerializeField]
        private string[] _selectedInstanceMembers;
        [SerializeField]
        private string[] _selectedStaticMembers;

       
        [NonSerialized]
        private BaseReflectedExtractorNode extractorNode;

        public override string name
        {
            get { return string.Format("Extract ({0})", typeof(T).FriendlyName()); }
        }

        public override void OnCreate(NodeCanvas.Framework.Graph assignedGraph)
        {
            _selectedInstanceMembers = new string[_instanceMemberNames.Count];
            GatherPorts();
        } 

        private void CheckData()
        {
            FillInfos();
            if (_selectedInstanceMembers == null || _selectedInstanceMembers.Length != _instanceMemberNames.Count){ _selectedInstanceMembers = new string[_instanceMemberNames.Count]; }
        }

        protected override void RegisterPorts()
        {
            CheckData();
            var neededNames = _isStatic? _selectedStaticMembers : _selectedInstanceMembers;
            var list = new List<MemberInfo>();
            for (var i = 0; i < neededNames.Length; i++)
            {
                var name = neededNames[i];
                if (string.IsNullOrEmpty(name)) {continue;}
                MemberInfo info;
                _memberInfos.TryGetValue(name, out info);
                if (info != null)
                {
                    list.Add(info);
                }
            }
            extractorNode = BaseReflectedExtractorNode.GetExtractorNode(typeof(T), _isStatic, list.ToArray());
            if (extractorNode != null)
            {
                extractorNode.RegisterPorts(this);
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
        ///----------------------------------------------------------------------------------------------
        #if UNITY_EDITOR
        
        //...
        protected override void OnNodeInspectorGUI()
        {
            if (_instanceMemberNames.Count > 0){
                UnityEditor.EditorGUI.BeginChangeCheck();
                DoListCheckout(_instanceMemberNames, _selectedInstanceMembers);
                if (UnityEditor.EditorGUI.EndChangeCheck()){
                    GatherPorts();
                }
            } else {
                UnityEditor.EditorGUILayout.HelpBox("This type has neither public fields nor public properties to extract.", UnityEditor.MessageType.Info);
            }

            base.OnNodeInspectorGUI();
        }

        //...
        static void DoListCheckout(List<string> input, string[] output)
        {
            for (var i = 0; i < input.Count; i++){
                var name = input[i];
                var active = output[i] != null;
                GUI.color = active? Color.white : new Color(1,1,1,0.5f);
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(name.SplitCamelCase());
                GUILayout.Label("Expose", GUILayout.Width(50));
                GUI.color = Color.white;
                active = UnityEditor.EditorGUILayout.Toggle(active, GUILayout.Width(20));
                GUILayout.EndHorizontal();
                output[i] = active? name : null;
            }
        }

        #endif

    }
}