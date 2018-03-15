path  %path%;D:\VS\Common7\IDE;c:\Python27;D:\Python;D:\Soft\VS2015\Common7\IDE
devenv /rebuild Release .trunk.sln/project "Assembly-CSharp.csproj" /projectconfig Release

copy .\Library\ScriptAssemblies\Assembly-CSharp-firstpass.dll .\Temp\bin\Debug\Assembly-CSharp-firstpass.dll
copy .\Library\ScriptAssemblies\Assembly-CSharp-firstpass.dll .\Release\bin\Debug\Assembly-CSharp-firstpass.dll
copy .\Library\ScriptAssemblies\Assembly-UnityScript-firstpass.dll .\Temp\bin\Debug\Assembly-UnityScript-firstpass.dll
copy .\Library\ScriptAssemblies\Assembly-UnityScript-firstpass.dll .\Release\bin\Debug\Assembly-UnityScript-firstpass.dll

copy .\obj\Release\%1.dll .\Assets\StreamingAssets\xgame.ezfun

python encrypte.py ./
pause