#coding:utf-8
import re
import json
import os
import shutil
import sys
import platform;
import time
import datetime
import fileutil
reload(sys)
sys.setdefaultencoding('utf-8')
now = datetime.datetime.now()
#转换为指定的格式:
otherStyleTime = now.strftime("%Y-%m-%d_%H%M%S")
UnityCMD = ""
SVNCMD = ""
BUILDHOME= os.path.dirname(os.getcwd())
print("BUILDHOME:" + BUILDHOME)
BUILDHOME = BUILDHOME.replace("\\", "/")
print("BUILDHOME:" + BUILDHOME)
OutProjPath = os.getcwd().replace("\\", "/") + "/Builds"  
ExportProjPath = OutProjPath + "/X2"

#传入打包平台
sdk_name = sys.argv[1]

#SDK path 在运行时候改变
SDKPath = BUILDHOME + "/tools/SDK/" + sdk_name
print("cur platform:" + platform.system())
IsWindow = False
if platform.system() == "Windows":
    IsWindow = True
    UnityCMD="D:/ProgramSoft/unity538f1/Editor/Unity.exe"
    SVNCMD="D:/ProgramSoft/svn/bin/TortoiseProc.exe "
else:
    UnityCMD="/Applications/Unity545/Unity.app/Contents/MacOS/Unity"
    SVNCMD="svn "
def svn_revert_up():
    cmd = SVNCMD + "revert ../ -R *"
    if IsWindow:
        cmd = SVNCMD +" /command:revert /path:./../"
    print(cmd)
    os.system(cmd)
    cmd = SVNCMD + "up ../"
    if IsWindow:
        cmd = SVNCMD +" /command:update /path:./../"
    print(cmd)
    os.system(cmd)
    print("svn up finish")
    
def ezsdk_proc():
    import ezsdkplatform
    ezsdkplatform.ProcessDifPlatform(sdk_name)
def ezsdk_procGameData():
    import ezsdkplatform
    ezsdkplatform.ProcessDifPlatformGameData(sdk_name)
def Build_Google_Proj():
    cmd = UnityCMD + " -quit -batchmode -projectPath " + BUILDHOME + " -executeMethod AutoBuild.BuildAndroidCommon -outPath " + OutProjPath
    print(cmd)
    os.system(cmd)
    
def Build_Android_AB():
    print("Build_Android_AB")
    
def Move_Resource_Out():
    print("Move_Resource_Out")
    
def Move_Script_Out():
    print("Move_Script_Out")
def Build_AB_Android():
    print("Build_AB_Android")
    
def Build_DLL_Android():
    print("Build_DLL_Android")
    

def move_google_proj_to_SDK_plat():
    print(ExportProjPath+ "/libs")
    print(SDKPath + "/app/libs")
    
    print(ExportProjPath+ "/assets")
    print(SDKPath + "/app/src/main/assets")
    appjniPath = SDKPath + "/app/src/main/jniLibs"
    if not os.path.exists(appjniPath):
        os.makedirs(appjniPath)
    jniLibsPath = ExportProjPath+ "/libs"
    dirs = os.listdir(jniLibsPath)
    #cpu arch copy to jniLibs
    for dir in dirs:
        srcname = os.path.join(jniLibsPath, dir)
        dstname = os.path.join(appjniPath, dir)
        if os.path.isdir(srcname):
            fileutil.file_copytree(srcname, dstname, True);
    fileutil.file_copytree(ExportProjPath+ "/libs",  SDKPath + "/app/libs", False)
    fileutil.file_copytree(ExportProjPath+ "/assets",  SDKPath + "/app/src/main/assets", True)
    os.remove(SDKPath + "/app/libs/xgame2activity.jar")

def Release_APK():
    if platform.system() == "Windows":
        os.chdir(SDKPath)
        print(os.getcwd())
        #注意这玩意，build.gradle如果只用android配置文件一个样，当用来ndk配置完全不一样，网上的文档大部分针对都是只用adt 照着去弄的话会让人怀疑人生
        #像应用宝的就是用了gradle-experimental  http://tools.android.com/tech-docs/new-build-system/gradle-experimental
        cmd="gradlew.bat assembleRelease"
        print(cmd)
        os.system(cmd)
    else:
        os.chdir(SDKPath)
        print(os.getcwd())
        cmd = "chmod a+x ./gradlew"
        print(cmd)
        os.system(cmd)
        cmd="./gradlew assembleRelease"
        print(cmd)
        os.system(cmd)
        
def BuildAndroid():
    #svn_revert_up()
    print("ezsdk_proc")
    ezsdk_proc()
    #Build_Google_Proj()
    # 下面两步循环
    print("move_google_proj_to_SDK_plat")
    move_google_proj_to_SDK_plat()
    print("ezsdk_procGameData")
    ezsdk_procGameData()
    print("Release_APK")
    Release_APK()
if __name__ == "__main__":
    BuildAndroid()