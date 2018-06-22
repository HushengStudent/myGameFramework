using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public static class ReflectedNodesHelper
    {
        private const string RETURN_VALUE_NAME = "Value";

        public static ParamDef GetGetterDefFromInfo(MemberInfo info)
        {
            ParamDef result = new ParamDef
            {
                presentedInfo = info,
                paramMode = ParamMode.Undefined
            };
            if (info != null)
            {
                result.paramMode = ParamMode.Out;
                var methodInfo = info as MethodInfo;
                if (methodInfo != null)
                {
                    var name = methodInfo.Name;
                    if (name.StartsWith(ReflectionTools.METHOD_SPECIAL_NAME_GET))
                    {
                        name = name.Substring(ReflectionTools.METHOD_SPECIAL_NAME_GET.Length);
                    }
                    result.portName = name;
                    result.paramType = methodInfo.ReturnType;
                }
                var fieldInfo = info as FieldInfo;
                if (fieldInfo != null)
                {
                    result.portName = fieldInfo.Name;
                    result.paramType = fieldInfo.FieldType;
                }
            }
            return result;
        }

        public static ParamDef GetDefFromInfo(ParameterInfo info, bool last)
        {
            ParamDef result = new ParamDef();
            if (info != null)
            {
                var directType = info.ParameterType;
                var isParams = false;
                if (last && directType.RTIsArray())
                {
                    isParams = info.IsDefined(typeof(ParamArrayAttribute), false);
                }
                var elementType = directType.RTGetElementType();
                if (isParams)
                {
                    result.arrayType = directType.GetEnumerableElementType();
                }
                result.isParamsArray = isParams;
                var realType = directType.RTIsByRef() && elementType != null ? elementType : directType;
                result.paramType = realType;
                if (info.IsOut && directType.RTIsByRef())
                {
                    result.paramMode = ParamMode.Out;
                }
                else if (!info.IsOut && info.ParameterType.RTIsByRef())
                {
                    result.paramMode = ParamMode.Ref;
                }
                else
                {
                    result.paramMode = ParamMode.In;
                }
                result.portName = info.Name;
            }
            return result;
        }

        public static bool InitParams(Type targetType, bool isStatic, MemberInfo[] infos, out ParametresDef parametres)
        {
            parametres = default(ParametresDef);
            if (targetType == null) return false;
            parametres = new ParametresDef {paramDefinitions = new List<ParamDef>()};
            if (!isStatic)
            {
                parametres.resultDef = new ParamDef{paramMode = ParamMode.Undefined};
                parametres.instanceDef = new ParamDef
                {
                    paramType = targetType,
                    portName = targetType.FriendlyName(),
                    portId = "Instance",
                    paramMode = ParamMode.Instance
                };
            }
            for (var i = 0; i <= infos.Length - 1; i++)
            {
                var def = GetGetterDefFromInfo(infos[i]);
                if (def.paramMode != ParamMode.Undefined)
                {
                    parametres.paramDefinitions.Add(def);
                }
            }
            return true;
        }

        private static bool InitParams(ParameterInfo[] prms, Type returnType, ref ParametresDef parametres)
        {
            bool valueNameIsUsed = false;
            for (int i = 0; i <= prms.Length - 1; i++)
            {
                var def = GetDefFromInfo(prms[i], i == prms.Length - 1);
                if (def.portName == RETURN_VALUE_NAME && !valueNameIsUsed) valueNameIsUsed = true;
                if (parametres.instanceDef.paramMode != ParamMode.Undefined && def.portName == parametres.instanceDef.portName &&
                    (def.paramMode == ParamMode.In || def.paramMode == ParamMode.Ref || def.paramMode == ParamMode.Out))
                {
                    def.portId = def.portName + " ";
                }
                parametres.paramDefinitions.Add(def);
            }
            if (returnType != typeof(void))
            {
                parametres.resultDef.paramType = returnType;
                parametres.resultDef.portName = RETURN_VALUE_NAME;
                parametres.resultDef.portId = valueNameIsUsed? "*Value" : null;
                parametres.resultDef.paramMode = ParamMode.Result;
            }
            return true;
        }

        public static bool InitParams(ConstructorInfo constructor, out ParametresDef parametres)
        {
            parametres = new ParametresDef
            {
                paramDefinitions = new List<ParamDef>(),
                instanceDef = new ParamDef { paramMode = ParamMode.Undefined },
                resultDef = new ParamDef { paramMode = ParamMode.Undefined }
            };
            if (constructor == null || constructor.ContainsGenericParameters || constructor.IsGenericMethodDefinition) return false;
            var prms = constructor.GetParameters();
            var returnType = constructor.RTReflectedType();
            return InitParams(prms, returnType, ref parametres);
        }

        public static bool InitParams(MethodInfo method, out ParametresDef parametres)
        {
            parametres = new ParametresDef
            {
                paramDefinitions = new List<ParamDef>(),
                instanceDef = new ParamDef { paramMode = ParamMode.Undefined },
                resultDef = new ParamDef { paramMode = ParamMode.Undefined }
            };
            if (method == null || method.ContainsGenericParameters || method.IsGenericMethodDefinition) return false;
            var prms = method.GetParameters();
            var returnType = method.ReturnType;
            if (!method.IsStatic)
            {
                parametres.instanceDef.paramType = method.DeclaringType;
                parametres.instanceDef.portName = method.DeclaringType.FriendlyName();
                parametres.instanceDef.paramMode = ParamMode.Instance;
            }
            return InitParams(prms, returnType, ref parametres);
        }

        public static bool InitParams(FieldInfo field, out ParametresDef parametres)
        {
            parametres = new ParametresDef
            {
                paramDefinitions = null,
                instanceDef = new ParamDef { paramMode = ParamMode.Undefined},
                resultDef = new ParamDef { paramMode = ParamMode.Undefined },
            };
            if (field == null || field.FieldType.ContainsGenericParameters || field.FieldType.IsGenericTypeDefinition) return false;
            if (!field.IsStatic)
            {
                parametres.instanceDef.paramMode = ParamMode.Instance;
                parametres.instanceDef.paramType = field.DeclaringType;
                parametres.instanceDef.portName = field.DeclaringType.FriendlyName();
            }
            parametres.resultDef.paramMode = ParamMode.Result;
            parametres.resultDef.paramType = field.FieldType;
            parametres.resultDef.portName = RETURN_VALUE_NAME;
            return true;
        }

        public static string GetGeneratedKey(MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                return string.Format("{0} {1} {2}", memberInfo.DeclaringType.FullName, memberInfo.MemberType, memberInfo);
            }
            return string.Empty;
        }
    }
}