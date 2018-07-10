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
            InitDict(path);
            BehaviorTree tree = new BehaviorTree(null, entity);
            return tree;
        }

        private static void InitDict(string path)
        {
            _behaviorDict.Clear();
            _connectionDict.Clear();
            TextAsset json = Resources.Load<TextAsset>(path);
            string content = json.text.Replace("\r", "").Replace("\n", "");
            Hashtable table = MiniJsonExtensions.hashtableFromJson(content);
            ArrayList nodeList = table["nodes"] as ArrayList;
            ArrayList connectionList = table["connections"] as ArrayList;
            for (int i = 0; i < nodeList.Count; i++)
            {
                Hashtable nodeTable = nodeList[i] as Hashtable;
                int id = (int)nodeTable["$id"];
                AbsBehavior absBehavior = CreateBehavior(nodeTable);
                _behaviorDict[id] = absBehavior;
            }
            for(int i = 0;i< connectionList.Count; i++)
            {

            }
        }

        private static AbsBehavior CreateBehavior(Hashtable table)
        {
            AbsBehavior behavior = null;
            string type = table["$type"].ToString();
            switch (type)
            {
                //顺序执行节点;
                case "NodeCanvas.BehaviourTrees.BTSequence":
                    behavior = new BTSequence(table);
                    break;
                //随机执行节点;
                case "NodeCanvas.BehaviourTrees.BTRandom":
                    behavior = new BTRandom(table);
                    break;
                //选择执行节点;
                case "NodeCanvas.BehaviourTrees.BTSelector":
                    behavior = new BTSelector(table);
                    break;
                //等级条件执行节点;
                case "NodeCanvas.BehaviourTrees.BTLevel":
                    behavior = new BTLevel(table);
                    break;
                //日志执行节点;
                case "NodeCanvas.BehaviourTrees.BTLog":
                    behavior = new BTLog(table);
                    break;
                default:
                    break;
            }
            return behavior;
        }
    }
}