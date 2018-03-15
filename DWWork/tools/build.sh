#!/bin/bash

#define var
UnityCmd=/Applications/Unity/Unity.app/Contents/MacOS/Unity 
if [[ "xxx${WORKHOME}" = "xxx" ]]; then
	WORKHOME=${HOME}/Documents/xgame2
fi

CurrentPath=$(cd `(dirname $0)`; pwd)
BUILDHOME=${HOME}/Documents/xgame2/build/
PROJECTHOME=${CurrentPath}/../
DATE_TIME=`date +%Y%m%d`_`date +%H%M`
BUILDNAME=`basename ${CurrentPath%/*}`/`date +%Y%m%d`_`date +%H%M`


DIS_SIGNNAME="distribution"
DEV_SIGNNAME="iPhone Developer: ye zhu (74D5JPW6CX)"
#下载下来的mobileprovision需要用mobileprovision-read -f the-file-path -o UUID查看，绝大部分机器我是安装了这个脚本的
#如果没有请下载curl https://raw.githubusercontent.com/0xc010d/mobileprovision-read/master/main.m | clang -framework Foundation -framework Security -o /usr/local/bin/mobileprovision-read -x objective-c -
DEV_PROVISION="233034b2-c27c-42b4-91c8-07dd688918b9"

mkdir -p ${BUILDHOME}
mkdir -p ${BUILDHOME}/${BUILDNAME}

FUNCTION_NAME=""
APP_NAME=""
PARAM1=$1

choose(){
	PARAM1=$1	
	if [ "$PARAM1" == "IOS" ]; then
		echo "build IOS"
		APP_NAME="xgame2_iOS.ipa"
	elif [ "$PARAM1" == "IOS_HD" ]; then	
		echo "build IOS HD"
		APP_NAME="CGG_iOS_HD.ipa"
	elif [ "$PARAM1" == "Debug" ]; then
		echo "build debug APK"
		FUNCTION_NAME="Build_Debug_APK"
		APP_NAME="CGG_Debug"
	else
		echo "**************************************************"
		echo "**************************************************"
		echo "**************************************************"
		echo "Parameters Error!!"
		echo "	IOS		| build ios"
		echo "	IOS_HD		| build ios HD"
		echo "	Debug		| build debug apk"
		echo "**************************************************"
		echo "**************************************************"
		echo "**************************************************"		
		return 1
	return 0
	fi
}



#build android 
build_android_common(){
	echo "build android starting"
	 
	cmdBuildAB="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod BuildAssetsBundleEditor.BuildAndrokdAB"
	echo $cmdBuildAB
	#$cmdBuildAB 
	 
	cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod BuildAssetsBundleEditor.GenLuaFiles"
	echo $cmdGenLua
	$cmdGenLua  
	 
	cmdRemoveScripts="rm -rf ${PROJECTHOME}/Assets/XGame/Script"
	echo $cmdRemoveScripts
	$cmdRemoveScripts
	
	cmdRemoveEditor="rm -rf ${PROJECTHOME}/Assets/XGame/Editor"
	echo $cmdRemoveEditor
	$cmdRemoveEditor
	
	cmdRemoveuLua="rm -rf ${PROJECTHOME}/Assets/uLua"
	echo $cmdRemoveuLua
	$cmdRemoveuLua
	
	cmdRemoveuLua="rm -rf ${PROJECTHOME}/Assets/XGame/Public/Editor"
	echo $cmdRemoveuLua
	$cmdRemoveuLua
	
	cmdRemoveuLua="rm -rf ${PROJECTHOME}/Assets/XGame/Public/Scripts"
	echo $cmdRemoveuLua
	$cmdRemoveuLua
}

build_android(){ 
	cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.${FUNCTION_NAME} -outPath ${BUILDHOME}/${APP_NAME}"
	echo $cmd
	$cmd
	echo "build android finished"
}

#build ios
build_ios_project(){
	#cmdGenLua="rm -rf ${PROJECTHOME}/Assets/StreamingAssets/AssetBundles/*"
	echo $cmdGenLua
	$cmdGenLua
      
    #cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod #TextureAlphaEditor.SeperateAllTexturesRGBandAlphaChannel"
	#echo $cmdGenLua
	#$cmdGenLua  
    
    #cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod #EZFunTextureTools.HandleTexturesToPrefab"
	#echo $cmdGenLua
	#$cmdGenLua  
    
	#cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod #X2AssetsBundleEditor.BuildAllAssetBundleiOS"
	#echo $cmdGenLua
	#$cmdGenLua  
	 
	#mv ${PROJECTHOME}/Assets/XGame/Resources ${PROJECTHOME}/../ResourceTemp

    cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.BuildIOS -outPath ${BUILDHOME}/${BUILDNAME}/export_project"
    echo $cmd
    $cmd
	
	#mv ${PROJECTHOME}/../ResourceTemp ${PROJECTHOME}/Assets/XGame/Resources 
}

build_ab_ios_project(){
	

	cmdGenLua="rm -rf ${PROJECTHOME}/Assets/StreamingAssets/AssetBundles/*"
	echo $cmdGenLua
	$cmdGenLua  
    
    cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod TextureAlphaEditor.SeperateAllTexturesRGBandAlphaChannel"
	echo $cmdGenLua
	$cmdGenLua  
    
    cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod EZFunTextureTools.HandleTexturesToPrefab"
	echo $cmdGenLua
	$cmdGenLua  
	
	
	cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod X2AssetsBundleEditor.BuildAllAssetBundleiOS"
	echo $cmdGenLua
	$cmdGenLua  
	 
	mv ${PROJECTHOME}/Assets/XGame/Resources/EZFunUI ${PROJECTHOME}/../EZFunUITemp
	mv ${PROJECTHOME}/Assets/XGame/Resources/Prefab ${PROJECTHOME}/../PrefabTemp
	mv ${PROJECTHOME}/Assets/XGame/Resources/Scene ${PROJECTHOME}/../SceneTemp

    cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.BuildABIOS -outPath ${BUILDHOME}/${BUILDNAME}/export_project"
    echo $cmd
    $cmd
	
	mv ${PROJECTHOME}/../EZFunUITemp ${PROJECTHOME}/Assets/XGame/Resources/EZFunUI 
	mv ${PROJECTHOME}/../PrefabTemp ${PROJECTHOME}/Assets/XGame/Resources/Prefab 
	mv ${PROJECTHOME}/../SceneTemp ${PROJECTHOME}/Assets/XGame/Resources/Scene 
}

build_ios_hd_project(){
    cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.BuildIOSHD -outPath ${BUILDHOME}/${BUILDNAME}/export_project"
    echo $cmd
    $cmd
}

build_ios_xcarchive() {
    cd ${BUILDHOME}/${BUILDNAME}/export_project
    echo "ybgg ${BUILDHOME}/${BUILDNAME}/export_project"
    cmd="security unlock-keychain -p test12345"
    echo $cmd
    $cmd
	
	
	
#    cmd="xcodebuild -sdk iphoneos archive -scheme Unity-iPhone -target Unity-iPhone -configuration Debug  -archivePath ${BUILDHOME}/${BUILDNAME}/CGG.xcarchive"
    cmd="xcodebuild  -project Unity-iPhone.xcodeproj -target Unity-iPhone -configuration Release -sdk iphoneos build CODE_SIGN_IDENTITY=${DEV_SIGNNAME} PROVISIONING_PROFILE=${DEV_PROVISION}"
    echo $cmd
    #$cmd
	xcodebuild  -project Unity-iPhone.xcodeproj -target Unity-iPhone -configuration Release -sdk iphoneos build CODE_SIGN_IDENTITY="${DEV_SIGNNAME}" PROVISIONING_PROFILE="${DEV_PROVISION}"
}

build_ios_ipa() {
    rm -fr ${BUILDHOME}/${APP_NAME}
	cmd="xcrun -sdk iphoneos PackageApplication -v  ./build/Release-iphoneos/xgame2.app -o ${BUILDHOME}/${APP_NAME}"
	echo $cmd
	$cmd
    #cmd="xcodebuild -exportArchive -exportFormat IPA -archivePath ${BUILDHOME}/${BUILDNAME}/xgame2.xcarchive/ -exportPath ${BUILDHOME}/${APP_NAME}  -exportSigningIdentity ${DEV_SIGNNAME}"
    #echo $cmd
    #xcodebuild -exportArchive -exportFormat IPA -archivePath ${BUILDHOME}/${BUILDNAME}/xgame2.xcarchive/ -exportPath ${BUILDHOME}/${APP_NAME}  -exportSigningIdentity "${DEV_SIGNNAME}"
}

build_ios_moveDll()
{
	cmd="mv ${PROJECTHOME}/tools/iOS/nguiLib.dll ${PROJECTHOME}/Assets/Plugins/ios/"
	echo $cmd
	$cmd
	
	cmd="mv ${PROJECTHOME}/tools/iOS/DataEyeDll.dll ${PROJECTHOME}/Assets/Plugins/ios/"
	echo $cmd
	$cmd

	cmd="mv ${PROJECTHOME}/tools/iOS/XGame2Util.dll ${PROJECTHOME}/Assets/Plugins/ios/"
	echo $cmd
	$cmd
}

build_android_moveDll()
{
	cmd="mv ${PROJECTHOME}/tools/Android/nguiLib.dll ${PROJECTHOME}/Assets/Plugins/Android/"
	echo $cmd
	$cmd
	
	cmd="mv ${PROJECTHOME}/tools/Android/DataEyeDll.dll ${PROJECTHOME}/Assets/Plugins/Android/"
	echo $cmd
	$cmd

	cmd="mv ${PROJECTHOME}/tools/Android/XGame2Util.dll ${PROJECTHOME}/Assets/Plugins/ios/"
	echo $cmd
	$cmd
}

build_ios() {
	rm ${PROJECTHOME}/Assets/Plugins/Android/GamesirSDK.dll
	rm -r  ${PROJECTHOME}/Assets/Gamesir
	rm -r  ${PROJECTHOME}/Assets/Editor
    echo "build ios starting"
    rm -fr ${BUILDHOME}/${BUILDNAME}/*
	build_ios_moveDll
    build_ios_project
    build_ios_xcarchive
    build_ios_ipa
    echo "build ios finished"
}

build_ab_ios() {
	rm ${PROJECTHOME}/Assets/Plugins/Android/GamesirSDK.dll
	rm -r  ${PROJECTHOME}/Assets/Gamesir
	rm -r  ${PROJECTHOME}/Assets/Editor
    echo "build ios starting"
    rm -fr ${BUILDHOME}/${BUILDNAME}/*
	build_ios_moveDll
    build_ab_ios_project
    build_ios_xcarchive
    build_ios_ipa
    echo "build ios finished"
}

build_ios_hd() {
	rm ${PROJECTHOME}/Assets/Plugins/Android/GamesirSDK.dll
	rm -r  ${PROJECTHOME}/Assets/Gamesir
	rm -r  ${PROJECTHOME}/Assets/Editor
    echo "build ios starting"
    rm -fr ${BUILDHOME}/${BUILDNAME}/*
    build_ios_hd_project
    build_ios_xcarchive
    build_ios_ipa
    echo "build ios finished"
}

build_win() {
    echo "build win starting"
    mkdir -p ${BUILDHOME}/CGG.win
    cmd="${UnityCmd} -quit -batchmode ${PROJECTHOME} -executeMethod AutoBuild.BuildWin -outPath ${BUILDHOME}/CGG.win/CGG.exe"
    echo $cmd
    $cmd
    echo "build win finished"
}

svn_update() {
    echo "svn update starting"
    svn revert ${PROJECTHOME} -R *
	echo "PROJECTHOME ${PROJECTHOME}"
    svn up ${PROJECTHOME}
	echo "BUILDHOME ${BUILDHOME}"
    svn up ${BUILDHOME}
    #cmd="rm -fr ${PROJECTHOME}"
    #echo $cmd
    #$cmd
    #svn co svn://192.168.1.9/ezfun/client/trunk ${PROJECTHOME}
    echo "svn update finished"
}

svn_commit() {
    cd ${BUILDHOME}
    echo "svn commit starting"
	echo "commint ${APP_NAME}"
    svn commit -m'auto build' ${APP_NAME} 
    echo "svn commit finished"
}


if [ "$PARAM1" == "IOS" ]; then
	APP_NAME="XGame2_iOS_${DATE_TIME}.ipa"
    svn_update
	mv ${PROJECTHOME}/Assets/PlatformPlugin ${PROJECTHOME}/..
	rm -rf ${PROJECTHOME}/Assets/Plugins/x86
	rm -rf ${PROJECTHOME}/Assets/Plugins/x86_64
	#rm -rf ${PROJECTHOME}/Assets/Plugins/ios/LitJson.dll
	build_ios
	svn_commit	
	cmd="cp ${BUILDHOME}/${APP_NAME} /Volumes/share/xgame2/Builds/iOS/${APP_NAME}"
	echo $cmd
	$cmd
	#sh /Users/a123/Desktop/tool/buglySymboliOS.sh ${BUILDHOME}/${BUILDNAME}/export_project/build/Release-iphoneos/xgame2.app.dSYM  ${BUILDHOME}/xgame2.app.dSYM.zip
	#svn ci -m'auto build' ${BUILDHOME}/xgame2.app.dSYM.zip
	cmd="cp /Users/a123/Library/Logs/Unity/Editor.log ${BUILDHOME}/${BUILDNAME}/Editor.log"
	echo $cmd
	$cmd
	mv ${PROJECTHOME}/../PlatformPlugin ${PROJECTHOME}/Assets/ 
elif [ "$PARAM1" == "IOS_AB" ]; then
	APP_NAME="XGame2_iOS_${DATE_TIME}.ipa"
    svn_update
	mv ${PROJECTHOME}/Assets/PlatformPlugin ${PROJECTHOME}/..
	rm -rf ${PROJECTHOME}/Assets/Plugins/x86
	rm -rf ${PROJECTHOME}/Assets/Plugins/x86_64
	#rm -rf ${PROJECTHOME}/Assets/Plugins/ios/LitJson.dll
	build_ab_ios
	svn_commit	
	cmd="cp ${BUILDHOME}/${APP_NAME} /Volumes/share/xgame2/Builds/iOS/${APP_NAME}"
	echo $cmd
	$cmd
	#sh /Users/a123/Desktop/tool/buglySymboliOS.sh ${BUILDHOME}/${BUILDNAME}/export_project/build/Release-iphoneos/xgame2.app.dSYM  ${BUILDHOME}/xgame2.app.dSYM.zip
	#svn ci -m'auto build' ${BUILDHOME}/xgame2.app.dSYM.zip
	cmd="cp /Users/a123/Library/Logs/Unity/Editor.log ${BUILDHOME}/${BUILDNAME}/Editor.log"
	echo $cmd
	$cmd
	mv ${PROJECTHOME}/../PlatformPlugin ${PROJECTHOME}/Assets/ 
	
elif [ "$PARAM1" == "IOS_HD" ]; then
	APP_NAME="DFWS_iOS_HD.ipa"
	DEV_SIGNNAME="iPhone Developer: Jiaping Hong (VN7M6R4UE6)"
    svn_update
	build_ios_hd
	svn_commit
else
	for args in $@
	do
		choose $args
		paramRight=$?
		if [ $paramRight == 1 ]; then
			exit 0		
		fi
	done
	
	svn_update
	rm -rf ${PROJECTHOME}/Assets/Plugins/x86
	rm -rf ${PROJECTHOME}/Assets/Plugins/x86_64
	build_android_moveDll
	build_android_common
	
	for args in $@ 
	do 
		choose $args
		build_android
		svn_commit &
	done
	
	cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.BuildAndroidEnd"
	echo $cmd
	#$cmd
fi

