### myGameFramework
#### This is a game framework include client server and some tools.

software：Unity2017.4.25/visual studio2017.

#### Client：客户端工程使用Unity2017.4.16创建
    Module：
    (1)资源管理：AB打包、资源加载、资源卸载.(Finish)
    (2)tolua接入.(Finish)
    (3)Log封装.(Finish)
    (4)Lua UI模块：Lua类、工具方法、定时器、事件系统.(Finish)
    (5)Lua Profiler.(Finish)
    (6)ECS模式.(Finish)
    (7)通用单例.(Finish)
    (8)协程封装.(Finish)
    (9)事件系统.(Finish)
    (10)定时器.(Finish)
    (11)网络框架：Lua、C#.(Finish)
    (12)对象池.(Finish)
    (13)SDK封装.(Finish)
    (14)场景管理.(Finish)
    (15)UI：UI复用、图文混排、超链、HUD、飘字、物品掉落.(TODO)
    (16)配置表工具.(Finish)
    (17)实时阴影.(Finish)
    (18)Timeline.(TODO)
    (19)FMOD.(TODO)
    (20)T4M.(TODO)
    (21)FairyGUI.(TODO)
    (22)KCP.(TODO)
    (23)换装模块.(TODO)
    (24)寻路模块.(Finish)
    (25)ILRuntime.(TODO)
    (26)技能模块.(TODO)
    (27)Buff模块.(Finish)
    (28)AI模块：FSM、行为树.(Finish)
    (29)网络同步机制：帧同步、状态同步.(TODO)
    (30)编辑器：AI(行为树)、战斗(技能)、场景.(TODO)
	
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
	
    Asset Store:
    (1)More Effective Coroutine.
    (2)Fast Shadow Projector.
    (3)A* Pathfinding Project.
    (4)FlowCanvas/NodeCanvas.
    (5)Behavior Designer.
	(6)Unity Particle Pack 5.x.
	(6)Anima2D.
	(6)PostProcessing.
	(7)DOTween.
	
#### Server：服务器工程使用visual studio2017创建
    Reference：
    (1)protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net).
	
#### Tools：相关工具目录
    (1)protoc-gen-lua.
    (2)protoc-gen-csharp.
	
#### 更新记录
    (1)branch_v1.0.
    (2)branch_v2.0(更新tolua版本).
    (3)branch_v3.0(资源管理重构(异步加载优化;对象池)).