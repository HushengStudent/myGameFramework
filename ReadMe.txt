myGameFramework
This is a game framework include client server and some tools.

software：Unity5.5.4/visual studio2013.
--------------------------------------------------------------
Client：客户端工程使用Unity5.5.4创建
	(1)整合tolua，搭建lua ui开发框架：(tolua:https://github.com/topameng/tolua)
	(2)使用protoc-gen-lua，协议热更：(pblua:https://github.com/topameng/protoc-gen-lua)
	(3)AssetBundle打包：(myAssetBundleTools:https://github.com/HushengStudent/myAssetBundleTools)
	(4)Client使用protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net)
	
Server：服务器工程使用visual studio2013创建
	(1)Server使用protobuf-net-r668：(protobuf-net-r668:https://github.com/mdavid/protobuf-net)
	
Tools：相关工具目录
	(1)protoc-gen-lua：导出pb文件到lua文件，供lua使用protobuff.
	(2)protoc-gen-csharp：导出pb文件到C#文件，供Client和Server使用protobuff.
