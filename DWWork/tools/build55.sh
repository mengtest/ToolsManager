#!/bin/bash

#define var
UnityCmd=/Applications/Unity55/Unity.app/Contents/MacOS/Unity 
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
DEV_PROVISION="02548ff8-ab59-4390-97a8-c3dbbdd51447"

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
	elif [ "$PARAM1" == "YYB_DFBY" ]; then
		echo "build yingyongbao_dfby APK"
		FUNCTION_NAME="Build_YYB_Apk"
		APP_NAME="DFBY_YingYongBao.apk"
	elif [ "$PARAM1" == "UC" ]; then
		echo "build UC APK"
		FUNCTION_NAME="Build_UC_APK"
		APP_NAME="CGG_UC.apk"
	elif [ "$PARAM1" == "BD" ]; then
		echo "build baidu APK"
		FUNCTION_NAME="Build_BD_APK"
		APP_NAME="CGG_BaiDu.apk"
	elif [ "$PARAM1" == "LJ" ]; then
		echo "build lengjing APK"
		FUNCTION_NAME="Build_LJ_APK"
		APP_NAME="CGG_LengJing.apk"
	elif [ "$PARAM1" == "MI" ]; then
		echo "build xiaomi APK"
		FUNCTION_NAME="Build_Mi_APK"
		APP_NAME="CGG_XiaoMi.apk"
	elif [ "$PARAM1" == "IPay" ]; then
		echo "build IPay APK"
		FUNCTION_NAME="Build_IPay_APK"
		APP_NAME="CGG_IPay.apk"
	elif [ "$PARAM1" == "oppo" ]; then
		echo "build oppo APK"
		FUNCTION_NAME="Build_OPPO_APK"
		APP_NAME="CGG_Oppo.apk"
	elif [ "$PARAM1" == "vivo" ]; then
		echo "build vivo APK"
		FUNCTION_NAME="Build_VIVO_APK"
		APP_NAME="CGG_Vivo.apk"
	elif [ "$PARAM1" == "lenovo" ]; then
		echo "build lenovo APK"
		FUNCTION_NAME="Build_lenovo_APK"
		APP_NAME="CGG_Lenovo.apk"
	elif [ "$PARAM1" == "sogou" ]; then
		echo "build sogou APK"
		FUNCTION_NAME="Build_sogou_APK"
		APP_NAME="CGG_Sogou.apk"
	elif [ "$PARAM1" == "huawei" ]; then
		echo "build huawei APK"
		FUNCTION_NAME="Build_huawei_APK"
		APP_NAME="CGG_HuaWei.apk"
	elif [ "$PARAM1" == "qihoo" ]; then
		echo "build qihoo APK"
		FUNCTION_NAME="Build_qihoo_APK"
		APP_NAME="CGG_360.apk"
	elif [ "$PARAM1" == "jinli" ]; then
		echo "build jinli APK"
		FUNCTION_NAME="Build_jinli_APK"
		APP_NAME="CGG_JinLi.apk"
	elif [ "$PARAM1" == "mzw" ]; then
		echo "build mzw APK"
		FUNCTION_NAME="Build_mzw_APK"
		APP_NAME="CGG_mzw.apk"
	elif [ "$PARAM1" == "yyh" ]; then
		echo "build yyh APK"
		FUNCTION_NAME="Build_yyh_APK"
		APP_NAME="CGG_yyh.apk"
	elif [ "$PARAM1" == "pps" ]; then
		echo "build pps APK"
		FUNCTION_NAME="Build_pps_APK"
		APP_NAME="CGG_pps.apk"
	elif [ "$PARAM1" == "htc" ]; then
		echo "build htc APK"
		FUNCTION_NAME="Build_htc_APK"
		APP_NAME="CGG_htc.apk"
	elif [ "$PARAM1" == "kugou" ]; then
		echo "build kugou APK"
		FUNCTION_NAME="Build_kugou_APK"
		APP_NAME="CGG_kugou.apk"
	elif [ "$PARAM1" == "coolpad" ]; then
		echo "build coolpad APK"
		FUNCTION_NAME="Build_coolpad_APK"
		APP_NAME="CGG_coolpad.apk"
	elif [ "$PARAM1" == "m4399" ]; then
		echo "build m4399 APK"
		FUNCTION_NAME="Build_m4399_APK"
		APP_NAME="CGG_m4399.apk"
	elif [ "$PARAM1" == "YYB_CGG" ]; then
		echo "build YYB_CGG APK"
		FUNCTION_NAME="Build_MSDK_CGG_APK"
		APP_NAME="CGG_yingyongbao.apk"
	elif [ "$PARAM1" == "youku" ]; then
		echo "build youku APK"
		FUNCTION_NAME="Build_youku_APK"
		APP_NAME="CGG_youku.apk"
	elif [ "$PARAM1" == "wandoujia" ]; then
		echo "build wandoujia APK"
		FUNCTION_NAME="Build_wdj_APK"
		APP_NAME="CGG_wdj.apk"
	elif [ "$PARAM1" == "meizu" ]; then
		echo "build meizu APK"
		FUNCTION_NAME="Build_meizu_APK"
		APP_NAME="CGG_meizu.apk"
	elif [ "$PARAM1" == "dangle" ]; then
		echo "build dangle APK"
		FUNCTION_NAME="Build_dangle_APK"
		APP_NAME="CGG_dangle.apk"
	elif [ "$PARAM1" == "dianxin" ]; then
		echo "build dianxin APK"
		FUNCTION_NAME="Build_dianxin_APK"
		APP_NAME="CGG_dianxin.apk"
	elif [ "$PARAM1" == "xinlang" ]; then
		echo "build xinlang APK"
		FUNCTION_NAME="Build_sina_APK"
		APP_NAME="CGG_xinlang.apk"
	elif [ "$PARAM1" == "pptv" ]; then
		echo "build pptv APK"
		FUNCTION_NAME="Build_pptv_APK"
		APP_NAME="CGG_pptv.apk"
	elif [ "$PARAM1" == "kaopu" ]; then
		echo "build kaopu APK"
		FUNCTION_NAME="Build_kaopu_APK"
		APP_NAME="CGG_kaopu.apk"
	elif [ "$PARAM1" == "anzhi" ]; then
		echo "build anzhi APK"
		FUNCTION_NAME="Build_anzhi_APK"
		APP_NAME="CGG_anzhi.apk"
	elif [ "$PARAM1" == "p1767" ]; then
		echo "build p1767 APK"
		FUNCTION_NAME="Build_p1767_APK"
		APP_NAME="CGG_p1767.apk"
	elif [ "$PARAM1" == "p1767_mc" ]; then
		echo "build p1767_mc APK"
		FUNCTION_NAME="Build_p1767_mc_APK"
		APP_NAME="CGG_p1767_mc.apk"
	elif [ "$PARAM1" == "ccplay" ]; then
		echo "build ccplay APK"
		FUNCTION_NAME="Build_ccplay_APK"
		APP_NAME="CGG_ccplay.apk"
	elif [ "$PARAM1" == "dzcf" ]; then
		echo "build dzcf APK"
		FUNCTION_NAME="Build_MSDK_dzcf"
		APP_NAME="CGG_dzcf.apk"
	elif [ "$PARAM1" == "dsfm" ]; then
		echo "build dsfm APK"
		FUNCTION_NAME="Build_MSDK_dsfm"
		APP_NAME="CGG_dsfm.apk"
	elif [ "$PARAM1" == "dszn" ]; then
		echo "build dszn APK"
		FUNCTION_NAME="Build_MSDK_dszn"
		APP_NAME="CGG_dszn.apk"
	elif [ "$PARAM1" == "hstg" ]; then
		echo "build hstg APK"
		FUNCTION_NAME="Build_MSDK_hstg"
		APP_NAME="CGG_hstg.apk"
	elif [ "$PARAM1" == "dzcs" ]; then
		echo "build dzcs APK"
		FUNCTION_NAME="Build_MSDK_dzcs"
		APP_NAME="CGG_dzcs.apk"
	elif [ "$PARAM1" == "qtzb" ]; then
		echo "build qtzb APK"
		FUNCTION_NAME="Build_MSDK_banyws"
		APP_NAME="CGG_qtzb.apk"
	elif [ "$PARAM1" == "dxt" ]; then
		echo "build dxt APK"
		FUNCTION_NAME="Build_MSDK_dfzr"
		APP_NAME="CGG_dxt.apk"
	else
		echo "**************************************************"
		echo "**************************************************"
		echo "**************************************************"
		echo "Parameters Error!!"
		echo "	IOS		| build ios"
		echo "	IOS_HD		| build ios HD"
		echo "	Debug		| build debug apk"
		echo "	YYB_DFBY	| build daofengbaiye yingyongbao apk"
		echo "	UC		| build UC apk"
		echo "	BD		| build baidu apk"
		echo "	LJ		| build lengjing apk"
		echo "	MI		| build Build_Mi_APK apk"	
		echo "	IPay		| build IPay APK"	
		echo "	oppo		| build oppo APK"
		echo "	vivo		| build vivo APK"
		echo "	lenovo		| build lenovo APK"
		echo "	sogou		| build sogou APK"
		echo "	huawei		| build huawei APK"
		echo "	qihoo		| build qihoo APK"
		echo "	jinli		| build jinli APK"
		echo "	mzw		| build mzw APK"
		echo "	yyh		| build yyh APK"
		echo "	pps		| build pps APK"
		echo "	htc		| build htc APK"
		echo "	kugou		| build kugou APK"
		echo "	coolpad		| build coolpad APK"
		echo "	m4399		| build m4399 APK"
		echo "	YYB_CGG	| build YYB_CGG APK"
		echo "	youku		| build youku APK"
		echo "	wandoujia	| build wandoujia APK"
		echo "	meizu		| build meizu APK"
		echo "	dangle		| build dangle APK"
		echo "	dianxin		| build dianxin APK"
		echo "	xinlang 	| build xinlang APK"
		echo "	pptv 		| build pptv APK"
		echo "	kaopu 		| build kaopu APK"
		echo "	anzhi 		| build anzhi APK"
		echo "	p1767 		| build p1767 APK"		
		echo "	ccplay 		| build ccplay APK"
		echo "	dzcf 		| build dzcf APK"
		echo "	dsfm 		| build dsfm APK"
		echo "	dszn 		| build dszn APK"
		echo "	hstg 		| build hstg APK"
		echo "	dzcs 		| build dzcs APK"
		echo "	qtzb 		| build qtzb APK"
		echo "	dxt 		| build dxt APK"
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
	cmdGenLua="rm -rf ${PROJECTHOME}/Assets/StreamingAssets/AssetBundles/*"
	echo $cmdGenLua
	$cmdGenLua  
	
	cmdGenLua="${UnityCmd} -quit -batchmode -projectPath ${PROJECTHOME} -executeMethod X2AssetsBundleEditor.BuildAllAssetBundleiOS"
	echo $cmdGenLua
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
}

build_android_moveDll()
{
	cmd="mv ${PROJECTHOME}/tools/Android/nguiLib.dll ${PROJECTHOME}/Assets/Plugins/Android/"
	echo $cmd
	$cmd
	
	cmd="mv ${PROJECTHOME}/tools/Android/DataEyeDll.dll ${PROJECTHOME}/Assets/Plugins/Android/"
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

