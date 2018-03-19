#include <jni.h>
#include <android/asset_manager.h>
#include <android/asset_manager_jni.h>
#include <android/log.h>
#include <stdlib.h>
#include <string>
#define  LOG_TAG    "xgame2"
#define  LOGI(...)  __android_log_print(ANDROID_LOG_INFO,LOG_TAG,__VA_ARGS__)

#define MACRO_CAT( a, b )				MACRO_PRIMITIVE_CAT( a, b )
#define MACRO_PRIMITIVE_CAT( a, b )		a ## b
#define MACRO_COMMA						,
#define MACRO_EMPTY						

// DLL 导出函数定义宏
#ifdef _MSC_VER
#	define NATIVE_LIB_DLL __declspec(dllexport)
#else
#	define NATIVE_LIB_DLL
#endif

//调用约定定义宏
#ifdef _WINDLL
#	ifdef _MSC_VER
#		define NATIVE_LIB_API __stdcall
#	else
#		define NATIVE_LIB_API __attribute__((stdcall))
#	endif
#else
#	ifdef _MSC_VER
#		define NATIVE_LIB_API __cdecl
#	else
#		define NATIVE_LIB_API
#	endif
#endif

extern "C" 
{
    JNIEXPORT void JNICALL Java_com_ezfun_files_FileLoad_setAssetmanager( JNIEnv* env,jobject thiz, jobject assetManager);
	NATIVE_LIB_DLL void NATIVE_LIB_API LoadFileByData(const char *s,char* data);
	NATIVE_LIB_DLL int NATIVE_LIB_API LoadFileSize(const char *s);
};
AAssetManager *mgr;

JNIEXPORT void JNICALL Java_com_ezfun_files_FileLoad_setAssetmanager( JNIEnv* env, jobject thiz,jobject assetManager)
{
		LOGI("xgame2 JNIGlue.cpp : Calling loadFileFromJNI()...");
		mgr = AAssetManager_fromJava(env, assetManager);
		if (mgr == NULL)
		{
			LOGI("xgame2 mgr is null");
		}
		
}
NATIVE_LIB_DLL int NATIVE_LIB_API LoadFileSize(const char *s)
{
		if (mgr == NULL)
		{
			//__android_log_print(ANDROID_LOG_ERROR, LOG_TAG, "error loading asset maanger");
			return -1;
		} 
		AAsset * asset = AAssetManager_open(mgr,s,AASSET_MODE_BUFFER);
		if(!asset)
		{
			LOGI("xgame2 _ASSET_NOT_FOUND_ [%s]", s);
			return -1;
		}
		int strlen =AAsset_getLength(asset);
		AAsset_close(asset);
		return strlen;
}

NATIVE_LIB_DLL void NATIVE_LIB_API LoadFileByData(const char *s,char* buffData)
{
		if (mgr == NULL)
		{
			__android_log_print(ANDROID_LOG_ERROR, LOG_TAG, "error loading asset maanger");
			return ;
		} 
		
		AAsset * asset = AAssetManager_open(mgr,s,AASSET_MODE_BUFFER);
		if(!asset)
		{
			LOGI("xgame2 _ASSET_NOT_FOUND_ [%s]", s);
			return ;
		}
		int strlen =AAsset_getLength(asset);
		AAsset_read (asset,buffData,strlen);
		AAsset_close(asset);
}





