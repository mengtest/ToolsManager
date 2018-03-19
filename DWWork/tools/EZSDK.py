#coding:utf-8
import re
import json
import os
import shutil
import sys
import zlib
import struct
from Crypto.Cipher import DES
from Crypto.Cipher import AES
import binascii

reload(sys)
sys.setdefaultencoding('utf-8')

#替换某一个文件的key的值
def  ReplaceFileAppKey(szFileName, szKey, szReplaced):
    print "--ReplaceFileAppKey--" + szFileName + ";" + szKey + ";" + szReplaced
    fileOpen = open(szFileName)
    szFileContent = fileOpen.read()
    fileOpen.close()

    szTarget = ""
    match = re.findall(szKey + r'=(.*)', szFileContent)

    if match:
        for item in match:
            szTarget = item
        pass

    if szTarget == "":
        return

    szReplacedFileContent = szFileContent.replace(szKey + "=" + szTarget, szKey + "=" + szReplaced)

    fileWrite = open(szFileName, 'w')
    fileWrite.write(szReplacedFileContent)
    fileWrite.close()
    pass

#替换某一个文件的key的值(替换的是没有等号的)
def  ReplaceFileAppKey_Blank(szFileName, szKey, szReplaced):
    print "--ReplaceFileAppKey--" + szFileName + ";" + szKey + ";" + szReplaced
    fileOpen = open(szFileName)
    szFileContent = fileOpen.read()
    fileOpen.close()

    szTarget = ""
    match = re.findall(szKey + r' (.*)', szFileContent)

    if match:
        for item in match:
            szTarget = item
        pass

    if szTarget == "":
        return

    szReplacedFileContent = szFileContent.replace(szKey + " " + szTarget, szKey + " " + szReplaced)

    fileWrite = open(szFileName, 'w')
    fileWrite.write(szReplacedFileContent)
    fileWrite.close()
    pass

#替换某个文件夹下所有文件的值
def ReplaceStringInfo(szPath, oldString, newString):
    if oldString == newString:
        return
    print "--ReplaceStringInfo--" + szPath + ";" + oldString + ";" + newString
    ListDirsFile(szPath)
    if m_ListDirsFile:
        for fileName in m_ListDirsFile:
            strReplace = fileName.replace("\\", "/")
            fileOpen = open(strReplace)
            szFileContent = fileOpen.read()
            fileOpen.close()
            
            szReplacedFileContent = szFileContent.replace(oldString, newString)
            fileWrite = open(strReplace, 'w')
            fileWrite.write(szReplacedFileContent)
            fileWrite.close()

#替换某个文件的某个值
def ReplaceFileNameStrInfo(fileName, oldString, newString):
    print "--ReplaceFileNameStrInfo--" + fileName + ";" + oldString + ";" + newString
    fileOpen = open(fileName)
    szFileContent = fileOpen.read()
    fileOpen.close()

    szReplacedFileContent = szFileContent.replace(oldString, newString)
    fileWrite = open(fileName, 'w')
    fileWrite.write(szReplacedFileContent)
    fileWrite.close()

#加载配置的渠道信息
def LoadingParams(szFilePath, szType):
    print "--LoadingParams--" + szFilePath + ";" + szType
    fileParam = open(szFilePath)
    szFileContent = fileParam.read()
    fileParam.close()

    dictConfig = json.loads(szFileContent)

    dicSDK = {}
    objList = dictConfig["EZSDK"]
    for item in objList:
        if item['type'] == szType:
            dicSDK = item
        pass

    return dicSDK

#需要打包的渠道信息
def BuildPlatformList(szFilePath, *listSDK):
    if listSDK:
        for sdk in listSDK:
            gameData = LoadingParams(szFilePath, sdk)
            m_listGameDataConfig.append(gameData)
    print "BuildPlatformList" + str(m_listGameDataConfig)
    return m_listGameDataConfig

#写入需要打包的渠道信息内容
def WriteGameDataConfig(strConfigPath):
    ishas = os.path.exists(strConfigPath)
    if ishas:
        os.remove(strConfigPath)
        pass
    WriteConfig = open(strConfigPath, 'w')
    WriteConfig.writelines("{")
    WriteConfig.writelines("\n" + "\"EZSDK\":")
    WriteConfig.writelines("\n" + "[")
    if m_listGameDataConfig:
        count = 0
        for sdk in m_listGameDataConfig:
            count += 1
            WriteConfig.writelines("\n" + "{")
            WriteConfig.writelines("\n" + "\"GameID\":" + "\"" + sdk["GameID"] + "\"" + ",")
            WriteConfig.writelines("\n" + "\"type\":" + "\"" + sdk["type"] + "\"" + ",")
            WriteConfig.writelines("\n" + "\"PackageName\":" + "\"" + sdk["PackageName"] + "\"" + ",")
            WriteConfig.writelines("\n" + "\"Https\":" + "\"" + sdk["Https"] + "\"" + ",")
            WriteConfig.writelines("\n" + "\"Https_Debug\":" + "\"" + sdk["Https_Debug"] + "\"" + ",")
            WriteConfig.writelines("\n" + "\"LoginChannelID\":" + "\"" + sdk["LoginChannelID"] + "\"" + ",")
            WriteConfig.writelines("\n" + "\"PayChannelID\":" + "\"" + sdk["PayChannelID"] + "\"")
            if len(m_listGameDataConfig) == count:
                WriteConfig.writelines("\n" + "}")
            else:
               WriteConfig.writelines("\n" + "},")
    WriteConfig.writelines("\n" + "]");
    WriteConfig.writelines("\n" + "}");
    WriteConfig.close()

#创建目录	
def MakeDir(dirpath):
	ishas = os.path.exists(dirpath)
	if not ishas:
		os.makedirs(dirpath)

#移除文件
def Remove(dirpath):
    print "--Remove--" + dirpath
    if os.path.exists(dirpath):
        shutil.rmtree(dirpath)

#移动文件		
def MoveFile(orgin_path, target_path):
    print "--MoveFile--" + orgin_path + ";" + target_path
    if orgin_path == target_path :
        return
    ishas = os.path.exists(target_path)
    if ishas:
        shutil.rmtree(target_path)
    if os.path.exists(orgin_path):
        shutil.move(orgin_path, target_path)

#遍历文件夹下面的所有文件，包括子目录
def ListDirsFile(dirpath):
    print "--ListDirsFile--" + dirpath
    for filename in os.listdir(dirpath):
        child = os.path.join(dirpath, filename)
        if (os.path.isdir(child)):
            ListDirsFile(child)
        else:
            m_ListDirsFile.append(child)

#加密读取的渠道配置文件
def EncrypteGameDate(fileName):
    strIV = "dd7fd4a156d28bade96f816db1d18609".decode("hex")
    strKey = "dd7fd4a156d28bade96f816db1d18609".decode("hex")
    cipher = AES.new(strKey, AES.MODE_CBC, strIV)
    file = open(fileName, "r") 
    try:
        source_data = file.read()
        print "--Encrypte--" + source_data
    finally:
        file.close()
        print "--Encrypte--end"
    length = len(source_data)
    x = length % 16
    if x != 0:
        source_data = source_data + '0'*(16 - x)
    edata = cipher.encrypt(source_data)
    edata = struct.pack("!l", length) + edata
    WriteConfig = open(fileName, 'wb+')
    WriteConfig.write(edata)
    WriteConfig.close()

#保存递归查找的文件
m_ListDirsFile = []	
#保存当前打包的渠道列表
m_listGameDataConfig = []
