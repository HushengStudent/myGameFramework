/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/24 01:43:40
** desc:  行为树工厂;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class BehaviorTreeFactory
    {
        private static Dictionary<int, AbsBehavior> _behaviorDict = new Dictionary<int, AbsBehavior>();
        private static Dictionary<int, List<int>> _connectionDict = new Dictionary<int, List<int>>();

        public static BehaviorTree CreateBehaviorTree(BaseEntity entity, string path)
        {
            ReadConfig(path);
            BehaviorTree tree = new BehaviorTree(null, entity);
            return tree;
        }

        private static void ReadConfig(string path)
        {
            _behaviorDict.Clear();
            _connectionDict.Clear();
            TextAsset json = Resources.Load<TextAsset>(path);
            string content = json.text.Replace("\r", "").Replace("\n", "");
            Hashtable table = MiniJsonExtensions.hashtableFromJson(content);
            object nodes = table["nodes"];
        }

        private static AbsBehavior CreateBehavior(string type)
        {
            AbsBehavior behavior = null;
            string[] str = type.Split(".".ToCharArray());
            if (str.Length < 3)
            {
                LogUtil.LogUtility.PrintError(string.Format("[BehaviorTreeFactory]create Behavior error,type is {0}", type));
                return behavior;
            }
            switch (str[2])
            {
                case "BTLog":
                    break;
                case "BTSequence":
                    break;
                case "BTRandom":
                    break;
                case "BTSelector":
                    break;
                default:
                    break;
            }
            return behavior;
        }
    }
}