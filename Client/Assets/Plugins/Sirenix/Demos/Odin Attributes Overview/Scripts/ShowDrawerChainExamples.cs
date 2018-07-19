namespace Sirenix.OdinInspector.Demos
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ShowDrawerChainExamples : MonoBehaviour
    {
        [ShowDrawerChain]
        public List<int> SomeList;

        [ShowDrawerChain]
        public GameObject SomeObject;

        [Range(0, 10)]
        [ShowDrawerChain]
        public float SomeRange;
    }
}