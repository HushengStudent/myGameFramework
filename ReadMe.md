### myGameFramework
#### My game framework include client server and tools.

software：Unity2019.4.14/visual studio2017.

#### Client：Unity2019.4.14
    Module：
    (1)单例.(Finish)
    (2)LOG.(Finish)
    (3)定时器.(Finish)
    (4)ECS.(Finish)
    (5)资源管理：AssetBundle、对象池、资源加载/卸载.(Finish)
    (6)协程.(Finish)
    (7)事件系统.(Finish)
    (8)网络：Lua、C#.(Finish)
    (9)SDK管理.(Finish)
    (10)场景管理.(Finish)
    (11)配置表.(Finish)
    (12)实时阴影.(Finish)
    (13)Timeline.(TODO)
    (14)Wwise.(TODO)
    (15)KCP.(TODO)
    (16)换装/染色.(TODO)
    (17)寻路.(Finish)
    (18)技能.(TODO)
    (19)Buff.(Finish)
    (20)AI：FSM、行为树.(Finish)
    (21)网络同步机制：帧同步、状态同步.(TODO)
    (22)热更新.(TODO)
    (23)Jenkins.(TODO)
    (24)本地化.(TODO)
    (25)编辑器：AI(行为树)、战斗(技能)、场景、打包.(TODO)
    (26)Lua：tolua、class、profiler、工具、方法、定时器、事件系统.(Finish)
    (27)UI：滑动列表、图文混排、超链、HUD、飘字、物品掉落.(TODO)
    (28)项目优化.(TODO)
    (29)实用工具.(TODO)
	
    Reference：
    (1)tolua：(https://github.com/topameng/tolua).
    (2)protoc-gen-lua：(https://github.com/topameng/protoc-gen-lua).
    (3)AssetBundle：(https://github.com/HushengStudent/myAssetBundleTools).
    (4)protobuf-net-r668：(https://github.com/mdavid/protobuf-net).
    (5)ObjectPool：(https://github.com/HushengStudent/ugui).
    (6)Mixed Text and Graphics：(https://github.com/zouchunyi/EmojiText).
    (7)Table：(https://github.com/Ribosome2/ExcelUtilityWith-ExcelReader).
    (8)NetWork：(https://github.com/EllanJiang/GameFramework).
    (9)Lua Profiler：(https://github.com/yaukeywang/LuaMemorySnapshotDump).
    (10)Navigation：(https://github.com/LingJiJian/NavmeshExport).
	
    Third Party:
    (1)Anima2D.
    (2)AssetBundle Browser.
    (3)Astar Pathfinding Project.
    (4)Build Report.
    (5)Cinemachine.
	(6)DOTween.
	(7)Effect Examples.
	(8)Fast Shadow Projector.
	(9)Node Editor.
    (10)FlowCanvas/NodeCanvas.
	(11)PostProcessing.
	(12)TextMesh Pro.
	(13)TexturePacker Importer.
    (14)More Effective Coroutine.
    (15)Behavior Designer.
	(16)Unity Particle Pack 5.x.
	(17)Unity Logs Viewer.
	(18)Odin Inspector and Serializer 2.1.	
	
    环境变量配置:
    (1)myGameFramework:(工程路径)如C:/myGameFramework/
    (2)myGameFramework_protoc:(protoc.exe路径)如c:/protobuf-3.0.0/src/
	
#### Server：Visual Studio 2017
    Reference：
    (1)protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net).
	
#### Tools：相关工具目录
    (1)protoc-gen-lua.
    (2)protoc-gen-csharp.
	
#### 更新记录
    (1)branch_v1.0.
    (2)branch_v2.0(更新tolua版本).
    (3)branch_v3.0(资源管理重构(异步加载优化;对象池)).
    (4)branch_v4.0(更新到Unity2019).