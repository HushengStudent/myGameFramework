using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace ParadoxNotion
{

    ///Reflection utility and extention methods
    public static class ReflectionTools
    {
        public const BindingFlags FLAGS_ALL = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        public const BindingFlags FLAGS_ALL_DECLARED = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private static Assembly[] _loadedAssemblies;
        private static Assembly[] loadedAssemblies {
            get
            {
                if ( _loadedAssemblies == null ) {
                    _loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                }
                return _loadedAssemblies;
            }
        }

        //Alternative to Type.GetType to work with FullName instead of AssemblyQualifiedName when looking up a type by string
        //This also handles Generics and their arguments, assembly changes and namespace changes to some extend.
        private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
        public static Type GetType(string typeFullName) { return GetType(typeFullName, false, null); }
        public static Type GetType(string typeFullName, Type fallbackAssignable) { return GetType(typeFullName, true, fallbackAssignable); }
        public static Type GetType(string typeFullName, bool fallbackNoNamespace = false, Type fallbackAssignable = null) {

            if ( string.IsNullOrEmpty(typeFullName) ) {
                return null;
            }

            Type type = null;
            if ( typeMap.TryGetValue(typeFullName, out type) ) {
                return type;
            }

            //direct look up
            type = GetTypeDirect(typeFullName);
            if ( type != null ) {
                return typeMap[typeFullName] = type;
            }

            //handle generics now
            type = TryResolveGenericType(typeFullName, fallbackNoNamespace, fallbackAssignable);
            if ( type != null ) {
                return typeMap[typeFullName] = type;
            }

            //make use of DeserializeFromAttribute
            type = TryResolveDeserializeFromAttribute(typeFullName);
            if ( type != null ) {
                return typeMap[typeFullName] = type;
            }

            if ( fallbackNoNamespace ) {
                //get type regardless namespace
                type = TryResolveWithoutNamespace(typeFullName, fallbackAssignable);
                if ( type != null ) {
                    //we store the found type's.FullName in the cache (instead of provided name), so that other types dont fail.
                    return typeMap[type.FullName] = type;
                }
            }

            ParadoxNotion.Services.Logger.LogError(string.Format("Type with name '{0}' could not be resolved.", typeFullName), "Type Request");
            return typeMap[typeFullName] = null;
        }

        //direct type look up with it's FullName
        static Type GetTypeDirect(string typeFullName) {
            var type = Type.GetType(typeFullName);
            if ( type != null ) {
                return type;
            }

            for ( var i = 0; i < loadedAssemblies.Length; i++ ) {
                var asm = loadedAssemblies[i];
                try { type = asm.GetType(typeFullName); }
                catch { continue; }
                if ( type != null ) {
                    return type;
                }
            }

            return null;
        }

        //Resolve generic types by their .FullName or .ToString
        //Remark: a generic's type .FullName returns a string where it's arguments only are instead printed as AssemblyQualifiedName.
        static Type TryResolveGenericType(string typeFullName, bool fallbackNoNamespace = false, Type fallbackAssignable = null) {

            //ensure that it is a generic type implementation, not a definition
            if ( typeFullName.Contains('`') == false || typeFullName.Contains('[') == false ) {
                return null;
            }

            try //big try/catch block cause maybe there is a bug. Hopefully not.
            {
                var quoteIndex = typeFullName.IndexOf('`');
                var genericTypeDefName = typeFullName.Substring(0, quoteIndex + 2);
                var genericTypeDef = GetType(genericTypeDefName, fallbackNoNamespace, fallbackAssignable);
                if ( genericTypeDef == null ) {
                    return null;
                }

                int argCount = Convert.ToInt32(typeFullName.Substring(quoteIndex + 1, 1));
                var content = typeFullName.Substring(quoteIndex + 2, typeFullName.Length - quoteIndex - 2);
                string[] split = null;
                if ( content.StartsWith("[[") ) { //this means that assembly qualified name is contained. Name was generated with FullName.
                    var startIndex = typeFullName.IndexOf("[[") + 2;
                    var endIndex = typeFullName.LastIndexOf("]]");
                    content = typeFullName.Substring(startIndex, endIndex - startIndex);
                    split = content.Split(new string[] { "],[" }, argCount, StringSplitOptions.RemoveEmptyEntries).ToArray();
                } else { //this means that the name was generated with type.ToString().
                    var startIndex = typeFullName.IndexOf('[') + 1;
                    var endIndex = typeFullName.LastIndexOf(']');
                    content = typeFullName.Substring(startIndex, endIndex - startIndex);
                    split = content.Split(new char[] { ',' }, argCount, StringSplitOptions.RemoveEmptyEntries).ToArray();

                }

                var argTypes = new Type[argCount];
                for ( var i = 0; i < split.Length; i++ ) {
                    var subName = split[i];
                    if ( !subName.Contains('`') && subName.Contains(',') ) { //remove assembly info since we work with FullName, but only if it's not yet another generic.
                        subName = subName.Substring(0, subName.IndexOf(','));
                    }

                    var argType = GetType(subName);
                    if ( argType == null ) {
                        return null;
                    }
                    argTypes[i] = argType;
                }

                return genericTypeDef.RTMakeGenericType(argTypes);
            }

            catch ( Exception e ) {
                ParadoxNotion.Services.Logger.LogException(e, "Type Request Bug");
                return null;
            }
        }

        //uterly slow, but only happens when we have a null type
        static Type TryResolveDeserializeFromAttribute(string typeName) {
            var allTypes = GetAllTypes(true);
            for ( var i = 0; i < allTypes.Length; i++ ) {
                var t = allTypes[i];
                var att = t.RTGetAttribute<Serialization.DeserializeFromAttribute>(false);
                if ( att != null && att.previousTypeFullName == typeName ) {
                    return t;
                }
            }
            return null;
        }

        //fallback type look up with it's FullName. This is slow.
        static Type TryResolveWithoutNamespace(string typeName, Type fallbackAssignable = null) {

            //dont handle generic implementations this way (still handles definitions though).
            if ( typeName.Contains('`') && typeName.Contains('[') ) {
                return null;
            }

            //remove assembly info if any
            if ( typeName.Contains(',') ) {
                typeName = typeName.Substring(0, typeName.IndexOf(','));
            }

            //ensure strip namespace
            if ( typeName.Contains('.') ) {
                var dotIndex = typeName.LastIndexOf('.') + 1;
                typeName = typeName.Substring(dotIndex, typeName.Length - dotIndex);
            }

            //check all types
            var allTypes = GetAllTypes(true);
            for ( var i = 0; i < allTypes.Length; i++ ) {
                var t = allTypes[i];
                if ( t.Name == typeName && ( fallbackAssignable == null || fallbackAssignable.RTIsAssignableFrom(t) ) ) {
                    return t;
                }
            }
            return null;
        }

        ///----------------------------------------------------------------------------------------------

        ///Get every single type in loaded assemblies
        private static Type[] _allTypes;
        public static Type[] GetAllTypes(bool includeObsolete) {
            if ( _allTypes != null ) {
                return _allTypes;
            }

            var result = new List<Type>();
            for ( var i = 0; i < loadedAssemblies.Length; i++ ) {
                var asm = loadedAssemblies[i];
                try { result.AddRange(asm.GetExportedTypes().Where(t => includeObsolete == true || !t.RTIsDefined<System.ObsoleteAttribute>(true))); }
                catch { continue; }
            }
            return _allTypes = result.OrderBy(t => t.FriendlyName()).OrderBy(t => t.Namespace).ToArray();
        }

        ///Get a collection of types assignable to provided type, excluding Abstract types
        private static Dictionary<Type, Type[]> _subTypesMap = new Dictionary<Type, Type[]>();
        public static Type[] GetImplementationsOf(Type baseType) {

            Type[] result = null;
            if ( _subTypesMap.TryGetValue(baseType, out result) ) {
                return result;
            }

            var temp = new List<Type>();
            var allTypes = GetAllTypes(false);
            for ( var i = 0; i < allTypes.Length; i++ ) {
                var type = allTypes[i];
                if ( baseType.RTIsAssignableFrom(type) && !type.RTIsAbstract() ) {
                    temp.Add(type);
                }
            }
            return _subTypesMap[baseType] = temp.ToArray();
        }

        ///----------------------------------------------------------------------------------------------

        private static object[] _tempArgs = new object[1];
        ///Returns an object[] with a single element, that can for example be used as method invocation args
        public static object[] SingleTempArgsArray(object arg) {
            _tempArgs[0] = arg;
            return _tempArgs;
        }

        ///----------------------------------------------------------------------------------------------

        //Method operator special name to friendly name map
        public readonly static Dictionary<string, string> op_FriendlyNamesLong = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"op_Equality", "Equal"},
            {"op_Inequality", "Not Equal"},
            {"op_GreaterThan", "Greater"},
            {"op_LessThan", "Less"},
            {"op_GreaterThanOrEqual", "Greater Or Equal"},
            {"op_LessThanOrEqual", "Less Or Equal"},
            {"op_Addition", "Add"},
            {"op_Subtraction", "Subtract"},
            {"op_Division", "Divide"},
            {"op_Multiply", "Multiply"},
            {"op_UnaryNegation", "Negate"},
            {"op_UnaryPlus", "Positive"},
            {"op_Increment", "Increment"},
            {"op_Decrement", "Decrement"},
            {"op_LogicalNot", "NOT"},
            {"op_OnesComplement", "Complements"},
            {"op_False", "FALSE"},
            {"op_True", "TRUE"},
            {"op_Modulus", "MOD"},
            {"op_BitwiseAnd", "AND"},
            {"op_BitwiseOR", "OR"},
            {"op_LeftShift", "Shift Left"},
            {"op_RightShift", "Shift Right"},
            {"op_ExclusiveOr", "XOR"},
            {"op_Implicit", "Convert"},
            {"op_Explicit", "Convert"},
        };

        //Method operator special name to friendly name map
        public readonly static Dictionary<string, string> op_FriendlyNamesShort = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"op_Equality", "="},
            {"op_Inequality", "≠"},
            {"op_GreaterThan", ">"},
            {"op_LessThan", "<"},
            {"op_GreaterThanOrEqual", "≥"},
            {"op_LessThanOrEqual", "≤"},
            {"op_Addition", "+"},
            {"op_Subtraction", "-"},
            {"op_Division", "÷"},
            {"op_Multiply", "×"},
            {"op_UnaryNegation", "Negate"},
            {"op_UnaryPlus", "Positive"},
            {"op_Increment", "++"},
            {"op_Decrement", "--"},
            {"op_LogicalNot", "NOT"},
            {"op_OnesComplement", "~"},
            {"op_False", "FALSE"},
            {"op_True", "TRUE"},
            {"op_Modulus", "MOD"},
            {"op_BitwiseAnd", "AND"},
            {"op_BitwiseOR", "OR"},
            {"op_LeftShift", "<<"},
            {"op_RightShift", ">>"},
            {"op_ExclusiveOr", "XOR"},
            {"op_Implicit", "Convert"},
            {"op_Explicit", "Convert"},
        };

        ///Operator C# to friendly aliases
        public readonly static Dictionary<string, string> op_CSharpAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"!=", "≠"},
            {">=", "≥"},
            {"<=", "≤"},
            {"/", "÷"},
            {"*", "×"},
        };

        public const string METHOD_SPECIAL_NAME_GET = "get_";
        public const string METHOD_SPECIAL_NAME_SET = "set_";
        public const string METHOD_SPECIAL_NAME_ADD = "add_";
        public const string METHOD_SPECIAL_NAME_REMOVE = "remove_";
        public const string METHOD_SPECIAL_NAME_OP = "op_";

        public enum MethodType
        {
            Normal = 0,
            PropertyAccessor = 1,
            Event = 2,
            Operator = 3,
        }

        ///Returns the type of method case of accessor, operator or event.
        public static MethodType GetMethodSpecialType(this MethodBase method) {
            var name = method.Name;
            if ( method.IsSpecialName ) {
                if ( name.StartsWith(METHOD_SPECIAL_NAME_GET) || name.StartsWith(METHOD_SPECIAL_NAME_SET) ) {
                    return MethodType.PropertyAccessor;
                }
                if ( name.StartsWith(METHOD_SPECIAL_NAME_ADD) || name.StartsWith(METHOD_SPECIAL_NAME_REMOVE) ) {
                    return MethodType.Event;
                }
                if ( name.StartsWith(METHOD_SPECIAL_NAME_OP) ) {
                    return MethodType.Operator;
                }
            }
            return MethodType.Normal;
        }

        ///Get a friendly name for the type
        public static string FriendlyName(this Type t, bool compileSafe = false) {

            if ( t == null ) {
                return null;
            }

            if ( !compileSafe && t.IsByRef ) {
                t = t.GetElementType();
            }

            if ( !compileSafe && t == typeof(UnityEngine.Object) ) {
                return "UnityObject";
            }

            var s = compileSafe ? t.FullName : t.Name;
            if ( !compileSafe ) {
                if ( s == "Single" ) { s = "Float"; }
                if ( s == "Single[]" ) { s = "Float[]"; }
                if ( s == "Int32" ) { s = "Integer"; }
                if ( s == "Int32[]" ) { s = "Integer[]"; }
            }

            if ( t.RTIsGenericParameter() ) {
                s = "T";
            }

            if ( t.RTIsGenericType() ) {
                s = compileSafe && !string.IsNullOrEmpty(t.Namespace) ? t.Namespace + "." + t.Name : t.Name;
                var args = t.RTGetGenericArguments();
                if ( args.Length != 0 ) {

                    s = s.Replace("`" + args.Length.ToString(), "");

                    s += compileSafe ? "<" : " (";
                    for ( var i = 0; i < args.Length; i++ ) {
                        s += ( i == 0 ? "" : ", " ) + args[i].FriendlyName(compileSafe);
                    }
                    s += compileSafe ? ">" : ")";
                }
            }

            return s;
        }

        ///Get a friendly name for member info
        public static string FriendlyName(this MemberInfo info) {
            if ( info == null ) { return null; }
            if ( info is Type ) { return FriendlyName((Type)info); }
            var type = info.ReflectedType.FriendlyName();
            return type + '.' + info.Name;
        }

        ///Get a friendly name of a methd which is the case for when it's a special name.
        public static string FriendlyName(this MethodBase method) { var specialType = MethodType.Normal; return method.FriendlyName(out specialType); }
        public static string FriendlyName(this MethodBase method, out MethodType specialNameType) {
            specialNameType = MethodType.Normal;
            var methodName = method.Name;
            if ( method.IsSpecialName ) {
                if ( methodName.StartsWith(METHOD_SPECIAL_NAME_GET) ) {
                    methodName = "Get " + methodName.Substring(METHOD_SPECIAL_NAME_GET.Length).CapitalizeFirst();
                    specialNameType = MethodType.PropertyAccessor;
                    return methodName;
                }
                if ( methodName.StartsWith(METHOD_SPECIAL_NAME_SET) ) {
                    methodName = "Set " + methodName.Substring(METHOD_SPECIAL_NAME_SET.Length).CapitalizeFirst();
                    specialNameType = MethodType.PropertyAccessor;
                    return methodName;
                }
                if ( methodName.StartsWith(METHOD_SPECIAL_NAME_ADD) ) {
                    methodName = methodName.Substring(METHOD_SPECIAL_NAME_ADD.Length) + " +=";
                    specialNameType = MethodType.Event;
                    return methodName;
                }
                if ( methodName.StartsWith(METHOD_SPECIAL_NAME_REMOVE) ) {
                    methodName = methodName.Substring(METHOD_SPECIAL_NAME_REMOVE.Length) + " -=";
                    specialNameType = MethodType.Event;
                    return methodName;
                }
                if ( methodName.StartsWith(METHOD_SPECIAL_NAME_OP) ) {
                    op_FriendlyNamesLong.TryGetValue(methodName, out methodName);
                    specialNameType = MethodType.Operator;
                    return methodName;
                }
            }
            return methodName;
        }

        ///Get a friendly full signature string name for a method
		private static Dictionary<MethodBase, string> cacheSignatures = new Dictionary<MethodBase, string>();
        public static string SignatureName(this MethodBase method) {
            string sig = null;
            if ( cacheSignatures.TryGetValue(method, out sig) ) {
                return sig;
            }

            var specialType = MethodType.Normal;
            var methodName = method.FriendlyName(out specialType);
            var parameters = method.GetParameters();
            if ( method is ConstructorInfo ) {
                sig = string.Format("new {0} (", method.DeclaringType.FriendlyName());
            } else {
                sig = string.Format("{0}{1} (", method.IsStatic && specialType != MethodType.Operator ? "static " : "", methodName);
            }
            for ( var i = 0; i < parameters.Length; i++ ) {
                var p = parameters[i];
                if ( p.IsParams(parameters) ) {
                    sig += "params ";
                }
                sig += ( p.ParameterType.IsByRef ? ( p.IsOut ? "out " : "ref " ) : "" ) + p.ParameterType.FriendlyName() + ( i < parameters.Length - 1 ? ", " : "" );
            }
            if ( method is MethodInfo ) {
                sig += ") : " + ( method as MethodInfo ).ReturnType.FriendlyName();
            } else {
                sig += ")";
            }
            return cacheSignatures[method] = sig;
        }

        ///----------------------------------------------------------------------------------------------

        public static Type RTReflectedOrDeclaredType(this MemberInfo member) {
            return member.ReflectedType != null ? member.ReflectedType : member.DeclaringType;
            // return member.DeclaringType;
        }

        public static bool RTIsAssignableFrom(this Type type, Type other) {
            return type.IsAssignableFrom(other);
        }

        public static bool RTIsAssignableTo(this Type type, Type other) {
            return other.RTIsAssignableFrom(type);
        }

        public static bool RTIsAbstract(this Type type) {
            return type.IsAbstract;
        }

        public static bool RTIsValueType(this Type type) {
            return type.IsValueType;
        }

        public static bool RTIsArray(this Type type) {
            return type.IsArray;
        }

        public static bool RTIsInterface(this Type type) {
            return type.IsInterface;
        }

        public static bool RTIsSubclassOf(this Type type, Type other) {
            return type.IsSubclassOf(other);
        }

        public static bool RTIsGenericParameter(this Type type) {
            return type.IsGenericParameter;
        }

        public static bool RTIsGenericType(this Type type) {
            return type.IsGenericType;
        }

        public static MethodInfo RTGetGetMethod(this PropertyInfo prop) {
            return prop.GetGetMethod();
        }

        public static MethodInfo RTGetSetMethod(this PropertyInfo prop) {
            return prop.GetSetMethod();
        }

        public static MethodInfo RTGetDelegateMethodInfo(this Delegate del) {
            return del.Method;
        }

        public static Type RTMakeGenericType(this Type type, params Type[] typeArgs) {
            return type.MakeGenericType(typeArgs);
        }

        public static Type[] RTGetGenericArguments(this Type type) {
            return type.GetGenericArguments();
        }

        public static Type[] RTGetEmptyTypes() {
            return Type.EmptyTypes;
        }

        public static Type RTGetElementType(this Type type) {
            if ( type == null ) return null;
            return type.GetElementType();
        }

        public static bool RTIsByRef(this Type type) {
            if ( type == null ) return false;
            return type.IsByRef;
        }

        ///----------------------------------------------------------------------------------------------

        ///Create object of type
        public static object CreateObject(this Type type) {
            if ( type == null ) return null;
            return Activator.CreateInstance(type);
        }

        ///Create uninitialized object of type
		public static object CreateObjectUninitialized(this Type type) {
            if ( type == null ) return null;
            return FormatterServices.GetUninitializedObject(type);
        }

        ///----------------------------------------------------------------------------------------------

        public static ConstructorInfo RTGetDefaultConstructor(this Type type) {
            var ctors = type.RTGetConstructors();
            for ( var i = 0; i < ctors.Length; i++ ) {
                if ( ctors[i].GetParameters().Length == 0 ) {
                    return ctors[i];
                }
            }
            return null;
        }

        public static ConstructorInfo RTGetConstructor(this Type type, Type[] paramTypes) {
            var ctors = type.RTGetConstructors();
            for ( var i = 0; i < ctors.Length; i++ ) {
                var ctor = ctors[i];
                var parameters = ctor.GetParameters();
                if ( parameters.Length != paramTypes.Length ) {
                    continue;
                }
                var sequenceEquals = true;
                for ( var j = 0; j < parameters.Length; j++ ) {
                    if ( parameters[j].ParameterType != paramTypes[j] ) {
                        sequenceEquals = false;
                        break;
                    }
                }
                if ( sequenceEquals ) {
                    return ctor;
                }
            }
            return null;
        }

        public static MethodInfo RTGetMethod(this Type type, string name) {
            var methods = type.RTGetMethods();
            for ( var i = 0; i < methods.Length; i++ ) {
                if ( methods[i].Name == name ) {
                    return methods[i];
                }
            }
            return null;
        }

        public static MethodInfo RTGetMethod(this Type type, string name, Type[] paramTypes) {
            var methods = type.RTGetMethods();
            for ( var i = 0; i < methods.Length; i++ ) {
                var m = methods[i];
                if ( m.Name == name ) {
                    var parameters = m.GetParameters();
                    if ( parameters.Length != paramTypes.Length ) {
                        continue;
                    }
                    var sequenceEquals = true;
                    for ( var j = 0; j < parameters.Length; j++ ) {
                        if ( parameters[j].ParameterType != paramTypes[j] ) {
                            sequenceEquals = false;
                            break;
                        }
                    }
                    if ( sequenceEquals ) {
                        return m;
                    }
                }
            }
            return null;
        }

        public static FieldInfo RTGetField(this Type type, string name) {
            var fields = type.RTGetFields();
            for ( var i = 0; i < fields.Length; i++ ) {
                if ( fields[i].Name == name ) {
                    return fields[i];
                }
            }
            return null;
        }

        public static PropertyInfo RTGetProperty(this Type type, string name) {
            var props = type.RTGetProperties();
            for ( var i = 0; i < props.Length; i++ ) {
                if ( props[i].Name == name ) {
                    return props[i];
                }
            }
            return null;
        }

        public static EventInfo RTGetEvent(this Type type, string name) {
            var events = type.RTGetEvents();
            for ( var i = 0; i < events.Length; i++ ) {
                if ( events[i].Name == name ) {
                    return events[i];
                }
            }
            return null;
        }

        ///----------------------------------------------------------------------------------------------

        private static Dictionary<Type, ConstructorInfo[]> _typeConstructors = new Dictionary<Type, ConstructorInfo[]>();
        public static ConstructorInfo[] RTGetConstructors(this Type type) {
            ConstructorInfo[] constructors;
            if ( !_typeConstructors.TryGetValue(type, out constructors) ) {
                constructors = type.GetConstructors(FLAGS_ALL);
                _typeConstructors[type] = constructors;
            }

            return constructors;
        }

        private static Dictionary<Type, MethodInfo[]> _typeMethods = new Dictionary<Type, MethodInfo[]>();
        public static MethodInfo[] RTGetMethods(this Type type) {
            MethodInfo[] methods;
            if ( !_typeMethods.TryGetValue(type, out methods) ) {
                methods = type.GetMethods(FLAGS_ALL);
                _typeMethods[type] = methods;
            }

            return methods;
        }

        private static Dictionary<Type, FieldInfo[]> _typeFields = new Dictionary<Type, FieldInfo[]>();
        public static FieldInfo[] RTGetFields(this Type type) {
            FieldInfo[] fields;
            if ( !_typeFields.TryGetValue(type, out fields) ) {
                fields = type.GetFields(FLAGS_ALL);
                _typeFields[type] = fields;
            }

            return fields;
        }

        private static Dictionary<Type, PropertyInfo[]> _typeProperties = new Dictionary<Type, PropertyInfo[]>();
        public static PropertyInfo[] RTGetProperties(this Type type) {
            PropertyInfo[] properties;
            if ( !_typeProperties.TryGetValue(type, out properties) ) {
                properties = type.GetProperties(FLAGS_ALL);
                _typeProperties[type] = properties;
            }

            return properties;
        }

        private static Dictionary<Type, EventInfo[]> _typeEvents = new Dictionary<Type, EventInfo[]>();
        public static EventInfo[] RTGetEvents(this Type type) {
            EventInfo[] events;
            if ( !_typeEvents.TryGetValue(type, out events) ) {
                events = type.GetEvents(FLAGS_ALL);
                _typeEvents[type] = events;
            }

            return events;
        }

        ///----------------------------------------------------------------------------------------------

        public static bool RTIsDefined<T>(this Type type, bool inherited) where T : Attribute { return type.RTIsDefined(typeof(T), inherited); }
        public static bool RTIsDefined(this Type type, Type attributeType, bool inherited) {
            return type.IsDefined(attributeType, inherited);
        }

        public static bool RTIsDefined<T>(this MemberInfo member, bool inherited) where T : Attribute { return member.RTIsDefined(typeof(T), inherited); }
        public static bool RTIsDefined(this MemberInfo member, Type attributeType, bool inherited) {
            return member.IsDefined(attributeType, inherited);
        }

        private static Dictionary<Type, object[]> _typeAttributes = new Dictionary<Type, object[]>();
        public static T RTGetAttribute<T>(this Type type, bool inherited) where T : Attribute {
            // return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
            object[] attributes;
            if ( !_typeAttributes.TryGetValue(type, out attributes) ) {
                attributes = type.GetCustomAttributes(true);
                _typeAttributes[type] = attributes;
            }
            for ( var i = 0; i < attributes.Length; i++ ) {
                var att = (Attribute)attributes[i];
                var attType = att.GetType();
                if ( attType.RTIsAssignableTo(typeof(T)) ) {
                    if ( inherited || type.RTIsDefined(attType, false) ) {
                        return (T)att;
                    }
                }
            }
            return null;
        }

        private static Dictionary<MemberInfo, object[]> _memberAttributes = new Dictionary<MemberInfo, object[]>();
        public static T RTGetAttribute<T>(this MemberInfo member, bool inherited) where T : Attribute {
            // return (T)member.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
            object[] attributes;
            if ( !_memberAttributes.TryGetValue(member, out attributes) ) {
                attributes = member.GetCustomAttributes(true);
                _memberAttributes[member] = attributes;
            }
            for ( var i = 0; i < attributes.Length; i++ ) {
                var att = (Attribute)attributes[i];
                var attType = att.GetType();
                if ( attType.RTIsAssignableTo(typeof(T)) ) {
                    if ( inherited || member.RTIsDefined(attType, false) ) {
                        return (T)att;
                    }
                }
            }
            return null;
        }

        ///Get all attributes of type T recursively up the type hierarchy
        public static T[] RTGetAttributesRecursive<T>(this Type type) where T : Attribute {
            var result = new List<T>();
            var current = type;
            while ( current != null ) {
                var att = current.RTGetAttribute<T>(false);
                if ( att != null ) {
                    result.Add(att);
                }
                current = current.BaseType;
            }
            return result.ToArray();
        }

        ///----------------------------------------------------------------------------------------------

        public static ParameterInfo[] RTGetDelegateTypeParameters(this Type delegateType) {
            if ( delegateType == null || !delegateType.RTIsSubclassOf(typeof(Delegate)) ) {
                return new ParameterInfo[0];
            }
            var invokeMethod = delegateType.RTGetMethod("Invoke");
            return invokeMethod.GetParameters();
        }

        ///Create delegate
		public static T RTCreateDelegate<T>(this MethodInfo method, object instance) {
            return (T)(object)method.RTCreateDelegate(typeof(T), instance);
        }

        ///Create delegate
		public static Delegate RTCreateDelegate(this MethodInfo method, Type type, object instance) {
            if ( instance != null ) {
                var instanceType = instance.GetType();
                if ( method.DeclaringType != instanceType ) {
                    method = instanceType.RTGetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
                }
            }
            return Delegate.CreateDelegate(type, instance, method);
        }

        ///Convert delegate
        public static Delegate ConvertDelegate(Delegate originalDelegate, Type targetDelegateType) {
            return Delegate.CreateDelegate(targetDelegateType, originalDelegate.Target, originalDelegate.Method);
        }

        ///----------------------------------------------------------------------------------------------

        ///Is the field read only?
        public static bool IsReadOnly(this FieldInfo field) {
            return field.IsInitOnly || field.IsLiteral;
        }

        ///Is the field a Constant?
        public static bool IsConstant(this FieldInfo field) {
            return field.IsReadOnly() && field.IsStatic;
        }

        ///Quicky to get if an event info is static.
        public static bool IsStatic(this EventInfo info) {
            var m = info.GetAddMethod();
            return m != null ? m.IsStatic : false;
        }

        ///Is the parameter provided a params array?
        public static bool IsParams(this ParameterInfo parameter, ParameterInfo[] parameters) {
            return parameter.Position == parameters.Length - 1 && parameter.IsDefined(typeof(ParamArrayAttribute), false);
        }

        ///Utility to determine obsolete members quicker. Also handles property accessor methods.
        public static bool IsObsolete(this MemberInfo member, bool inherited = true) {
            if ( member is MethodInfo ) {
                var m = (MethodInfo)member;
                if ( m.IsPropertyAccessor() ) {
                    member = m.GetAccessorProperty();
                }
            }
            return member.RTIsDefined<System.ObsoleteAttribute>(inherited);
        }

        ///----------------------------------------------------------------------------------------------

        ///BaseDefinition for PropertyInfos.
	    public static PropertyInfo GetBaseDefinition(this PropertyInfo propertyInfo) {
            var method = propertyInfo.GetAccessors(true).FirstOrDefault();
            if ( method == null ) {
                return null;
            }

            var baseMethod = method.GetBaseDefinition();
            if ( baseMethod == method ) {
                return propertyInfo;
            }

            var arguments = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();
            return baseMethod.DeclaringType.GetProperty(propertyInfo.Name, FLAGS_ALL, null, propertyInfo.PropertyType, arguments, null);
        }

        ///BaseDefinition for FieldInfo. Not exactly correct but here for consistency.
        public static FieldInfo GetBaseDefinition(this FieldInfo fieldInfo) {
            return fieldInfo.DeclaringType.RTGetField(fieldInfo.Name);
        }

        ///Get a list of methods that extend the provided type
        private static Dictionary<Type, MethodInfo[]> _typeExtensions = new Dictionary<Type, MethodInfo[]>();
        public static MethodInfo[] GetExtensionMethods(this Type targetType) {
            MethodInfo[] methods = null;
            if ( _typeExtensions.TryGetValue(targetType, out methods) ) {
                return methods;
            }
            var result = new List<MethodInfo>();
            foreach ( var t in GetAllTypes(false) ) {
                if ( !t.IsSealed || t.IsGenericType || !t.RTIsDefined<System.Runtime.CompilerServices.ExtensionAttribute>(false) ) {
                    continue;
                }

                foreach ( var m in t.RTGetMethods() ) {
                    if ( m.IsExtensionMethod() && m.GetParameters()[0].ParameterType.RTIsAssignableFrom(targetType) ) {
                        result.Add(m);
                    }
                }
            }

            return _typeExtensions[targetType] = result.ToArray();
        }

        ///Helper to determine if method is extension quicker.
        public static bool IsExtensionMethod(this MethodInfo method) {
            return method.RTIsDefined<System.Runtime.CompilerServices.ExtensionAttribute>(false);
        }

        ///Returns if method is Get or Set method of a property.
        public static bool IsPropertyAccessor(this MethodInfo method) {
            return method.GetMethodSpecialType() == MethodType.PropertyAccessor;
        }

        ///Returns whether the property is an indexer.
        public static bool IsIndexerProperty(this PropertyInfo property) {
            return property.GetIndexParameters().Length != 0;
        }

        ///Returns if the property is auto.
        public static bool IsAutoProperty(this PropertyInfo property) {
            if ( !property.CanRead || !property.CanWrite ) { return false; }
            var backingFieldName = "<" + property.Name + ">k__BackingField";
            return property.DeclaringType.RTGetField(backingFieldName) != null;
        }

        ///Returns the equivalent property of a method that represents an accessor method.
        public static PropertyInfo GetAccessorProperty(this MethodInfo method) {
            if ( method.IsPropertyAccessor() ) {
                return method.RTReflectedOrDeclaredType().RTGetProperty(method.Name.Substring(4));
            }
            return null;
        }

        ///Is type a supported enumerable collection?
        public static bool IsEnumerableCollection(this Type type) {
            if ( type == null ) { return false; }
            return typeof(IEnumerable).RTIsAssignableFrom(type) && ( type.RTIsGenericType() || type.RTIsArray() );
        }

        ///Returns the element type of an enumerable type.
        public static Type GetEnumerableElementType(this Type type) {
            if ( type == null ) { return null; }

            if ( !typeof(IEnumerable).RTIsAssignableFrom(type) ) {
                return null;
            }

            if ( type.HasElementType || type.RTIsArray() ) {
                return type.GetElementType();
            }

            if ( type.RTIsGenericType() ) {
                //These are not exactly correct, but serve the purpose of usage.
                var args = type.RTGetGenericArguments();
                if ( args.Length == 1 ) {
                    return args[0];
                }
                //This is special. We only support Dictionary<string, T> and always consider 1st arg to be string.
                if ( args.Length == 2 ) {
                    return args[1];
                }
            }
            /*
            var interfaces = type.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++){
                var iface = interfaces[i];
                if (!iface.RTIsGenericType()){
                    continue;
                }
                var genType = iface.GetGenericTypeDefinition();
                if (genType != typeof(IEnumerable<>)){
                    continue;
                }

                return iface.RTGetGenericArguments()[0];
            }
            */
            return null;
        }

        ///----------------------------------------------------------------------------------------------

        ///Returns the first argument parameter constraint
        public static Type GetFirstGenericParameterConstraintType(this Type type) {
            if ( type == null || !type.RTIsGenericType() ) { return null; }
            type = type.GetGenericTypeDefinition();
            var arg1 = type.GetGenericArguments().First();
            var c1 = arg1.GetGenericParameterConstraints().FirstOrDefault();
            return c1 != null ? c1 : typeof(object);
        }

        ///Returns the first argument parameter constraint
        public static Type GetFirstGenericParameterConstraintType(this MethodInfo method) {
            if ( method == null || !method.IsGenericMethod ) { return null; }
            method = method.GetGenericMethodDefinition();
            var arg1 = method.GetGenericArguments().First();
            var c1 = arg1.GetGenericParameterConstraints().FirstOrDefault();
            return c1 != null ? c1 : typeof(object);
        }

        ///----------------------------------------------------------------------------------------------

        ///Can type be made generic by using target type as argument?
        public static bool CanBeMadeGenericWith(this Type def, Type type) {
            if ( def == null || !def.RTIsGenericType() ) { return false; }
            return type.IsAllowedByGenericArgument(def.GetGenericTypeDefinition().GetGenericArguments().FirstOrDefault());
        }

        ///Can method be made generic by using target type as argument?
        public static bool CanBeMadeGenericWith(this MethodInfo def, Type type) {
            if ( def == null || !def.IsGenericMethod ) { return false; }
            return type.IsAllowedByGenericArgument(def.GetGenericMethodDefinition().GetGenericArguments().FirstOrDefault());
        }

        ///Is type allowed to be assigned to target generic argument based on that argument's constaints?
        public static bool IsAllowedByGenericArgument(this Type type, Type genericArgument) {

            if ( type == null || genericArgument == null ) { return false; }

            var constraints = genericArgument.GetGenericParameterConstraints();
            var attributes = genericArgument.GenericParameterAttributes;

            var result = true;
            for ( var i = 0; i < constraints.Length; i++ ) {
                var constraint = constraints[i];
                if ( constraint == typeof(ValueType) ) continue;
                if ( !result ) break;
                result &= constraint.RTIsAssignableFrom(type);
            }

            if ( result ) {
                if ( ( attributes & GenericParameterAttributes.DefaultConstructorConstraint ) ==
                    GenericParameterAttributes.DefaultConstructorConstraint &&
                    ( attributes & GenericParameterAttributes.NotNullableValueTypeConstraint ) !=
                    GenericParameterAttributes.NotNullableValueTypeConstraint ) {
                    var constructor = type.RTGetConstructors().FirstOrDefault(info => info.IsPublic && info.GetParameters().Length == 0);
                    if ( constructor == null ) result = false;
                }
            }

            if ( result ) {
                if ( ( attributes & GenericParameterAttributes.ReferenceTypeConstraint ) ==
                    GenericParameterAttributes.ReferenceTypeConstraint ) {
                    if ( type.RTIsValueType() ) result = false;
                }
            }

            if ( result ) {
                if ( ( attributes & GenericParameterAttributes.NotNullableValueTypeConstraint ) ==
                    GenericParameterAttributes.NotNullableValueTypeConstraint ) {
                    if ( !type.RTIsValueType() ) result = false;
                }
            }
            return result;
        }

        ///Resize array of arbitrary element type. Creates a new instance.
        public static System.Array Resize(this System.Array array, int newSize) {
            if ( array == null ) { return null; }
            var oldSize = array.Length;
            var elementType = array.GetType().GetElementType();
            var newArray = System.Array.CreateInstance(elementType, newSize);
            var preserveLength = System.Math.Min(oldSize, newSize);
            if ( preserveLength > 0 ) {
                System.Array.Copy(array, newArray, preserveLength);
            }
            return newArray;
        }

        ///----------------------------------------------------------------------------------------------

        ///Check if conversion exists from -> to type and outs the Expression able to do that.
        public static bool CanConvert(Type fromType, Type toType, out UnaryExpression exp) {
            try {
                // Throws an exception if there is no conversion fromType -> toType
                exp = Expression.Convert(Expression.Parameter(fromType, null), toType);
                return true;
            }
            catch {
                exp = null;
                return false;
            }
        }

    }
}