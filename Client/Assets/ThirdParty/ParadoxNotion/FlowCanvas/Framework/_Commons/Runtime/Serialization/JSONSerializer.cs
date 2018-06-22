using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using UnityEngine;

namespace ParadoxNotion.Serialization{

    ///Serializes/Deserializes to/from JSON with 'FullSerializer'
    public static class JSONSerializer {
        
#if UNITY_EDITOR //this is used to avoid calling Unity API in serialization for the editor
        [UnityEditor.InitializeOnLoadMethod]
        static void Init(){
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

        private static Dictionary<string, fsData> cache = new Dictionary<string, fsData>();
        private static object serializerLock = new object();
        private static fsSerializer serializer = new fsSerializer();
        private static bool init = false;

        //initialize to true since StartUp is editor only and in runtime application is of course always playing.
        public static bool applicationPlaying = true;

        ///Serialize to json
        public static string Serialize(Type type, object value, bool pretyJson = false, List<UnityEngine.Object> objectReferences = null) {

            lock (serializerLock)
            {
                if (!init){
                    serializer.AddConverter(new fsUnityObjectConverter());
                    init = true;
                }

                //set the objectReferences context
                if (objectReferences != null){
                    objectReferences.Clear(); //we clear the list since it will be populated by the converter.
                    serializer.Context.Set<List<UnityEngine.Object>>(objectReferences);
                }

                //serialize the data
                fsData data;
                //We override the UnityObject converter if we serialize a UnityObject directly.
                //UnityObject converter will still be used for every serialized property found within the object though.
                var overrideConverterType = typeof(UnityEngine.Object).RTIsAssignableFrom(type)? typeof(fsReflectedConverter) : null;
                serializer.TrySerialize(type, overrideConverterType, value, out data).AssertSuccess();

                //print data to json
                if (pretyJson){
                    return fsJsonPrinter.PrettyJson(data);
                }
                return fsJsonPrinter.CompressedJson(data);
            }
        }

        ///Deserialize generic
        public static T Deserialize<T>(string serializedState, List<UnityEngine.Object> objectReferences = null, T deserialized = default(T)){
            return (T)Deserialize(typeof(T), serializedState, objectReferences, deserialized);
        }

        ///Deserialize from json
        public static object Deserialize(Type type, string serializedState, List<UnityEngine.Object> objectReferences = null, object deserialized = null) {

            lock (serializerLock)
            {
                if (!init){
                    serializer.AddConverter(new fsUnityObjectConverter());
                    init = true;
                }

                //set the objectReferences context
                if (objectReferences != null){
                    serializer.Context.Set<List<UnityEngine.Object>>(objectReferences);
                }

                fsData data = null;
                cache.TryGetValue(serializedState, out data);
                if (data == null){
                    data = fsJsonParser.Parse(serializedState);
                    cache[serializedState] = data;                
                }

                //deserialize the data
                //We override the UnityObject converter if we deserialize a UnityObject directly.
                //UnityObject converter will still be used for every serialized property found within the object though.
                var overrideConverterType = typeof(UnityEngine.Object).RTIsAssignableFrom(type)? typeof(fsReflectedConverter) : null;
                serializer.TryDeserialize(data, type, overrideConverterType, ref deserialized).AssertSuccess();

                return deserialized;
            }
        }

        ///Deep clone an object
        public static T Clone<T>(T original, List<UnityEngine.Object> objectReferences = null){
            return (T)Clone( (object)original, objectReferences );
        }

        ///Deep clone an object
        public static object Clone(object original, List<UnityEngine.Object> objectReferences = null){
            var type = original.GetType();
            var json = Serialize(type, original, false, objectReferences);
            return Deserialize(type, json, objectReferences);
        }

        ///writes json to prety json in a temp file and opens it
        public static void ShowData(string json, string name = ""){
            var prettyJson = fsJsonPrinter.PrettyJson(fsJsonParser.Parse(json));
            var dataPath = Path.GetTempPath() + (string.IsNullOrEmpty(name)? Guid.NewGuid().ToString() : name) + ".json";
            File.WriteAllText(dataPath, prettyJson);
            Process.Start(dataPath);
        }

    }
}