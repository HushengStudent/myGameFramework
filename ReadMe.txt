myGameFramework
This is a game framework include client server and some tools.

software：Unity5.5.4/visual studio2013.
--------------------------------------------------------------
Client：客户端工程使用Unity5.5.4创建
	Module：
	(1)资源管理：AssetBundle打包(finish)、资源加载(finish)、资源卸载(finish).
	(2)tolua接入(finish).
	(3)Log封装(finish).
	(4)Lua UI模块：
	    Lua类(finish)、工具方法(finish)、定时器(finish)、事件系统(finish)、网络协议(finish)、Lua Profiler(TODO).
	(5)ECS模式(TODO).
	(6)通用单例(finish).
	(7)协程封装(finish).
	(8)事件系统(finish).
	(9)定时器(finish).
	(10)网络框架：Lua(TODO)、C#(TODO).
	(11)对象池(finish).
	(12)sdk封装(finish).
	(13)场景管理(TODO).
	(14)技能模块：行为树(TODO)、FSM(TODO).
	(15)Buff(TODO).
	(16)ILRuntime(TODO).
	(17)网络同步机制：帧同步(TODO)、状态同步(TODO).
	(18)UI：UI复用(TODO)、图文混排(TODO)、超链(TODO)、HUD(TODO)、飘字(TODO)、物品掉落(TODO).
	(19)实时阴影(TODO).
	(20)换装模块(TODO).
	(21)寻路模块(TODO).
	(22)Timeline(TODO).
	
	Reference：
	(1)tolua：(tolua:https://github.com/topameng/tolua).
	(2)protoc-gen-lua：(pblua:https://github.com/topameng/protoc-gen-lua).
	(3)AssetBundle：(myAssetBundleTools:https://github.com/HushengStudent/myAssetBundleTools).
	(4)protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net).
	(5)More Effective Coroutine：(Asset Store)
	(6)ObjectPool：(ugui:https://github.com/HushengStudent/ugui)
	
Server：服务器工程使用visual studio2013创建
	Reference：
	(1)protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net).
	
Tools：相关工具目录
	(1)protoc-gen-lua.
	(2)protoc-gen-csharp.
