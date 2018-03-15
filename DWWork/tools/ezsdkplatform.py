import EZSDK
import sys

platform = sys.argv[1]

def ProcessDifPlatform(platform_str):
    print "--platform--:" + platform_str
    if platform_str == "yyb":
        yybDict = EZSDK.LoadingParams("GameData_All.json", "yyb")
        EZSDK.ReplaceFileAppKey("SDK/yyb/app/src/main/assets/ysdkconf.ini", "QQ_APP_ID", str(yybDict["QQ_APP_ID"]))
        EZSDK.ReplaceFileAppKey("SDK/yyb/app/src/main/assets/ysdkconf.ini", "WX_APP_ID", str(yybDict["WX_APP_ID"]))
        EZSDK.ReplaceFileAppKey("SDK/yyb/app/src/main/assets/ysdkconf.ini", "OFFER_ID", str(yybDict["OFFER_ID"]))
        EZSDK.MoveFile("SDK/yyb/app/src/main/java/com/tencent/tmgp/yyb/","SDK/yyb/app/src/main/java/" + yybDict["PackageName"].replace(".","/"))
        EZSDK.ReplaceStringInfo("SDK/yyb/app/src/main/java/" + yybDict["PackageName"].replace(".","/"), "com.tencent.tmgp.yyb", str(yybDict["PackageName"]))
        EZSDK.ReplaceFileNameStrInfo("SDK/yyb/app/src/main/AndroidManifest.xml", "com.tencent.tmgp.yyb", str(yybDict["PackageName"]))
        EZSDK.ReplaceFileNameStrInfo("SDK/yyb/app/src/main/AndroidManifest.xml", "GameName", str(yybDict["GameName"]))
        EZSDK.ReplaceFileNameStrInfo("SDK/yyb/app/src/main/AndroidManifest.xml", "tencent_appid", "tencent" + str(yybDict["QQ_APP_ID"]))
        EZSDK.ReplaceFileNameStrInfo("SDK/yyb/app/src/main/AndroidManifest.xml", "wechat_appid", str(yybDict["WX_APP_ID"]))
        EZSDK.ReplaceFileAppKey_Blank("SDK/yyb/app/build.gradle", "applicationId", "\"" + str(yybDict["PackageName"]) + "\"")
        EZSDK.ReplaceFileAppKey_Blank("SDK/yyb/app/build.gradle", "versionCode", str(yybDict["versionCode"]))
        EZSDK.ReplaceFileAppKey_Blank("SDK/yyb/app/build.gradle", "versionName", "\"" + str(yybDict["versionName"]) + "\"")
        EZSDK.BuildPlatformList("GameData_All.json", "yyb")
        EZSDK.WriteGameDataConfig("../Assets/StreamingAssets/GameData.cf")
        EZSDK.EncrypteGameDate("../Assets/StreamingAssets/GameData.cf")
    if platform_str == "tiantuo_android":
        yybDict = EZSDK.LoadingParams("GameData_All.json", "tiantuo_android")
        EZSDK.ReplaceFileAppKey("SDK/tiantuo_android/app/src/main/assets/sjoys_app.ini", "app_id", str(yybDict["app_id"]))
        EZSDK.ReplaceFileAppKey("SDK/tiantuo_android/app/src/main/assets/sjoys_app.ini", "cch_id", str(yybDict["cch_id"]))
        EZSDK.ReplaceFileAppKey("SDK/tiantuo_android/app/src/main/assets/sjoys_app.ini", "md_id", str(yybDict["md_id"]))
        EZSDK.MoveFile("SDK/tiantuo_android/app/src/main/java/com/starjoys/demo/","SDK/tiantuo_android/app/src/main/java/" + yybDict["PackageName"].replace(".","/"))
        EZSDK.ReplaceStringInfo("SDK/tiantuo_android/app/src/main/java/" + yybDict["PackageName"].replace(".","/"), "com.starjoys.demo", str(yybDict["PackageName"]))
        EZSDK.ReplaceFileNameStrInfo("SDK/tiantuo_android/app/src/main/AndroidManifest.xml", "com.starjoys.demo", str(yybDict["PackageName"]))
        EZSDK.ReplaceFileNameStrInfo("SDK/tiantuo_android/app/src/main/AndroidManifest.xml", "GameName", str(yybDict["GameName"]))
        EZSDK.ReplaceFileAppKey_Blank("SDK/tiantuo_android/app/build.gradle", "applicationId", "\"" + str(yybDict["PackageName"]) + "\"")
        EZSDK.ReplaceFileAppKey_Blank("SDK/tiantuo_android/app/build.gradle", "versionCode", str(yybDict["versionCode"]))
        EZSDK.ReplaceFileAppKey_Blank("SDK/tiantuo_android/app/build.gradle", "versionName", "\"" + str(yybDict["versionName"]) + "\"")
        EZSDK.ReplaceFileAppKey("SDK/tiantuo_android/app/src/main/java/" + yybDict["PackageName"].replace(".","/") + "/PlatformSDK.java", "m_strAppkey", "\"" + str(yybDict["app_key"]) + "\";")
        EZSDK.BuildPlatformList("GameData_All.json", "tiantuo_android")
        EZSDK.WriteGameDataConfig("../Assets/StreamingAssets/GameData.cf")
        EZSDK.EncrypteGameDate("../Assets/StreamingAssets/GameData.cf")
    if platform_str == "tiantuo_iOS":
        EZSDK.BuildPlatformList("GameData_All.json", "tiantuo_iOS")
        EZSDK.WriteGameDataConfig("../Assets/StreamingAssets/GameData.cf")
        EZSDK.EncrypteGameDate("../Assets/StreamingAssets/GameData.cf")
    if platform_str == "anzhi":
        EZSDK.BuildPlatformList("GameData_All.json", "anzhi")
        EZSDK.WriteGameDataConfig("../Assets/StreamingAssets/GameData.cf")
        EZSDK.EncrypteGameDate("../Assets/StreamingAssets/GameData.cf")
        
def ProcessDifPlatformGameData(platform_str):
    print "--platform--:" + platform_str
    yybDict = EZSDK.LoadingParams("GameData_All.json", platform_str)
    EZSDK.BuildPlatformList("GameData_All.json", platform_str)
    EZSDK.WriteGameDataConfig("SDK/" + platform_str + "/app/src/main/assets/GameData.cf")
    EZSDK.EncrypteGameDate("SDK/" + platform_str + "/app/src/main/assets/GameData.cf")

if __name__ == "__main__":
    ProcessDifPlatform(platform)