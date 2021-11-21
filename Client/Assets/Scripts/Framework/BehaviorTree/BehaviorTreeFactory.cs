/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/24 01:43:40
** desc:  行为树工厂;
*********************************************************************************/

using Framework.ECSModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class BehaviorTreeFactory
    {
        private static AbsBehavior _rootBehavior = null;
        private static Dictionary<int, AbsBehavior> _behaviorDict = new Dictionary<int, AbsBehavior>();
        private static Dictionary<int, List<int>> _connectionDict = new Dictionary<int, List<int>>();

        public static BehaviorTree CreateBehaviorTree(AbsEntity entity, string path)
        {
            InitDict(path);
            if (_rootBehavior == null)
            {
                LogHelper.PrintError("[BehaviorTreeFactory]Root Behavior is null!");
                return null;
            }
            GenerateConnect(new List<AbsBehavior>() { _rootBehavior });
            var tree = new BehaviorTree(_rootBehavior, entity);
            _rootBehavior = null;
            _behaviorDict.Clear();
            _connectionDict.Clear();
            return tree;
        }

        private static void InitDict(string path)
        {
            _rootBehavior = null;
            _behaviorDict.Clear();
            _connectionDict.Clear();
            var json = Resources.Load<TextAsset>(path);
            var content = json.text.Replace("\r", "").Replace("\n", "");
            var table = MiniJsonExtensions.hashtableFromJson(content);
            var nodeList = table["nodes"] as ArrayList;
            var connectionList = table["connections"] as ArrayList;
            for (var i = 0; i < nodeList.Count; i++)
            {
                var nodeTable = nodeList[i] as Hashtable;
                if (int.TryParse(nodeTable["$id"].ToString(), out var id))
                {
                    var absBehavior = CreateBehavior(nodeTable, id);
                    _behaviorDict[id] = absBehavior;
                    if (_rootBehavior == null)
                    {
                        _rootBehavior = absBehavior;
                    }
                    else
                    {
                        if (absBehavior.Id < _rootBehavior.Id)
                        {
                            _rootBehavior = absBehavior;
                        }
                    }
                }
                else
                {
                    LogHelper.PrintError("[BehaviorTreeFactory]try get node id error!");
                }
            }
            for (var i = 0; i < connectionList.Count; i++)
            {
                var connectionTable = connectionList[i] as Hashtable;
                var scurceNode = connectionTable["_sourceNode"] as Hashtable;
                var targetNode = connectionTable["_targetNode"] as Hashtable;
                if (int.TryParse(scurceNode["$ref"].ToString(), out var source)
                    && int.TryParse(targetNode["$ref"].ToString(), out var target))
                {
                    if (!_connectionDict.TryGetValue(source, out var list))
                    {
                        _connectionDict[source] = new List<int>();
                        list = _connectionDict[source];
                    }
                    list.Add(target);
                }
                else
                {
                    LogHelper.PrintError("[BehaviorTreeFactory]try get source id and target id error!");
                }
            }
        }

        private static void GenerateConnect(List<AbsBehavior> list)
        {
            var nextList = new List<AbsBehavior>();
            AbsBehavior target;
            for (var i = 0; i < list.Count; i++)
            {
                target = list[i];
                var id = target.Id;
                if (!_connectionDict.TryGetValue(id, out var connectList))
                {
                    continue;
                }
                var sonList = new List<AbsBehavior>();
                for (var j = 0; j < connectList.Count; j++)
                {
                    var sonId = connectList[j];
                    if (!_behaviorDict.TryGetValue(sonId, out var son))
                    {
                        continue;
                    }
                    if (son != null)
                    {
                        sonList.Add(son);
                    }
                }
                if (target.IsComposite)
                {
                    var composite = target as AbsComposite;
                    if (sonList.Count < 1)
                    {
                        composite.Serialize(null);
                    }
                    else
                    {
                        composite.Serialize(sonList);
                        nextList.AddRange(sonList);
                    }
                }
                else
                {
                    var decorator = target as AbsDecorator;
                    if (sonList.Count < 1)
                    {
                        decorator.Serialize(null);
                    }
                    else
                    {
                        decorator.Serialize(sonList[0]);
                        nextList.Add(sonList[0]);
                    }
                }
            }
            if (nextList.Count > 0)
            {
                GenerateConnect(nextList);
            }
        }

        private static AbsBehavior CreateBehavior(Hashtable table, int id)
        {
            AbsBehavior behavior = null;
            var type = table["$type"].ToString();
            var str = type.Split(".".ToCharArray());
            if (3 == str.Length)
            {
                var name = "Framework." + str[2];
                var target = Type.GetType(name);
                behavior = Activator.CreateInstance(target, table) as AbsBehavior;
            }
            if (behavior != null)
            {
                behavior.Id = id;
            }
            return behavior;
        }
    }
}