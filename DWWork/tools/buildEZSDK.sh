#!/bin/bash

#define var
UnityCmd=/Applications/Unity5.3.5f1/Unity.app/Contents/MacOS/Unity 
XCodePath=/Applications/Xcode.app/Contents/Developer/usr/bin/
CurrentPath=$(cd `(dirname $0)`; pwd)
WORKHOME=${CurrentPath}/../..
BUILDHOME=${WORKHOME}/build/
PROJECTHOME=${CurrentPath}/../
BUILDNAME=`basename ${CurrentPath%/*}`/`date +%Y%m%d`_`date +%H%M`
DIS_SIGNNAME="distribution"
DEV_SIGNNAME="iPhone Developer: Jiaping Hong (VN7M6R4UE6)"
ANT_SDKPATH=""
APPSHOW_NAME=""

mkdir -p ${BUILDHOME}
mkdir -p ${BUILDHOME}/${BUILDNAME}

FUNCTION_NAME=""
APP_NAME=""
PARAM1=$1
PARAM2=$2
PARAM3=$3

choose(){
	PARAM1=$1	
	if [ "$PARAM1" == "IOS" ]; then
		echo "build IOS"
		APP_NAME="DFWS_iOS.ipa"
	elif [ "$PARAM1" == "OverSea_Google" ]; then
		echo "buidl OverSea apk begin"
	else
		echo "**************************************************"
		echo "**************************************************"
		echo "**************************************************"
		echo "Parameters Error!!"
		echo "	IOS		| build ios"
		echo "**************************************************"
		echo "**************************************************"
		echo "**************************************************"		
		return 1
	return 0
	fi
}

#build ios
build_ios_project(){
    cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.BuildIOS -outPath ${BUILDHOME}/${BUILDNAME}/export_project"
    echo $cmd
    $cmd
}

build_ios_OverSea_project(){
	cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.BuildOverSeaiOS -outPath ${BUILDHOME}/${BUILDNAME}"
    echo $cmd
    $cmd
}

build_ios_xcarchive() {
    cd ${BUILDHOME}/${BUILDNAME}
    echo "cd ${BUILDHOME}/${BUILDNAME}"
    cmd="security unlock-keychain -p test12345"
    echo $cmd
    $cmd
#    cmd="xcodebuild -sdk iphoneos archive -scheme Unity-iPhone -target Unity-iPhone -configuration Debug  -archivePath ${BUILDHOME}/${BUILDNAME}/XGame.xcarchive"
    cmd="${XCodePath}xcodebuild -sdk iphoneos archive -scheme Unity-iPhone -target Unity-iPhone -archivePath ${BUILDHOME}/${BUILDNAME}/XGame.xcarchive"
    echo $cmd
    $cmd
}

build_ios_ipa() {
    rm -fr ${BUILDHOME}/${APP_NAME}
    cmd="${XCodePath}xcodebuild -exportArchive -exportFormat IPA -archivePath ${BUILDHOME}/${BUILDNAME}/XGame.xcarchive/ -exportPath ${BUILDHOME}/${APP_NAME}  -exportSigningIdentity ${DEV_SIGNNAME}"
    echo $cmd
    ${XCodePath}xcodebuild -exportArchive -exportFormat IPA -archivePath ${BUILDHOME}/${BUILDNAME}/XGame.xcarchive/ -exportPath ${BUILDHOME}/${APP_NAME}  -exportSigningIdentity "${DEV_SIGNNAME}"
}

build_android(){
	cmd="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod AutoBuild.${FUNCTION_NAME} -outPath ${BUILDHOME}/${APP_NAME}"
	echo $cmd
	$cmd
	echo "build android finished"
}

build_Move_EZSDKDLL_iOS()
{
	cmd="rm -rf ${PROJECTHOME}Assets/Plugins/EZSDK.dll"
	echo $cmd
	$cmd
	
	cmd="mv -f ${PROJECTHOME}tools/EZSDKDLL/EZSDK.dll ${PROJECTHOME}Assets/Plugins/"
	echo $cmd
	$cmd
}

build_Move_Java()
{
	cmd="rm -rf ${ANT_SDKPATH}"
	echo $cmd
	$cmd
}

build_Python_Android()
{
	cmd="python ${PROJECTHOME}tools/EZSDK.py"
	echo $cmd
	$cmd
}

build_android_common_AS()
{
	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/libs ${ANT_SDKPATH}/app/"
	echo $cmd
	$cmd

	cmd="rm -rf ${ANT_SDKPATH}/app/src/main/jniLibs/armeabi-v7a"
	echo $cmd
	$cmd
	
	cmd="rm -rf ${ANT_SDKPATH}/app/src/main/jniLibs/x86"
	echo $cmd
	$cmd

	cmd="mv -f ${ANT_SDKPATH}/app/libs/armeabi-v7a ${ANT_SDKPATH}/app/src/main/jniLibs/"
	echo $cmd
	$cmd
	
	cmd="mv -f ${ANT_SDKPATH}/app/libs/x86 ${ANT_SDKPATH}/app/src/main/jniLibs/"
	echo $cmd
	$cmd

	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/assets ${ANT_SDKPATH}/app/src/main/"
	echo $cmd
	$cmd
		
	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/res/drawable/app_icon.png ${ANT_SDKPATH}/app/src/main/res/drawable/app_icon.png"
	echo $cmd
	$cmd

	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/res/drawable-hdpi/app_icon.png ${ANT_SDKPATH}/app/src/main/res/mipmap-hdpi/app_icon.png"
	echo $cmd
	$cmd

	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/res/drawable-ldpi/app_icon.png ${ANT_SDKPATH}/app/src/main/res/mipmap-ldpi/app_icon.png"
	echo $cmd
	$cmd

	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/res/drawable-xhdpi/app_icon.png ${ANT_SDKPATH}/app/src/main/res/mipmap-xhdpi/app_icon.png"
	echo $cmd
	$cmd

	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/res/drawable-xxhdpi/app_icon.png ${ANT_SDKPATH}/app/src/main/res/mipmap-xxhdpi/app_icon.png"
	echo $cmd
	$cmd

	cmd="cp -rf ${BUILDHOME}/${APP_NAME}/${APPSHOW_NAME}/res/drawable-xxxhdpi/app_icon.png ${ANT_SDKPATH}/app/src/main/res/mipmap-xxxhdpi/app_icon.png"
	echo $cmd
	$cmd

	cmd="cd ${ANT_SDKPATH}"
	echo $cmd
	$cmd

	cmd="chmod +x gradlew"
	echo $cmd
	$cmd

	#cmd="./gradlew assembleDebug"
	#cmd1="cp -rf ./app/build/outputs/apk/app-debug.apk ${BUILDHOME}/${APP_NAME}_debug.apk"
	#if [ "$PARAM2" == "release" ]; then
	cmd="./gradlew assembleRelease"
	cmd1="cp -rf ./app/build/outputs/apk/app-release.apk ${BUILDHOME}/${APP_NAME}_release.apk"
	#fi
	echo $cmd
	$cmd
	echo $cmd1
	$cmd1
}

svn_commit_apk_AS()
{
	cd ${BUILDHOME}
    echo "svn commit starting"
	
	#if [ "$PARAM2" == "release" ]; then
		echo "commint ${APP_NAME}_release"
    	svn commit -m'auto build' ${APP_NAME}_release.apk &
    #else
    #	echo "commint ${APP_NAME}_debug"
    #	svn commit -m'auto build' ${APP_NAME}_debug.apk &
    #fi
    echo "svn commit finished"
}

svn_update() {
    echo "svn update starting"
    svn revert ${PROJECTHOME} -R *
    svn up ${PROJECTHOME}
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

if [ "$PARAM1" == "OverSea_iOS" ]; then
    APP_NAME="EZSDK_IOS_OverSea.ipa"
    DEV_SIGNNAME="iPhone Developer: ye zhu (74D5JPW6CX)"
    BUILDNAME="EZSDK_TestiOS/export_project"
    mkdir -p ${BUILDHOME}/${BUILDNAME}
    svn_update
    echo "build ios starting"
	build_Move_EZSDKDLL_iOS
    build_ios_OverSea_project
    echo "out xcodeproject end"
    build_ios_xcarchive
    build_ios_ipa
    svn_commit
    echo "build ios end"
elif [ "$PARAM1" == "OverSea_Google" ]; then
	echo "buidl OverSea apk begin"
	APP_NAME="EZSDK_OverSea_Google"
	FUNCTION_NAME="BuildOverSeaAndroid"
	ANT_SDKPATH="${PROJECTHOME}tools/SDK/OverSea"
	APPSHOW_NAME="DynastyBlades"
	build_Move_Java
	svn_update
	build_android
	build_Python_Android
	build_android_common_AS
	svn_commit_apk_AS
elif [ "$PARAM1" == "yyb" ]; then
	echo "buidl yyb begin"
	APP_NAME="EZSDK_yyb"
	FUNCTION_NAME="BuildYYB"
	ANT_SDKPATH="${PROJECTHOME}tools/SDK/YSDK"
	APPSHOW_NAME="EZSDK_Py"
	build_Move_Java
	svn_update
	build_android
	build_Python_Android
	build_android_common_AS
	svn_commit_apk_AS
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

