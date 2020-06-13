### myGameFramework
#### My game framework include client server and tools.

software：Unity2017.4.25/visual studio2017.

#### Client：客户端工程使用Unity2017.4.25创建
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
    (19)Wwise.(TODO)
    (20)FairyGUI.(TODO)
    (21)KCP.(TODO)
    (22)换装/染色模块.(TODO)
    (23)寻路模块.(Finish)
    (24)技能模块.(TODO)
    (25)Buff模块.(Finish)
    (26)AI模块：FSM、行为树.(Finish)
    (27)网络同步机制：帧同步、状态同步.(TODO)
    (28)编辑器：AI(行为树)、战斗(技能)、场景、打包.(TODO)
	
    Reference：
    (1)[tolua](https://github.com/topameng/tolua)
    (2)[protoc-gen-lua](https://github.com/topameng/protoc-gen-lua)
    (3)[AssetBundle](https://github.com/HushengStudent/myAssetBundleTools)
    (4)[protobuf-net-r668](https://github.com/mdavid/protobuf-net)
    (5)[ObjectPool](https://github.com/HushengStudent/ugui)
    (6)[Mixed Text and Graphics](https://github.com/zouchunyi/EmojiText)
    (7)[Table](https://github.com/Ribosome2/ExcelUtilityWith-ExcelReader)
    (8)[NetWork](https://github.com/EllanJiang/GameFramework)
    (9)[Lua Profiler](https://github.com/yaukeywang/LuaMemorySnapshotDump)
    (10)[Navigation](https://github.com/LingJiJian/NavmeshExport)
	
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
	
#### Server：服务器工程使用visual studio2017创建
    Reference：
    (1)[protobuf-net-r668](protobuf-net-r668:https://github.com/mdavid/protobuf-net)
	
#### Tools：相关工具目录
    (1)protoc-gen-lua.
    (2)protoc-gen-csharp.
	
#### 更新记录
    (1)branch_v1.0.
    (2)branch_v2.0(更新tolua版本).
    (3)branch_v3.0(资源管理重构(异步加载优化;对象池)).