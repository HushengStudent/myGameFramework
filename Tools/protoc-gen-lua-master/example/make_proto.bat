for %%i in (*.proto) do (  
    protoc.exe --plugin=protoc-gen-lua="..\plugin\build.bat" --lua_out=.\Lua\Protol %%i      
) 