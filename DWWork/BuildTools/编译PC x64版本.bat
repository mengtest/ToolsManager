@SETLOCAL
@REM UNITY install path
@set UnityPATH="D:\Program Files\Unity\Editor"

@set "t=%cd%"
@cd..
@set "ProjectPath=%cd%"
@echo %ProjectPath%

@echo Begin Build
@cd /d %UnityPATH%

Unity.exe -batchmode -projectPath "%ProjectPath%" -executeMethod BuildTools.BuildForPC_X64 -quit -logFile "%ProjectPath%\pc.log"

@echo End Build
@echo See build Log:%ProjectPath%\pc.log

@ENDLOCAL
@pause