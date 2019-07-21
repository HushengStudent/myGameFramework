#if UNITY_EDITOR

using System;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using ParadoxNotion;

namespace ParadoxNotion.Design
{

    public static class AOTClassesGenerator
    {

        //always spoof those for shake of convenience
        static readonly List<Type> defaultSpoofTypes = new List<Type>
        {
            typeof(bool),
            typeof(float),
            typeof(int),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Keyframe),
            typeof(Bounds),
            typeof(Color),
            typeof(Rect),
            typeof(ContactPoint),
            typeof(ContactPoint2D),
            typeof(Collision),
            typeof(Collision2D),
            typeof(RaycastHit),
            typeof(RaycastHit2D),
            typeof(Ray),
            typeof(Space),
        };

        ///custom generic types to spoof were we cant use [SpoofAOT]
        static readonly List<Type> customGenericSpoof = new List<Type>
        {
            typeof(System.Action<>),
            typeof(System.Func<>),
            typeof(UnityEngine.Events.UnityAction<>),
            typeof(IList<>),
            typeof(List<>),
        };

        ///Generates AOT classes file out of preferred types list
        public static void GenerateAOTClasses(string path, Type[] targetTypes) {

            if ( string.IsNullOrEmpty(path) ) {
                return;
            }

            var spoofTypes = defaultSpoofTypes;
            spoofTypes.AddRange(targetTypes.Where(t => t.IsValueType && !spoofTypes.Contains(t)));
            spoofTypes = spoofTypes.Distinct().ToList();
            var types = ReflectionTools.GetAllTypes(true).Where(t => t.IsDefined(typeof(SpoofAOTAttribute), true)).Distinct().ToList();

            var nTypes = 0;
            var nMethods = 0;

            var sb = new StringBuilder();

            sb.AppendLine("#pragma warning disable 0219, 0168, 0612");
            sb.AppendLine("namespace ParadoxNotion.Internal{");
            sb.AppendLine();
            sb.AppendLine("\t//Auto generated classes for AOT support, where using undeclared generic classes with value types is limited. These are not actualy used but rather just declared for the compiler");
            sb.AppendLine("\tpartial class AOTDummy{");
            sb.AppendLine();
            sb.AppendLine("\t\tobject o = null;");

            sb.AppendLine("\t\t///----------------------------------------------------------------------------------------------");

            //Generic Types
            foreach ( var type in types ) {
                if ( !type.IsAbstract && type.IsGenericTypeDefinition && type.GetGenericArguments().Length == 1 ) {
                    var constrains = type.GetGenericArguments()[0].GetGenericParameterConstraints();
                    if ( constrains.Length == 0 || constrains[0].IsValueType ) {

                        if ( typeof(Delegate).IsAssignableFrom(type) ) {
                            nTypes++;
                            sb.AppendLine(string.Format("\t\tvoid {0}()", type.FriendlyName(true).Replace(".", "_").Replace("<T>", "_Delegate")) + "{");
                            foreach ( var spoofType in spoofTypes ) {
                                var a = type.FriendlyName(true).Replace("<T>", "<" + spoofType.FullName + ">").Replace("+", ".");
                                var b = "_" + type.FriendlyName(true).Replace(".", "_").Replace("<T>", "_" + spoofType.FullName.Replace(".", "_").Replace("+", "_"));
                                sb.AppendLine(string.Format("\t\t\t{0} {1} = default({2});", a, b, a));
                            }
                            sb.AppendLine("\t\t}");

                        } else {

                            foreach ( var spoofType in spoofTypes ) {
                                nTypes++;
                                var a = type.FriendlyName(true).Replace(".", "_").Replace("<T>", "_" + spoofType.FullName.Replace(".", "_").Replace("+", "_"));
                                var b = type.FriendlyName(true).Replace("<T>", "<" + spoofType.FullName + ">").Replace("+", ".") + "{}";
                                sb.AppendLine(string.Format("\t\tclass {0} : {1}", a, b));
                            }
                        }

                        sb.AppendLine();
                    }
                }
            }

            sb.AppendLine("\t\t///----------------------------------------------------------------------------------------------");

            //Generic Methods
            foreach ( var type in types ) {
                var index = 0;
                foreach ( var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly) ) {
                    if ( method.IsGenericMethodDefinition && method.GetGenericArguments().Length == 1 ) {

                        var constrains = method.GetGenericArguments()[0].GetGenericParameterConstraints();
                        if ( constrains.Length == 0 || constrains[0].IsValueType ) {

                            index++;

                            var decType = method.DeclaringType;
                            var varName = "_" + decType.FullName.Replace(".", "_");
                            sb.AppendLine(string.Format("\t\tvoid {0}_{1}_{2}()", decType.FullName.Replace(".", "_"), method.Name, index) + " {");
                            if ( !method.IsStatic ) {
                                sb.AppendLine(string.Format("\t\t\t{0} {1} = default({2});", decType.FullName, varName, decType.FullName));
                            }

                            foreach ( var spoofType in spoofTypes ) {
                                nMethods++;
                                var a = method.IsStatic ? decType.FullName : varName;
                                var b = method.Name;
                                var c = spoofType.FullName.Replace("+", ".");
                                var paramsString = "";
                                var parameters = method.GetParameters();
                                for ( var i = 0; i < parameters.Length; i++ ) {
                                    var parameter = parameters[i];
                                    var toString = parameter.ParameterType.FullName;
                                    if ( parameter.ParameterType.IsGenericParameter ) {
                                        toString = spoofType.FullName;
                                    }
                                    if ( parameter.ParameterType.IsGenericType ) {
                                        toString = parameter.ParameterType.FriendlyName(true).Replace("<T>", "<" + spoofType.FullName + ">");
                                        toString = toString.Replace("[[T]]", "");
                                    }
                                    toString = toString.Replace("+", ".");
                                    paramsString += string.Format("({0})o", toString);
                                    if ( i < parameters.Length - 1 ) {
                                        paramsString += ", ";
                                    }
                                }
                                var d = paramsString;
                                sb.AppendLine(string.Format("\t\t\t{0}.{1}<{2}>( {3} );", a, b, c, d));
                            }

                            sb.AppendLine("\t\t}");
                            sb.AppendLine();
                        }
                    }
                }
            }

            sb.AppendLine("\t\t///----------------------------------------------------------------------------------------------");

            //custom stuff
            sb.AppendLine("\t\tvoid CustomSpoof(){");
            foreach ( var spoofType in spoofTypes ) {
                var sName = spoofType.FullName.Replace("+", ".");
                var fName = spoofType.FullName.Replace(".", "_").Replace("+", "_");
                foreach ( var genericType in customGenericSpoof ) {
                    nTypes++;
                    var a = genericType.FriendlyName(true).Replace("<T>", "<" + sName + ">");
                    var b = genericType.FriendlyName(true).Replace(".", "_").Replace("<T>", "_") + fName;
                    sb.AppendLine(string.Format("\t\t\t{0} {1};", a, b));
                }
                nTypes++;
                sb.AppendLine(string.Format("\t\t\tSystem.Collections.Generic.IDictionary<System.String, {0}> IDict_{1};", sName, fName));
                sb.AppendLine(string.Format("\t\t\tSystem.Collections.Generic.Dictionary<System.String, {0}> Dict_{1};", sName, fName));
                sb.AppendLine("\t\t\t///------");
            }
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine(string.Format("//{0} Types | {1} Methods spoofed", nTypes, nMethods));
            sb.AppendLine("#pragma warning restore 0219, 0168, 0612");

            File.WriteAllText(path, sb.ToString());
        }

        ///Generates a link.xml file out of preferred types list
        public static void GenerateLinkXML(string path, Type[] targetTypes) {

            if ( string.IsNullOrEmpty(path) ) {
                return;
            }

            var spoofTypes = defaultSpoofTypes;
            spoofTypes.AddRange(targetTypes);
            var pairs = new Dictionary<string, List<Type>>();
            foreach ( var type in spoofTypes ) {
                var asmName = type.Assembly.GetName().Name;
                if ( !pairs.ContainsKey(asmName) ) {
                    pairs[asmName] = new List<Type>();
                }
                pairs[asmName].Add(type);
            }

            var sb = new StringBuilder();
            sb.AppendLine("<linker>");

            sb.AppendLine("\t<assembly fullname=\"Assembly-CSharp\" preserve=\"all\">");
            sb.AppendLine("\t</assembly>");

            ///get assembly from a common paradoxnotion *runtime* type
            var paradoxAsmName = typeof(ParadoxNotion.Serialization.JSONSerializer).Assembly.GetName().Name;
            sb.AppendLine(string.Format("\t<assembly fullname=\"{0}\" preserve=\"all\">", paradoxAsmName));
            sb.AppendLine("\t</assembly>");

            foreach ( var pair in pairs ) {
                sb.AppendLine(string.Format("\t<assembly fullname=\"{0}\">", pair.Key));
                foreach ( var type in pair.Value ) {
                    sb.AppendLine("\t\t<type fullname=\"" + type.FullName + "\" preserve=\"all\"/>");
                }
                sb.AppendLine("\t</assembly>");
            }
            sb.AppendLine("</linker>");

            File.WriteAllText(path, sb.ToString());
        }
    }
}

#endif