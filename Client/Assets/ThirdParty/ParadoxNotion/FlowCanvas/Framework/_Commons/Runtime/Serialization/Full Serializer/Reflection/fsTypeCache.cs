using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal {
    /// <summary>
    /// Caches type name to type lookups. Type lookups occur in all loaded assemblies.
    /// </summary>
    public static class fsTypeCache {

        //PARADOXNOTION ADDITION
        //Just use ReflectionTools.GetType implementation to avoid duplicate code here.
        public static Type GetType(string name) { return GetType(name, false, null); }
        public static Type GetType(string name, Type fallbackAssignable) { return GetType(name, true, fallbackAssignable); }
        static Type GetType(string name, bool fallbackNoNamespace, Type fallbackAssignable) {
            return ReflectionTools.GetType(name, fallbackNoNamespace, fallbackAssignable);
/*
            if (string.IsNullOrEmpty(name)) { return null; }
            Type type;
            if (_cachedTypes.TryGetValue(name, out type) == false) {
                if (TryIndirectTypeLookup(name, out type) == false) {}
                _cachedTypes[name] = type;
            }
            return type;
*/
        }

/*
        /// <summary>
        /// Cache from fully qualified type name to type instances.
        /// </summary>
        // TODO: verify that type names will be unique
        private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Assemblies indexed by their name.
        /// </summary>
        private static Dictionary<string, Assembly> _assembliesByName;

        /// <summary>
        /// A list of assemblies, by index.
        /// </summary>
        private static List<Assembly> _assembliesByIndex;

        static fsTypeCache() {
            // Setup assembly references so searching and the like resolves correctly.
            _assembliesByName = new Dictionary<string, Assembly>();
            _assembliesByIndex = new List<Assembly>();

#if (!UNITY_EDITOR && UNITY_METRO && !ENABLE_IL2CPP) // no AppDomain on WinRT
            var assembly = typeof(object).GetTypeInfo().Assembly;
            _assembliesByName[assembly.FullName] = assembly;
            _assembliesByIndex.Add(assembly);
            // _assembliesByIndex[0] = assembly;
#else
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                _assembliesByName[assembly.FullName] = assembly;
                _assembliesByIndex.Add(assembly);
            }
#endif

            _cachedTypes = new Dictionary<string, Type>();

#if !(UNITY_WP8  || UNITY_METRO) // AssemblyLoad events are not supported on these platforms
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
#endif
        }

#if !(UNITY_WP8 || UNITY_METRO) // AssemblyLoad events are not supported on these platforms
        private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args) {
            _assembliesByName[args.LoadedAssembly.FullName] = args.LoadedAssembly;
            _assembliesByIndex.Add(args.LoadedAssembly);

            _cachedTypes = new Dictionary<string, Type>();
        }
#endif
/*
        /// <summary>
        /// Does a direct lookup for the given type, ie, goes directly to the assembly identified by
        /// assembly name and finds it there.
        /// </summary>
        /// <param name="assemblyName">The assembly to find the type in.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="type">The found type.</param>
        /// <returns>True if the type was found, false otherwise.</returns>
        private static bool TryDirectTypeLookup(string assemblyName, string typeName, out Type type) {
            if (assemblyName != null) {
                Assembly assembly;
                if (_assembliesByName.TryGetValue(assemblyName, out assembly)) {
                    type = assembly.GetType(typeName, false);
                    return type != null;
                }
            }

            type = null;
            return false;
        }

        /// <summary>
        /// Tries to do an indirect type lookup by scanning through every loaded assembly until the
        /// type is found in one of them.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="type">The found type.</param>
        /// <returns>True if the type was found, false otherwise.</returns>
        private static bool TryIndirectTypeLookup(string typeName, out Type type) {
            // There used to be a foreach loop through the value keys of the _assembliesByName
            // dictionary. However, during that loop assembly loads could occur, causing an
            // OutOfSync exception. To resolve that, we just iterate through the assemblies by
            // index.

            int i = 0;
            while (i < _assembliesByIndex.Count) {
                Assembly assembly = _assembliesByIndex[i];

                // try GetType; should be fast
                type = assembly.GetType(typeName);
                if (type != null) {
                    return true;
                }

                // private type or similar; go through the slow path and check every type's full
                // name
                foreach (var foundType in assembly.GetTypes()) {
                    if (foundType.FullName == typeName) {
                        type = foundType;
                        return true;
                    }
                }

                ++i;
            }

            type = null;
            return false;
        }

        /// <summary>
        /// Removes any cached type lookup results.
        /// </summary>
        public static void Reset() {
            _cachedTypes = new Dictionary<string, Type>();
        }
*/

    }
}

