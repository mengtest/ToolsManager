#include "SceneNodeDistanceRefresh.h"
#include <unordered_set>
#include <unordered_map>
static unordered_set<int> m_executeSet;
static vector<int> m_active0List;
static vector<int> m_active1List;
static vector<int> m_active2List;

static vector<int> m_temp0List;
static vector<int> m_temp1List;

static unordered_map<int, NodeData*> m_map;
static unordered_map<float, unordered_map<int, unordered_map<int, vector<int>*>*>*> m_nodeDic;

NATIVE_LIB_DLL void NATIVE_LIB_API Scene_InitModule()
{

}
NATIVE_LIB_DLL void NATIVE_LIB_API Scene_CreateNode(float x, float y, float z, float sx, float sy, float sz, int nodeId, int nx, int ny, float distance) {

	auto nodeiter = m_map.find(nodeId);
	if (nodeiter == m_map.end())
	{
		Bounds bounds(Vector3(x, y, z), Vector3(sx, sy, sz));
		auto data = new NodeData();
		if (data == NULL)
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "Failed to new an NodeData object of actor.");
#endif
			return;
		}
		data->distance = distance;
		data->bounds = bounds;
		auto dataiter = m_map.insert(pair<int, NodeData*>(nodeId, data));
		if (!dataiter.second)
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "Failed to insert nodeId=%d an NodeData object of actor.", nodeId);
#endif
			SafeDelete(data);
			return;
		}
	}

	auto disIter = m_nodeDic.find(distance);
	if (disIter == m_nodeDic.end())
	{
		auto disDic = new unordered_map<int, unordered_map<int, vector<int>*>*>();
		if (disDic == NULL)
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "Failed to new an unordered_map<int, unordered_map<int, vector<int>*>*>()object of actor.");
#endif
			return;
		}
		auto ite = m_nodeDic.insert(pair<float, unordered_map<int, unordered_map<int, vector<int>*>*>*>(distance, disDic));
		if (!ite.second)
		{
			SafeDelete(disDic);
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "The actor (nx = %d) xDic object has already been created, couldn't create it again.", nx);
#endif
			return;
		}
		disIter = m_nodeDic.find(distance);
	}

	auto xMap = disIter->second->find(nx);
	if (xMap == disIter->second->end())
	{
		auto xDic = new unordered_map<int, vector<int>*>();
		if (xDic == NULL)
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "Failed to new an unordered_map<int, vector<int>*>() object of actor.");
#endif
			return;
		}
		auto ite = disIter->second->insert(pair<int, unordered_map<int, vector<int>*>*>(nx, xDic));
		if (!ite.second)
		{
			SafeDelete(xDic);
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "The actor (nx = %d) xDic object has already been created, couldn't create it again.", nx);
#endif
			return;
		}
		xMap = disIter->second->find(nx);
	}

	auto yMap = xMap->second->find(ny);
	if (yMap == xMap->second->end())
	{
		auto yDic = new vector<int>();
		if (yDic == NULL)
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "Failed to new an  unordered_set<vector<int>*>() object of actor.");
#endif
		}
		auto ite = xMap->second->insert(pair<int, vector<int>*>(ny, yDic));
		if (!ite.second)
		{
			SafeDelete(yDic);
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "The actor (nx = %d ny = %d) xDic object has already been created, couldn't create it again.", nx, ny);
#endif
			return;
		}
		yMap = xMap->second->find(ny);
	}
	yMap->second->push_back(nodeId);
}

int PosToBounds(const Bounds& bouds, const Vector3& vec, float distance)
{
	if (distance != 0)
	{
		float dis = Bounds::SqrDistanceWithoutY(bouds, vec);
		float loadDis = (distance + 10);
		if (dis <= distance * distance)
		{
			return 0;
		}
		else if (dis <= loadDis * loadDis)
		{
			return 1;
		}
	}
	return 2;
}

void Scene_HandleList(const vector<int>& sceneList, const Vector3& vec)
{
	int state = 2;
	for (size_t i = 0; i < sceneList.size(); i++)
	{
		auto  sceneId = sceneList[i];
		if (m_executeSet.find(sceneId) != m_executeSet.end())
		{
			continue;
		}
		m_executeSet.insert(sceneId);
		auto dataIter = m_map.find(sceneId);
		if (dataIter == m_map.end())
		{
			continue;
		}
		auto bounds = dataIter->second;
		state = PosToBounds(bounds->bounds, vec, bounds->distance);
		if (state == 2)
		{
			m_active2List.push_back(sceneId);
		}
		else if (state == 1)
		{
			m_temp1List.push_back(sceneId);
		}
		else {
			m_temp0List.push_back(sceneId);
		}
	}
}

static float m_minX;
static float m_minZ;
static int m_nodeWidth;
static int m_maxWidthNum;
static int m_maxLengthNum;

NATIVE_LIB_DLL void NATIVE_LIB_API Scene_InitData(int nodeWidget, int MaxWidgetNum, int MaxLengthNum, float minX, float minZ)
{
	m_nodeWidth = nodeWidget;
	m_minX = minX;
	m_minZ = minZ;
	m_maxLengthNum = MaxLengthNum;
	m_maxWidthNum = MaxWidgetNum;
}

NATIVE_LIB_DLL int NATIVE_LIB_API Scene_Update(float x, float y, float z) {

	m_active2List.clear();
	Vector3 heroPos(x, y, z);
	m_executeSet.clear();
	Scene_HandleList(m_active0List, heroPos);
	Scene_HandleList(m_active1List, heroPos);
	int curWidhtNum = (int)((x - m_minX) / m_nodeWidth);
	int curLengthNum = (int)((z - m_minZ) / m_nodeWidth);
	auto disIter = m_nodeDic.begin();
	auto endIter = m_nodeDic.end();
	int LoadNum = 1;
	int state;
	for (; disIter != endIter; disIter++)
	{
		LoadNum = (int)((disIter->first + m_nodeWidth -1) / m_nodeWidth);
		LoadNum = LoadNum < 4 ? LoadNum : 4;
		int minWidght = curWidhtNum - LoadNum > 0 ? curWidhtNum - LoadNum : 0;
		int MaxWidght = curWidhtNum + LoadNum >= m_maxWidthNum ? m_maxWidthNum - 1 : curWidhtNum + LoadNum;

		int minLength = curLengthNum - LoadNum > 0 ? curLengthNum - LoadNum : 0;
		int MaxLength = curLengthNum + LoadNum >= m_maxLengthNum ? m_maxLengthNum - 1 : curLengthNum + LoadNum;
		for (int i = minWidght; i <= MaxWidght; i++)
		{
			auto xIter = disIter->second->find(i);
			if (xIter == disIter->second->end() || xIter->second == NULL)
			{
				continue;
			}
			for (int j = minLength; j <= MaxLength; j++)
			{
				auto yIter = xIter->second->find(j);
				if (yIter == xIter->second->end() || yIter->second == NULL)
				{
					continue;
				}
				for (size_t m = 0; m < yIter->second->size(); m++)
				{
					auto prefabId = yIter->second->at(m);
					if (m_executeSet.find(prefabId) != m_executeSet.end())
					{
						continue;
					}
					m_executeSet.insert(prefabId);

					auto dataIter = m_map.find(prefabId);
					if (dataIter == m_map.end())
					{
						continue;
					}
					auto bounds = dataIter->second;
					state = PosToBounds(bounds->bounds, heroPos, bounds->distance);
					if (state == 2)
					{
						continue;
					}
					else if (state == 1)
					{
						m_temp1List.push_back(prefabId);
					}
					else {
						m_temp0List.push_back(prefabId);
					}
				}
			}
		}
	}

	auto tempList = m_temp0List;
	m_temp0List = m_active0List;
	m_active0List = tempList;

	tempList = m_temp1List;
	m_temp1List = m_active1List;
	m_active1List = tempList;
	m_temp0List.clear();
	m_temp1List.clear();
	m_executeSet.clear();
	return m_active0List.size() + m_active1List.size() + m_active2List.size();
}
NATIVE_LIB_DLL void NATIVE_LIB_API Scene_Get(int value[], int& activeLeng, int & loadResourceLenth, int & disableLength) {
	int index = 0;
	for (size_t i = 0; i < m_active0List.size(); i++)
	{
		value[index] = m_active0List[i];
		index++;
	}
	for (size_t i = 0; i < m_active1List.size(); i++)
	{
		value[index] = m_active1List[i];
		index++;
	}
	for (size_t i = 0; i < m_active2List.size(); i++)
	{
		value[index] = m_active2List[i];
		index++;
	}
	activeLeng = m_active0List.size();
	loadResourceLenth = m_active1List.size();
	disableLength = m_active2List.size();
}
NATIVE_LIB_DLL void NATIVE_LIB_API Scene_Clear() {
	m_executeSet.clear();
	m_active0List.clear();
	m_active1List.clear();
	m_temp1List.clear();
	m_temp0List.clear();
	m_active2List.clear();
	auto iterCur = m_map.begin();
	auto iterEnd = m_map.end();
	for (; iterCur != iterEnd; ++iterCur)
	{
		auto pAttribute = iterCur->second;
		SafeDelete(pAttribute);
	}
	m_map.clear();
	auto dicIter = m_nodeDic.begin();
	auto dicEnd = m_nodeDic.end();
	for (; dicIter != dicEnd; ++dicIter)
	{
		auto yIter = dicIter->second->begin();
		auto yEnd = dicIter->second->end();
		for (; yIter != yEnd; yIter++)
		{
			yIter->second->clear();
			SafeDelete(yIter->second);
		}
		dicIter->second->clear();
		SafeDelete(dicIter->second);
	}
	m_nodeDic.clear();
}