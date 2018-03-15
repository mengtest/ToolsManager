using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[RegisterSystem(typeof(UMengSys), true)]
public class UMengSys : TCoreSystem<UMengSys>, IInitializeable
{
    public static UMengSys TInstance
    {
        get { return Instance; }
    }
    private string str = "result";
    private bool m_isLoginSuc = false;
    private TimeValue m_isSharing = new TimeValue(); //分享中
    private TimeValue m_isLoging = new TimeValue();  //登陆中 给5S保护 防止重复点 也防止异常不能点了
    //等待时间
    public static float waitTime = 0.01f;

    public int JPGQuality = 85; //转换成的JPG质量
    public float TextureScale = 0;//要压缩的图片比例
    public float TextureTargetHeight = 720.0f; //转换的最终高度

    public void Init()
    {
        m_isLoginSuc = false;
    }

    public void Release()
    {

    }

    public void Logout()
    {
        m_isLoginSuc = false;

        if (Application.platform == RuntimePlatform.WindowsEditor) 
        {
            return;
        }
        Social.AuthDelegate authcallback = delegate(Platform platform, int stCode, Dictionary<string, string> message)
        {
            Debug.Log("Logout =" + (stCode == Social.SUCCESS));
        };
        Social.DeleteAuthorization(Platform.WEIXIN, authcallback);
    }

    //微信登陆
    public void WeiXinLogin()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return;
        }

        if (m_isLoging.Value > 0)
        {
            return;
        }
        m_isLoging.Value = 5;

        if (m_isLoginSuc)
        {
            EventSys.Instance.AddEvent(EEventType.WeiXinLoginEnd, true, "isLogined");
        }
        else
        {
            EventSys.Instance.AddEventNow(EEventType.ShowWaitWindow,true,2);
            Social.AuthDelegate authcallback = delegate(Platform platform, int stCode, Dictionary<string, string> message)
            {
                EventSys.Instance.AddEventNow(EEventType.ShowWaitWindow, false);
                bool success = false;
                string resStr = "{";
                if (stCode == Social.SUCCESS)
                {
                    m_isLoginSuc = true;
                    str = "success";
                    foreach (KeyValuePair<string, string> kv in message)
                    {
                        string n = kv.Key;
                        string s = kv.Value;
                        resStr = EZFunString.LinkString(resStr, n, "=\"", s, "\"", ",");
                    }
                    resStr = EZFunString.LinkString(resStr,"}");
                    success = true;
                }
                else
                {
                    m_isLoginSuc = false;
                    str = "WeiXinLogin fail!";
                    success = false;
                };
                //Debug.LogError("str =" + resStr);

                EventSys.Instance.AddEvent(EEventType.WeiXinLoginEnd, success, resStr);
            };
            Social.Authorize(Platform.WEIXIN, authcallback);
        }
    }

    //直接分享本地图片 不下载
    public void WeiXinShareLocalUrl(Platform myPlatform, string text, string title, string picUrl, string targeturl, LuaInterface.LuaFunction luaFunc)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return;

        Social.ShareDelegate sharecallback = delegate(Platform platform, int stCode, string errorMsg)
        {
            if (luaFunc != null)
                luaFunc.Call(stCode == Social.SUCCESS, stCode);
        };
        Social.DirectShare(myPlatform, text, picUrl, title, targeturl, sharecallback);
    }


    //加载图片 分离功能
    public void LoadPic(string picUrl, string picPath, LuaInterface.LuaFunction luaFunc) 
    {
        GameRoot.Instance.StartCoroutine(LoadImgIEn(picUrl, picPath, luaFunc));
    }

    IEnumerator LoadImgIEn(string picUrl, string picPath, LuaInterface.LuaFunction luaFunc)
    {
        //开始下载图片
        var www = new WWW(picUrl);
        yield return www;
        //下载完成，保存图片到路径filePath
        Texture2D texture2D = www.texture;
        if (texture2D.width != 8 || texture2D.height != 8)
        {
            File.WriteAllBytes(picPath, www.bytes);
            luaFunc.Call(true, picPath);
        }
        else 
        {
            luaFunc.Call(false, picPath);
        }
    }

    //获取截图
    public void GetScreenTexturePic(int capx, int capy, int capwidth, int capheight,string picPath, LuaInterface.LuaFunction luaFunc)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return;
        GameRoot.Instance.StartCoroutine(GetScreenTexturePicIen(capx, capy, capwidth, capheight, picPath, luaFunc));
    }

    private IEnumerator GetScreenTexturePicIen(int capx, int capy, int capwidth, int capheight, string picPath, LuaInterface.LuaFunction luaFunc)
    {
        yield return new WaitForEndOfFrame();
        //需要正确设置好图片保存格式
        Texture2D texture2D = new Texture2D(capwidth, capheight, TextureFormat.RGB24, false);
        //按照设定区域读取像素；注意是以左下角为原点读取
        texture2D.ReadPixels(new Rect(capx, capy, capwidth, capheight), 0, 0, false);
      
        if (TextureScale != 1) 
        {
            //不知道是微信还是友盟的原因 图片高超过540就会变得超级大 而且还会最终压到这个尺寸
            if (TextureScale == 0)
                TextureScale = capheight / TextureTargetHeight;

            if (TextureScale != 1)
                texture2D = ScaleTexture(texture2D, (int)(capwidth / TextureScale), (int)(capheight / TextureScale));
        }

        byte[] byt = texture2D.EncodeToJPG(JPGQuality);
        System.IO.File.WriteAllBytes(picPath, byt);
        luaFunc.Call();
    }

    //压缩宽高
    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    //翻转图片
    Texture2D flipPic(Texture2D texture2d)
    {
        int width = texture2d.width;//得到图片的宽度.   
        int height = texture2d.height;//得到图片的高度 

        Texture2D NewTexture2d = new Texture2D(height, width);//创建一张同等大小的空白图片 

        //int i = 0;
        //TODO::是否要做压缩
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                NewTexture2d.SetPixel(i, j, texture2d.GetPixel(width - j - 1, i));
            }
        }
        NewTexture2d.Apply();

        return NewTexture2d;
    }

    //用摄像机截屏
    public void GetScreenTexturePic(Camera camera, int capx, int capy, int capwidth, int capheight, string picPath, LuaInterface.LuaFunction luaFunc)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return;

        Rect rect = new Rect(capx, capy, capwidth, capheight);
        GameRoot.Instance.StartCoroutine(CaptureCamera(camera, rect, picPath, luaFunc));
    }

    //摄像机截屏
    private IEnumerator CaptureCamera(Camera camera, Rect rect, string imgPath, LuaInterface.LuaFunction luaFunc)
    {
        yield return new WaitForEndOfFrame();
        // 创建一个RenderTexture对象
        RenderTexture rt = new RenderTexture(1280, 720, 24);
        //RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 24);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        camera.targetTexture = rt;
        camera.Render();
        yield return new WaitForEndOfFrame();

        // 激活这个rt, 并从中中读取像素。
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();
        screenShot = flipPic(screenShot);
        // 重置相关参数，以使用camera继续在屏幕上显示
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        GameObject.Destroy(rt);
        yield return new WaitForEndOfFrame();
        // 最后将这些纹理数据，成一个png图片文件
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = imgPath;
        File.WriteAllBytes(filename, bytes);
        luaFunc.Call();
    }
}
