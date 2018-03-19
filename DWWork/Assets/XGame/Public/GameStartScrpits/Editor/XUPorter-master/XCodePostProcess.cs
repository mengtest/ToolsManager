using UnityEngine;
using LitJson;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public enum IOSType
{
    dawang,
}
/// <summary>
/// 如果用XCODE7打包，需要修改XCODE3个参数1 BITCODE，2 Objective-c Automatic Refenrece Count要改成NO，要不WebView.mm会报错，3 C++ Standard Libarary要修改成libstdc++
/// </summary>
public static class XCodePostProcess
{
    public static IOSType m_isoType = IOSType.dawang;
    //#endregion

    /// <summary>
    /// 这个脚本不要引用 其他的脚本
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] ReadFileStream(string path)
    {
        byte[] b = null;
        if (File.Exists(path))
        {
            using (Stream file = File.OpenRead(path))
            {
                b = new byte[(int)file.Length];
                file.Read(b, 0, b.Length);
                file.Close();
                file.Dispose();
            }
        }
        if (b == null)
            Debug.LogError("ReadFileStream Read file failed! " + path);
        return b;
    }

#if UNITY_EDITOR
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        var path = Application.dataPath.Substring(0, Application.dataPath.Length - 7);
        byte[] b = ReadFileStream(path + "/tools/iosType.json");
        if (b != null)
        {
            string str = System.Text.Encoding.UTF8.GetString(b);
            Debug.Log(str);
            var json = JsonMapper.ToObject(str);
            int index;
            int.TryParse(json[0].ToString(), out index);
            if (index < Enum.GetNames(typeof(IOSType)).Length)
                m_isoType = (IOSType)index;

            Debug.Log(m_isoType + "   index:" + index);
        }

        // Create a new project object from build target
        XCProject project = new XCProject(pathToBuiltProject);

        // Find and run through all projmods files to patch the project.
        // Please pay attention that ALL projmods files in your project folder will be excuted!
        string[] files = Directory.GetFiles(Application.dataPath, "*.projmods", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.Contains(m_isoType.ToString()))
            {
                UnityEngine.Debug.Log("ProjMod File: " + file);
                project.ApplyMod(file);
            }
        }

        //TODO implement generic settings as a module option
        //project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");

        //add by janus for admob
        path = Path.GetFullPath(pathToBuiltProject);

        EditorPlist(path);
        EditorCode(path);
        EditorXcodeFile(project);
        // Finally save the xcode project
        project.Save();
    }
#endif

    public static void EditorXcodeFile(XCProject xc)
    {
        //这个是在xcode7上需要设置的
        XClass UnityAppController = new XClass(Path.Combine(xc.filePath, "project.pbxproj"));
        UnityAppController.Replace("ENABLE_BITCODE = YES;", "ENABLE_BITCODE = NO;");
        UnityAppController.Replace("GCC_ENABLE_OBJC_EXCEPTIONS = NO;", "GCC_ENABLE_OBJC_EXCEPTIONS = YES;");
    }

    public static void Log(string message)
    {
        UnityEngine.Debug.Log("PostProcess: " + message);
    }
    private static void EditorPlist(string filePath)
    {
        XCPlist list = new XCPlist(filePath);
        string bundle = "com.da.jxqp";
        string PlistAdd = "";

        PlistAdd = @"  
            <key>CFBundleURLTypes</key>
            <array>
            	<dict>
            		<key>CFBundleTypeRole</key>
           			<string>Editor</string>
            		<key>CFBundleURLName</key> 
					<string>" + bundle + @"</string>
           			<key>CFBundleURLSchemes</key>
            		<array>
						<string>da.jxqp</string>
            			<string>jxqp</string>
            			<string>dwfun.jxqp</string>
            		</array>
            	</dict>
                <dict>
           			<key>CFBundleURLSchemes</key>
            		<array>
						<string>wxe5c84117c9a78c66</string>
            		</array>
            	</dict>
            </array>
            <key>LSApplicationQueriesSchemes</key>
            		<array>
						 <!-- 微信 URL Scheme 白名单-->
                        <string>wechat</string>
                        <string>weixin</string>
            		</array>	
			<key>UIFileSharingEnabled</key>
			<true/>
            <key>NSLocationAlwaysUsageDescription</key>
			<string>App需要您的同意，才能始终访问位置，查看附近牌友信息</string>
            <key>NSLocationWhenInUseUsageDescription</key>
            <string>App需要您的同意，才能在使用期间访问位置，查看附近牌友信息</string>";
        //在plist里面增加一行
        list.AddKey(PlistAdd);
        //保存
        list.Save();
    }

    private static void EditorCode(string filePath)
    {
        //读取UnityAppController.mm文件
        XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");
        SetUMeng(UnityAppController);
    }

    private static void SetUMeng(XClass UnityAppController)
    {
        string addCode = "";
        //注册umeng
        addCode = "#import <UMSocialCore/UMSocialCore.h>\n";
        UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"\n", addCode);

        addCode = "[[UMSocialManager defaultManager] openLog:YES];\n";
        // 获取友盟social版本号
        addCode += "NSLog(@\"UMeng social version: %@\", [UMSocialGlobal umSocialSDKVersion]);\n";
        addCode += "[UMSocialGlobal shareInstance].type = @\"u3d\";\n";
        //设置友盟appkey
        addCode += "[[UMSocialManager defaultManager] setUmSocialAppkey:@\"59f971c2aed179051e000089\"];\n";
        //设置微信的appKey和appSecret
        addCode += "[[UMSocialManager defaultManager] setPlaform:UMSocialPlatformType_WechatSession appKey:@\"wxe5c84117c9a78c66\" appSecret:@\"64501286cf69454c51a60da8df03d754\" redirectURL:@\"http://dw7758.com\"]; \n";

        UnityAppController.WriteBelow("[KeyboardDelegate Initialize];\n", addCode);

        addCode = "BOOL result = [[UMSocialManager defaultManager] handleOpenURL:url];";
        UnityAppController.WriteBelow("AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);\n", addCode);
    }

}
