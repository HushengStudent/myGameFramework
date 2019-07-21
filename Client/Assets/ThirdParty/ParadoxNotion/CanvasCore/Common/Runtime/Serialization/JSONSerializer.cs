using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using UnityEngine;

namespace ParadoxNotion.Serialization
{

    ///High-Level API. Serializes/Deserializes to/from JSON with 'FullSerializer'
    public static class JSONSerializer
    {

#if UNITY_EDITOR //this is used to avoid calling Unity API in serialization for the editor
        [UnityEditor.InitializeOnLoadMethod]
        static void Init() {
            applicationPlaying = false; //set to false since this is always called in editor start where application is not playing.
#if UNITY_2017_2_OR_NEWER
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeChanged;
#else
            UnityEditor.EditorApplication.playmodeStateChanged += PlayModeChanged;
#endif
        }
        static void PlayModeChanged
        (
#if UNITY_2017_2_OR_NEWER
            UnityEditor.PlayModeStateChange state
#endif
        ) { applicationPlaying = Application.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode; }
#endif

        public static bool applicationPlaying { get; private set; }
        private static Dictionary<string, fsData> cache;
        private static object serializerLock;
        private static fsSerializer serializer;

        static JSONSerializer() {
            //initialize to true since Init is editor only and in runtime, application is of course always playing.
            applicationPlaying = true;
            cache = new Dictionary<string, fsData>();
            serializerLock = new object();
            serializer = new fsSerializer();
        }

        ///----------------------------------------------------------------------------------------------

        ///Serialize to json
        public static string Serialize(Type type, object instance, bool pretyJson = false, List<UnityEngine.Object> objectReferences = null) {

            lock ( serializerLock ) {
                //set the objectReferences context
                if ( objectReferences != null ) {
                    objectReferences.Clear(); //we clear the list since it will be populated by the converter.
                    serializer.Context.Set<List<UnityEngine.Object>>(objectReferences);
                }

                //serialize the data
                fsData data;
                //We override the UnityObject converter if we serialize a UnityObject directly.
                //UnityObject converter will still be used for every serialized property found within the object though.
                var overrideConverterType = typeof(UnityEngine.Object).RTIsAssignableFrom(type) ? typeof(fsReflectedConverter) : null;
                serializer.TrySerialize(type, overrideConverterType, instance, out data).AssertSuccess();

                return fsJsonPrinter.ToJson(data, pretyJson);
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///Deserialize from json
        public static T Deserialize<T>(string json, List<UnityEngine.Object> objectReferences = null) {
            return (T)Internal_Deserialize(typeof(T), json, objectReferences, null);
        }

        ///Deserialize from json
        public static object Deserialize(Type type, string json, List<UnityEngine.Object> objectReferences = null) {
            return Internal_Deserialize(type, json, objectReferences, null);
        }

        ///Deserialize overwrite from json
        public static T DeserializeOverwrite<T>(T instance, string json, List<UnityEngine.Object> objectReferences = null) {
            return (T)Internal_Deserialize(typeof(T), json, objectReferences, instance);
        }

        ///Deserialize overwrite from json
        public static object DeserializeOverwrite(object instance, string json, List<UnityEngine.Object> objectReferences = null) {
            return Internal_Deserialize(instance.GetType(), json, objectReferences, instance);
        }

        ///Deserialize from json
        public static object Internal_Deserialize(Type type, string json, List<UnityEngine.Object> objectReferences, object instance) {

            lock ( serializerLock ) {
                //set the objectReferences context from where unity references will read
                if ( objectReferences != null ) {
                    serializer.Context.Set<List<UnityEngine.Object>>(objectReferences);
                }

                fsData data = null;
                cache.TryGetValue(json, out data);
                if ( data == null ) {
                    data = fsJsonParser.Parse(json);
                    cache[json] = data;
                }

                //deserialize the data
                //We override the UnityObject converter if we deserialize a UnityObject directly.
                //UnityObject converter will still be used for every serialized property found within the object though.
                var overrideConverterType = typeof(UnityEngine.Object).RTIsAssignableFrom(type) ? typeof(fsReflectedConverter) : null;
                serializer.TryDeserialize(data, type, overrideConverterType, ref instance).AssertSuccess();

                return instance;
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///Deep clone an object
        public static T Clone<T>(T original, List<UnityEngine.Object> objectReferences = null) {
            return (T)Clone((object)original, objectReferences);
        }

        ///Deep clone an object
        public static object Clone(object original, List<UnityEngine.Object> objectReferences = null) {
            var type = original.GetType();
            var json = Serialize(type, original, false, objectReferences);
            return Deserialize(type, json, objectReferences);
        }

        ///Writes json to prety json in a temp file and opens it
        public static void ShowData(string json, string fileName = "") {
            var prettyJson = PrettifyJson(json);
            var dataPath = Path.GetTempPath() + ( string.IsNullOrEmpty(fileName) ? Guid.NewGuid().ToString() : fileName ) + ".json";
            File.WriteAllText(dataPath, prettyJson);
            Process.Start(dataPath);
        }

        ///Prettify existing json string
        public static string PrettifyJson(string json) {
            return fsJsonPrinter.PrettyJson(fsJsonParser.Parse(json));
        }
    }
}