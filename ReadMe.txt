myGameFramework
This is a game framework include client server and some tools.

software：Unity5.5.4/visual studio2017.
--------------------------------------------------------------
Client：客户端工程使用Unity5.5.4创建
	Module：
	(1)资源管理：AssetBundle打包、资源加载、资源卸载.(Finish)
	(2)tolua.(Finish)
	(3)Log封装.(Finish)
	(4)Lua UI模块：Lua类、工具方法、定时器、事件系统.(Finish)
	(5)Lua Profiler.(Finish)
	(6)ECS模式.(Finish)
	(7)通用单例.(Finish)
	(8)协程封装.(Finish)
	(9)事件系统.(Finish)
	(10)定时器.(Finish)
	(11)网络框架：Lua、C#.(TODO)
	(12)对象池.(Finish)
	(13)sdk封装.(Finish)
	(14)场景管理.(Finish)
	(15)技能模块：FSM.(TODO)
	(16)Buff.(Finish)
	(17)ILRuntime.(TODO)
	(18)网络同步机制：帧同步、状态同步.(TODO)
	(19)UI：UI复用、图文混排、超链、HUD、飘字、物品掉落.(TODO)
	(20)实时阴影.(Finish)
	(21)换装模块.(TODO)
	(22)寻路模块.(TODO)
	(23)Timeline.(TODO)
	(24)FMOD.(TODO)
	(25)T4M.(TODO)
	(26)FairyGUI.(TODO)
	(27)配置表工具.(Finish)
	(28)AI：行为树.(TODO)
	(29)KCP.(TODO)
	(30)编辑器：AI、战斗、场景.(TODO)
	
	Reference：
	(1)tolua：(tolua:https://github.com/topameng/tolua).
	(2)protoc-gen-lua：(pblua:https://github.com/topameng/protoc-gen-lua).
	(3)AssetBundle：(myAssetBundleTools:https://github.com/HushengStudent/myAssetBundleTools).
	(4)protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net).
	(5)ObjectPool：(ugui:https://github.com/HushengStudent/ugui).
	(6)Mixed Text and Graphics：(EmojiText:https://github.com/zouchunyi/EmojiText).
	(7)Table：(ExcelUtilityWith-ExcelReader:https://github.com/Ribosome2/ExcelUtilityWith-ExcelReader).
	(8)NetWork：(GameFramework:https://github.com/EllanJiang/GameFramework).
	(9)Lua Profiler：(LuaMemorySnapshotDump:https://github.com/yaukeywang/LuaMemorySnapshotDump).
	(10)Navigation：(NavmeshExport:https://github.com/LingJiJian/NavmeshExport).
	
	Asset Store:
	(1)More Effective Coroutine.
	(2)Fast Shadow Projector.
	(3)A* Pathfinding Project.
	(4)FlowCanvas.
	(5)NodeCanvas.
	
Server：服务器工程使用visual studio2017创建
	Reference：
	(1)protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net).
	
Tools：相关工具目录
	(1)protoc-gen-lua.
	(2)protoc-gen-csharp.
