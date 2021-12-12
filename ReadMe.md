### myGameFramework
#### 基础框架

software：Unity2019.4.26/visual studio2017.

#### Client：
    Module：
    >> LOG.(Finish)
    >> ECS.(Finish)
    >> 单例.(Finish)
    >> 协程.(Finish)
    >> 定时器.(Finish)
    >> 配置表.(Finish)
    >> 热更新.(TODO)
    >> 热重载.(TODO)
    >> 本地化.(TODO)
    >> 对象池.(Finish)
    >> AssetBundle.(Finish)
    >> 资源管理.(Finish)
    >> 场景管理.(Finish)
    >> SDK管理.(Finish)
    >> 事件系统.(Finish)
    >> 网络系统.(Finish)
	
    >> 阴影.(Finish)
    >> Timeline.(TODO)
    >> Wwise.(TODO)
    >> KCP.(TODO)
    >> Jenkins.(TODO)
    >> 换装/染色.(TODO)
	
    >> 寻路.(Finish)
    >> 技能.(TODO)
    >> Buff.(Finish)
    >> AI：FSM、行为树.(Finish)
    >> 网络同步机制：帧同步、状态同步.(TODO)
	
    >> Editor：AI<行为树>、战斗<技能>、场景、打包.(TODO)
    >> Lua：tolua、Class、Profiler、工具、方法、定时器、事件系统.(Finish)
    >> UI：滑动列表、图文混排、超链、HUD、飘字、物品掉落.(TODO)
	
    >> 项目优化.(TODO)
    >> 其他工具.(TODO)
	
    Reference：
    >> tolua：(https://github.com/topameng/tolua).
    >> protoc-gen-lua：(https://github.com/topameng/protoc-gen-lua).
    >> AssetBundle：(https://github.com/HushengStudent/myAssetBundleTools).
    >> protobuf-net-r668：(https://github.com/mdavid/protobuf-net).
    >> ObjectPool：(https://github.com/HushengStudent/ugui).
    >> Mixed Text and Graphics：(https://github.com/zouchunyi/EmojiText).
    >> Table：(https://github.com/Ribosome2/ExcelUtilityWith-ExcelReader).
    >> NetWork：(https://github.com/EllanJiang/GameFramework).
    >> Lua Profiler：(https://github.com/yaukeywang/LuaMemorySnapshotDump).
    >> Navigation：(https://github.com/LingJiJian/NavmeshExport).
    >> FancyScrollView：(https://github.com/setchi/FancyScrollView).
    >> UIEffect：(https://github.com/mob-sakai/UIEffect).
	
    Third Party:
    >> Anima2D.
    >> AssetBundle Browser.
    >> Astar Pathfinding Project.
    >> Build Report.
    >> Cinemachine.
    >> DOTween.
    >> Effect Examples.
    >> Fast Shadow Projector.
    >> Node Editor.
    >> FlowCanvas/NodeCanvas.
    >> PostProcessing.
    >> TextMesh Pro.
    >> TexturePacker Importer.
    >> More Effective Coroutine.
    >> Behavior Designer.
    >> Unity Particle Pack 5.x.
    >> Unity Logs Viewer.
    >> Odin Inspector and Serializer 2.1.	
    >> BackgroundDownload.	
	
    环境变量配置:
    >> myGameFramework:(工程路径)如C:/myGameFramework/
    >> myGameFramework_protoc:(protoc.exe路径)如c:/protobuf-3.0.0/src/
	
#### Server：
    Reference：
    >> protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net).
	
#### Tools：
    >> protoc-gen-lua.
    >> protoc-gen-csharp.
	
#### 更新记录
    >> branch_v1.0.
    >> branch_v2.0(更新tolua版本).
    >> branch_v3.0(资源管理重构(异步加载优化;对象池)).
    >> branch_v4.0(更新到Unity2019).