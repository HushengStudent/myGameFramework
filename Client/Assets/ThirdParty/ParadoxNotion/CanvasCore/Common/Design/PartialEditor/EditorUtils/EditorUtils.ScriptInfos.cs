#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ParadoxNotion.Design
{

    ///Have some commonly stuff used across most inspectors and helper functions. Keep outside of Editor folder since many runtime classes use this in #if UNITY_EDITOR
    ///This is a partial class. Different implementation provide different tools, so that everything is referenced from within one class.
    partial class EditorUtils
    {

        [InitializeOnLoadMethod]
        static void Initialize_ScriptInfos() {
            TypePrefs.onPreferredTypesChanged += () => { cachedInfos = null; };
        }

        //For gathering script/type meta-information
        public class ScriptInfo
        {
            public Type originalType;
            public string originalName;
            public string originalCategory;

            public Type type;
            public string name;
            public string category;
            public int priority;

            public ScriptInfo() { }
            public ScriptInfo(Type type, string name, string category, int priority) {
                this.type = type;
                this.name = name;
                this.category = category;
                this.priority = priority;
            }
        }

        ///Get a list of ScriptInfos of the baseType excluding: the base type, abstract classes, Obsolete classes and those with the DoNotList attribute, categorized as a list of ScriptInfo
        private static Dictionary<Type, List<ScriptInfo>> cachedInfos;
        public static List<ScriptInfo> GetScriptInfosOfType(Type baseType) {

            if ( cachedInfos == null ) { cachedInfos = new Dictionary<Type, List<ScriptInfo>>(); }

            List<ScriptInfo> infosResult;
            if ( cachedInfos.TryGetValue(baseType, out infosResult) ) {
                return infosResult.ToList();
            }

            infosResult = new List<ScriptInfo>();

            var subTypes = ReflectionTools.GetImplementationsOf(baseType);
            if ( baseType.IsGenericTypeDefinition ) {
                subTypes = new Type[] { baseType };
            }

            foreach ( var subType in subTypes ) {

                if ( subType.IsAbstract || subType.IsDefined(typeof(DoNotListAttribute), true) || subType.IsDefined(typeof(ObsoleteAttribute), true) ) {
                    continue;
                }

                var isGeneric = subType.IsGenericTypeDefinition && subType.GetGenericArguments().Length == 1;
                var scriptName = subType.FriendlyName().SplitCamelCase();
                var scriptCategory = string.Empty;
                var scriptPriority = 0;

                var nameAttribute = subType.RTGetAttribute<NameAttribute>(true);
                if ( nameAttribute != null ) {
                    scriptPriority = nameAttribute.priority;
                    scriptName = nameAttribute.name;
                    if ( isGeneric && !scriptName.EndsWith("<T>") ) {
                        scriptName += " (T)";
                    }
                }

                var categoryAttribute = subType.RTGetAttribute<CategoryAttribute>(true);
                if ( categoryAttribute != null ) {
                    scriptCategory = categoryAttribute.category;
                }

                var info = new ScriptInfo(subType, scriptName, scriptCategory, scriptPriority);
                info.originalType = subType;
                info.originalName = scriptName;
                info.originalCategory = scriptCategory;

                //add the generic types based on constrains and prefered types list
                if ( isGeneric ) {
                    var exposeAsBaseDefinition = subType.RTIsDefined<ExposeAsDefinitionAttribute>(true);
                    if ( !exposeAsBaseDefinition ) {
                        var typesToWrap = TypePrefs.GetPreferedTypesList(true);
                        foreach ( var t in typesToWrap ) {
                            infosResult.Add(info.MakeGenericInfo(t, string.Format("/{0}/{1}", info.name, t.NamespaceToPath())));
                            infosResult.Add(info.MakeGenericInfo(typeof(List<>).MakeGenericType(t), string.Format("/{0}/{1}{2}", info.name, TypePrefs.LIST_MENU_STRING, t.NamespaceToPath()), -1));
                            infosResult.Add(info.MakeGenericInfo(typeof(Dictionary<,>).MakeGenericType(typeof(string), t), string.Format("/{0}/{1}{2}", info.name, TypePrefs.DICT_MENU_STRING, t.NamespaceToPath()), -2));
                        }
                        continue;
                    }
                }

                infosResult.Add(info);
            }

            infosResult = infosResult
            .Where(s => s != null)
            .OrderBy(s => s.GetBaseInfo().name)
            .OrderBy(s => s.GetBaseInfo().priority * -1)
            .OrderBy(s => s.GetBaseInfo().category)
            .ToList();
            cachedInfos[baseType] = infosResult;
            return infosResult;
        }

        ///Returns the base "definition" indo from which the provided info was made
        public static ScriptInfo GetBaseInfo(this ScriptInfo info) {
            var result = new ScriptInfo(info.originalType, info.originalName, info.originalCategory, info.priority);
            result.originalType = info.originalType;
            result.originalName = info.originalName;
            result.originalCategory = info.originalCategory;
            return result;
        }

        ///Makes and returns a closed generic ScriptInfo for targetType out of an existing ScriptInfo
        public static ScriptInfo MakeGenericInfo(this ScriptInfo info, Type targetType, string subCategory = null, int priorityShift = 0) {
            if ( info == null || !info.originalType.IsGenericTypeDefinition ) {
                return null;
            }

            info = info.GetBaseInfo();
            var genericArg = info.originalType.GetGenericArguments().First();
            if ( targetType.IsAllowedByGenericArgument(genericArg) ) {
                var genericType = info.originalType.MakeGenericType(targetType);
                var genericCategory = info.category + subCategory;
                var genericName = info.name.Replace("(T)", string.Format("({0})", targetType.FriendlyName()));
                var newInfo = new ScriptInfo(genericType, genericName, genericCategory, info.priority + priorityShift);
                newInfo.originalType = info.originalType;
                newInfo.originalName = info.originalName;
                newInfo.originalCategory = info.originalCategory;
                return newInfo;
            }
            return null;
        }

        //Not really. Only for purposes of ScriptInfos usage.
        static string NamespaceToPath(this Type type) {
            if ( type == null ) { return string.Empty; }
            return string.IsNullOrEmpty(type.Namespace) ? "No Namespace" : type.Namespace.Split('.').First();
        }
    }
}

#endif