#if UNITY_EDITOR

//Except where stated all code in this file is copyright of Jim Blackler, 2008.
//jimblackler@gmail.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ParadoxNotion.Design
{

    /// Utility class to provide documentation for various types where available with the assembly
    public static class DocsByReflection {
        
        private static Dictionary<MemberInfo, XmlElement> cachedElements = new Dictionary<MemberInfo, XmlElement>();
        private static Dictionary<MemberInfo, string> cachedSummaries = new Dictionary<MemberInfo, string>();

        ///Returns a cached XML elements for member
        public static XmlElement GetXmlElementForMember(MemberInfo memberInfo){

            if (memberInfo is MethodInfo){
                var method = (MethodInfo)memberInfo;
                if (method.IsPropertyAccessor()){
                    memberInfo = method.GetAccessorProperty();
                }
            }

            if (memberInfo == null){
                return null;
            }

            XmlElement element;
            if (cachedElements.TryGetValue(memberInfo, out element)){
                return element;
            }

            if (memberInfo is MethodInfo){
                element = XMLFromMember( (MethodInfo)memberInfo );
            } else if (memberInfo is Type){
                element = XMLFromType( (Type)memberInfo );
            } else {
                element = XMLFromMember(memberInfo);
            }

            return cachedElements[memberInfo] = element;
        }

        ///Returns a chached summary info for member
        public static string GetMemberSummary(MemberInfo memberInfo){
            string result;
            if (cachedSummaries.TryGetValue(memberInfo, out result)){
                return result;
            }
            var element = GetXmlElementForMember(memberInfo);
            return cachedSummaries[memberInfo] = (element != null? element["summary"].InnerText.Trim() : "No Documentation Found");
        }

        ///Returns a chached return info for method
        public static string GetMethodReturn(MethodBase method){
            var element = GetXmlElementForMember(method);
            return element != null? element["returns"].InnerText.Trim() : null;
        }

        ///Returns a semi-chached method parameter info for method
        public static string GetMethodParameter(MethodBase method, ParameterInfo parameter){ return GetMethodParameter(method, parameter.Name); }
        ///Returns a semi-chached method parameter info for method
        public static string GetMethodParameter(MethodBase method, string parameterName){
            var methodElement = GetXmlElementForMember(method);
            if (methodElement != null){
                foreach(var element in methodElement){
                    var xmlElement = element as XmlElement;
                    if (xmlElement == null){ continue; }
                    var found = xmlElement.Attributes["name"];
                    if (found != null && found.Value == parameterName){
                        return xmlElement.InnerText.Trim();
                    }
                }                
            }
            return null;
        }

        ///----------------------------------------------------------------------------------------------

        /// <summary>
        /// Provides the documentation comments for a specific method
        /// </summary>
        /// <param name="methodInfo">The MethodInfo (reflection data ) of the member to find documentation for</param>
        /// <returns>The XML fragment describing the method</returns>
        public static XmlElement XMLFromMember(MethodInfo methodInfo)
        {
            // Calculate the parameter string as this is in the member name in the XML
            string parametersString = "";
            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                if (parametersString.Length > 0)
                {
                    parametersString += ",";
                }

                parametersString += parameterInfo.ParameterType.FullName;
            }

            //AL: 15.04.2008 ==> BUG-FIX remove �()� if parametersString is empty
            if (parametersString.Length > 0){
                return XMLFromName(methodInfo.DeclaringType, 'M', methodInfo.Name + "(" + parametersString + ")");
            } else {
                return XMLFromName(methodInfo.DeclaringType, 'M', methodInfo.Name);
            }
        }

        /// <summary>
        /// Provides the documentation comments for a specific member
        /// </summary>
        /// <param name="memberInfo">The MemberInfo (reflection data) or the member to find documentation for</param>
        /// <returns>The XML fragment describing the member</returns>
        public static XmlElement XMLFromMember(MemberInfo memberInfo)
        {
            // First character [0] of member type is prefix character in the name in the XML
            return XMLFromName(memberInfo.DeclaringType, memberInfo.MemberType.ToString()[0], memberInfo.Name);
        }

        /// <summary>
        /// Provides the documentation comments for a specific type
        /// </summary>
        /// <param name="type">Type to find the documentation for</param>
        /// <returns>The XML fragment that describes the type</returns>
        public static XmlElement XMLFromType(Type type)
        {
            // Prefix in type names is T
            return XMLFromName(type, 'T', "");
        }

        /// <summary>
        /// Obtains the XML Element that describes a reflection element by searching the 
        /// members for a member that has a name that describes the element.
        /// </summary>
        /// <param name="type">The type or parent type, used to fetch the assembly</param>
        /// <param name="prefix">The prefix as seen in the name attribute in the documentation XML</param>
        /// <param name="name">Where relevant, the full name qualifier for the element</param>
        /// <returns>The member that has a name that describes the specified reflection element</returns>
        private static XmlElement XMLFromName(Type type, char prefix, string name)
        {
            string fullName;

            if (String.IsNullOrEmpty(name))
            {
                fullName = prefix + ":" + type.FullName;
            }
            else
            {
                fullName = prefix + ":" + type.FullName + "." + name;
            }

            XmlDocument xmlDocument = XMLFromAssembly(type.Assembly);
            if (xmlDocument == null){
                return null;
            }

            foreach (var element in xmlDocument["doc"]["members"])
            {
                var xmlElement = element as XmlElement;
                if (xmlElement != null){
                    if (xmlElement.Attributes["name"] == null){
                        continue;
                    }

                    if (xmlElement.Attributes["name"].Value.Equals(fullName))
                    {
                        return xmlElement;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// A cache used to remember Xml documentation for assemblies
        /// </summary>
        static Dictionary<Assembly, XmlDocument> cache = new Dictionary<Assembly, XmlDocument>();

        /// <summary>
        /// A cache used to store failure exceptions for assembly lookups
        /// </summary>
        static Dictionary<Assembly, Exception> failCache = new Dictionary<Assembly, Exception>();

        /// <summary>
        /// Obtains the documentation file for the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        /// <remarks>This version uses a cache to preserve the assemblies, so that 
        /// the XML file is not loaded and parsed on every single lookup</remarks>
        public static XmlDocument XMLFromAssembly(Assembly assembly)
        {
            if (failCache.ContainsKey(assembly))
            {
                return null;
            }

            try
            {
                if (!cache.ContainsKey(assembly))
                {
                    // load the docuemnt into the cache
                    cache[assembly] = XMLFromAssemblyNonCached(assembly);
                }

                return cache[assembly];
            }
            catch (Exception exception)
            {
                failCache[assembly] = exception;
                return null;
            }
        }

        /// <summary>
        /// Loads and parses the documentation file for the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        private static XmlDocument XMLFromAssemblyNonCached(Assembly assembly)
        {
            string assemblyFilename = assembly.CodeBase;

            const string prefix = "file:///";

            if (assemblyFilename.StartsWith(prefix))
            {
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(Path.ChangeExtension(assemblyFilename.Substring(prefix.Length), ".xml"));
                }
                catch
                {
                    return null;
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(streamReader);
                return xmlDocument;
            }
            else
            {
                return null;
            }
        }
    }
}

#endif