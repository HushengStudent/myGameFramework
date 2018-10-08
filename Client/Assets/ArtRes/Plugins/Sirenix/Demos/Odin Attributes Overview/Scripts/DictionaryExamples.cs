namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System.Collections.Generic;

    public class DictionaryExamples : SerializedMonoBehaviour
    {
        [InfoBox("In order to serialize dictionaries, all we need to do is to inherit our class from SerializedMonoBehaviour.")]
        public Dictionary<int, Material> IntMaterialLookup;

        public Dictionary<string, string> StringStringDictionary;

        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<string, List<int>> StringListDictionary;

        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
        public Dictionary<SomeEnum, MyCustomType> EnumObjectLookup;

        public class MyCustomType
        {
            public int SomeMember;
            public Quaternion SomeRotation;
            public GameObject SomePrefab;
        }

        public enum SomeEnum
        {
            First, Second, Third, Fourth, AndSoOn
        }
    }
}