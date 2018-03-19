path   %path%;c:\Python27;D:\Python

move .\..\Assets\XGame\Resources .\..\tools\Builds\Others
move .\..\Assets\uLua .\..\tools\Builds\Others
move .\..\Assets\XGame\Script .\..\tools\Builds\Others
move .\..\Assets\XGame\Editor .\..\tools\Builds\Others
move .\..\Assets\XGame\Public\Editor .\..\tools\Builds\Public
move .\..\Assets\XGame\Public\Scripts .\..\tools\Builds\Public

@SETLOCAL
@REM UNITY install path
@set UnityPATH="C:\Program Files\Unity5.4.5f1\Editor"

@set "t=%cd%"
@cd..
@set "ProjectPath=%cd%"
@echo %ProjectPath%

@echo Begin Build
@cd /d %UnityPATH%

Unity.exe -batchmode -projectPath "%ProjectPath%" -executeMethod AutoBuild.BuildAndroidAPK -outPath “%ProjectPath%\BuildTools” -quit -logFile "%ProjectPath%\android.log"

@echo End Build
@echo See build Log:%ProjectPath%\android.log

@ENDLOCAL
  
move .\..\tools\Builds\Others\Resources .\..\Assets\XGame\Resources
move .\..\tools\Builds\Others\uLua .\..\Assets\uLua
move .\..\tools\Builds\Others\Script .\..\Assets\XGame\Script
move .\..\tools\Builds\Others\Editor .\..\Assets\XGame\Editor
move .\..\tools\Builds\Public\Editor .\..\Assets\XGame\Public\Editor
move .\..\tools\Builds\Public\Scripts .\..\Assets\XGame\Public\Scripts

pause