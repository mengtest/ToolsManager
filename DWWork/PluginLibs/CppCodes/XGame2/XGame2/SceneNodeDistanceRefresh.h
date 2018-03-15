#pragma once
#include "Bounds.h"
#include "PreInclude.h"

class NodeData
{
public:
	Bounds bounds;
	float distance;
	NodeData() {}
};
extern "C"
{

	extern NATIVE_LIB_DLL void NATIVE_LIB_API Scene_InitModule();
	extern NATIVE_LIB_DLL void NATIVE_LIB_API Scene_CreateNode(float x, float y, float z, float sx, float sy, float sz, int nodeId, int nx, int ny, float distance);
	extern NATIVE_LIB_DLL int NATIVE_LIB_API Scene_Update(float x, float y, float z);
	extern NATIVE_LIB_DLL void NATIVE_LIB_API Scene_InitData(int nodeWidget, int MaxWidgetNum, int MaxLengthNum, float minX, float minZ);
	extern NATIVE_LIB_DLL void NATIVE_LIB_API Scene_Get(int value[], int& activeLeng, int & loadResourceLenth, int & disableLength);
	extern NATIVE_LIB_DLL void NATIVE_LIB_API Scene_Clear();
}