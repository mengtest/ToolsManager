using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LocalPrefabData
{

    #region 数据工具
    public class PrefabDataTools
    {
        #region 数据应用到Transform
        public static void ApplyPrefabData(PrefabData data, Transform trans, TableLoader resInstance)
        {
            data.nodes.Sort(delegate(NodeData data1, NodeData data2)
            {
                return data1.depth - data2.depth;
            });
            //uilabel,uisprite,transform等
            for (int i = 0; i < data.nodes.Count; i++)
            {
                string name = data.nodes[i].name;
                if (name.StartsWith("/"))
                    name = name.Substring(1);
                Transform childTrans = trans.Find(name);
                if (childTrans != null)
                {
                    ApplyNodeData(data.nodes[i], (childTrans));
                }
            }
            if (data.labelIds != null)
            {
                //文本修改
                for (int i = 0; i < data.labelIds.Count; i++)
                {
                    string pathName = data.labelIds[i].labelPath;
                    int textId = data.labelIds[i].labelID;
                    Transform childTrans = trans.Find(pathName);
                    if (childTrans != null)
                    {
                        UILabel label = childTrans.GetComponent<UILabel>();
                        string text = GetText(textId, resInstance);
                        if (label != null && !string.IsNullOrEmpty(text))
                        {
                            label.text = text;
                        }
                    }
                }
            }
        }

        public static string GetText(int id, TableLoader resInstance)
        {
            string text = "";

            ezfun_resource.ResText resText = (ezfun_resource.ResText)resInstance.GetEntry<ezfun_resource.ResTextList>(id);
            if (resText != null)
            {
                text = resText.text.Replace("\\n", "\n");
            }
            else
            {
                text = "";
            }
            return text;
        }

        private static void ApplyNodeData(NodeData data, Transform trans)
        {

            if (data.uiLableData != null)
            {
                ApplyUILable(data.uiLableData, trans);
            }
            if (data.uiSpriteData != null)
            {
                ApplySpriteData(data.uiSpriteData, trans);
            }
            UIRect rect = trans.GetComponent<UIRect>();
            //上面这句话的意思，如果有锚点的话，那么就不要移动位置了，用原来的就好
            //因为在实际游戏中修改的prefab，而修改了prefab，那么就会导致相对位置改了
            if (data.transformData != null && ( rect == null ||  (rect != null && !rect.isAnchored)))
            {
                ApplyTransformData(data.transformData, trans);
            }
        }



        private static void ApplyUILable(UILabelFixData data, Transform mono)
        {
            if (mono == null)
            {
                return;
            }
            //base.ApplyData(mono);
            if (mono == null)
            {
                return;
            }
            UIWidget widget = mono.GetComponent<UIWidget>();
            UILabel label = mono.GetComponent<UILabel>();
            if ((UILabel.Overflow)data.mOverflow != UILabel.Overflow.ShrinkContent || data.hasUILable)
            {
                label.overflowMethod = (UILabel.Overflow)data.mOverflow;
            }
            if (widget != null)
            {
                //怕之后添加字段，如果用默认值，那么就不给赋值
                // if (color != ColorDouble.white)
                {
                    //      widget.color = color.ToColor();
                }
                if (data.AspectType != -1 || data.hasWidget)
                {
                    widget.keepAspectRatio = (UIWidget.AspectRatioSource)data.AspectType;
                    if ((UIWidget.AspectRatioSource)data.AspectType == UIWidget.AspectRatioSource.Free)
                    {
                        widget.height = data.height;
                        widget.width = data.width;
                    }
                    else if ((UIWidget.AspectRatioSource)data.AspectType == UIWidget.AspectRatioSource.BasedOnWidth)
                    {
                        widget.width = data.width;
                        widget.aspectRatio = (float)data.aspectRatio;
                        widget.height = (int)(data.aspectRatio * data.width);
                    }
                    else
                    {
                        widget.height = data.height;
                        widget.aspectRatio = (float)data.aspectRatio;
                        widget.width = (int)(data.aspectRatio * data.height);
                    }
                }
                if (data.depth != -1 || data.hasWidget)
                {
                    widget.depth = data.depth;
                }
                if (data.color != null)
                {
                    widget.color = PrefabDataTools.ColorDoubleToColor(data.color);
                }

                if (data.pivot != -1 || data.hasWidget)
                {
                    widget.pivot = (UIWidget.Pivot)data.pivot;
                }
                bool isActive = widget.enabled;
                widget.enabled = false;
                widget.enabled = true;
                widget.enabled = isActive;

            }
            //UILabel label = mono.GetComponent<UILabel>();
            if (label != null)
            {
                if (data.fontSize != 16 || data.hasUILable)
                {
                    label.fontSize = data.fontSize;
                }
                if ((UILabel.Overflow)data.mOverflow != UILabel.Overflow.ShrinkContent || data.hasUILable)
                {
                    label.overflowMethod = (UILabel.Overflow)data.mOverflow;
                }
                if (data.mSpacingX != 0 || data.hasUILable)
                {
                    label.spacingX = data.mSpacingX;
                }
                if (data.mSpacingY != 0 || data.hasUILable)
                {
                    label.spacingY = data.mSpacingY;
                }
                if (data.aspectRatio != 0.0 || data.hasUILable)
                {
                    label.aspectRatio = (float)data.aspectRatio;
                }
                if (data.keepCrisp != (int)UILabel.Crispness.OnDesktop || data.hasUILable)
                {
                    label.keepCrispWhenShrunk = (UILabel.Crispness)data.keepCrisp;
                }
                if (data.alignMent != (int)NGUIText.Alignment.Left || data.hasUILable)
                {
                    label.alignment = (NGUIText.Alignment)data.alignMent;
                }
                if (data.gradientBottom != null)
                {
                    label.gradientBottom = PrefabDataTools.ColorDoubleToColor(data.gradientBottom);
                }
                if (data.gradientTop != null)
                {
                    label.gradientTop = PrefabDataTools.ColorDoubleToColor(data.gradientTop);
                }
                label.applyGradient = data.isGradient;
            }
            bool isLablActive = label.enabled;
            label.enabled = false;
            label.enabled = true;
            label.enabled = isLablActive;
        }


        private static void ApplyTransformData(TransformFixData data, Transform mono)
        {
            if (mono == null)
            {
                return;
            }
            //base.ApplyData(mono);
            if (mono is Transform)
            {
                Transform trans = mono as Transform;
                if (data.localPos != null && data.localPos != PrefabDataTools.zero)
                {
                    trans.localPosition = PrefabDataTools.Vector3DoubleToVecter3(data.localPos);

                }
                if (data.localRotation != null && data.localRotation != PrefabDataTools.identity)
                {
                    trans.localRotation = PrefabDataTools.QuaternionDoubleToQuaternion(data.localRotation);
                }
                if (data.localScale != null && data.localScale != PrefabDataTools.zero)
                {
                    trans.localScale = PrefabDataTools.Vector3DoubleToVecter3(data.localScale);
                }
            }
        }

        private static void ApplySpriteData(UIFixSpriteData data, Transform mono)
        {
            if (mono == null)
            {
                return;
            }
            if (mono == null)
            {
                return;
            }
            UIWidget widget = mono.GetComponent<UIWidget>();
            if (widget != null)
            {
                //怕之后添加字段，如果用默认值，那么就不给赋值
                // if (color != ColorDouble.white)
                {
                    //      widget.color = color.ToColor();
                }

                if (data.AspectType != -1 || data.hasWidget)
                {
                    widget.keepAspectRatio = (UIWidget.AspectRatioSource)data.AspectType;
                    if ((UIWidget.AspectRatioSource)data.AspectType == UIWidget.AspectRatioSource.Free)
                    {
                        widget.height = data.height;
                        widget.width = data.width;
                    }
                    else if ((UIWidget.AspectRatioSource)data.AspectType == UIWidget.AspectRatioSource.BasedOnWidth)
                    {
                        widget.width = data.width;
                        widget.aspectRatio = (float)data.aspectRatio;
                        widget.height = Mathf.RoundToInt(((float)data.width) / widget.aspectRatio);
                        ;

                    }
                    else
                    {
                        widget.height = data.height;
                        widget.aspectRatio = (float)data.aspectRatio;
                        widget.width = Mathf.RoundToInt(((float)data.height) * (float)data.aspectRatio); ;
                    }
                    bool isActive = widget.enabled;
                    widget.enabled = false;
                    widget.enabled = true;
                    widget.enabled = isActive;
                }


                if (data.depth != -1 || data.hasWidget)
                {
                    widget.depth = data.depth;
                }

                if (data.color != null)
                {
                    widget.color = PrefabDataTools.ColorDoubleToColor(data.color);
                }

                if (data.pivot != -1 || data.hasWidget)
                {
                    widget.pivot = (UIWidget.Pivot)data.pivot;
                }

            }
            //base.ApplyData(mono);
            UISprite sprite = mono.GetComponent<UISprite>();
            if (sprite != null)
            {
                if ((UISprite.Type)data.mType != UISprite.Type.Simple || data.hasUISprite)
                {
                    sprite.type = (UISprite.Type)data.mType;
                }
                if (data.mAtlas != "" && data.mSprite != "")
                {
                    var atlas = NGUITools.GetAtlasByName(data.mAtlas);
                    if (atlas != null)
                    {
                        sprite.atlas = atlas;
                    }
                    sprite.spriteName = data.mSprite;
                }

            }
            bool isSpriteActive = sprite.enabled;
            sprite.enabled = false;
            sprite.enabled = true;
            sprite.enabled = isSpriteActive;
        }

        #endregion
        #region unity数据中float与double的转换
        public static Quaternion QuaternionDoubleToQuaternion(QuaternionDouble data)
        {
            Quaternion vec = new Quaternion();
            vec.x = (float)data.x;
            vec.y = (float)data.y;
            vec.z = (float)data.z;
            vec.w = (float)data.w;
            return vec;
        }

        public static QuaternionDouble Quaternion3ToDouble(Quaternion vec)
        {
            QuaternionDouble vecd = new QuaternionDouble();
            vecd.x = vec.x;
            vecd.y = vec.y;
            vecd.z = vec.z;
            vecd.w = vec.w;
            return vecd;
        }

        public static QuaternionDouble identity
        {
            get
            {
                QuaternionDouble mZero = new QuaternionDouble();
                return mZero;
            }
        }


        public static Vector3 Vector3DoubleToVecter3(Vector3Double data)
        {
            Vector3 vec = new Vector3();
            vec.x = (float)data.x;
            vec.y = (float)data.y;
            vec.z = (float)data.z;
            return vec;
        }

        public static Vector3Double Vector3ToDouble(Vector3 vec)
        {
            Vector3Double vecd = new Vector3Double();
            vecd.x = vec.x;
            vecd.y = vec.y;
            vecd.z = vec.z;
            return vecd;
        }

        public static Vector3Double zero
        {
            get
            {
                Vector3Double mZero = new Vector3Double();
                return mZero;
            }
        }


        public static Color ColorDoubleToColor(ColorDouble data)
        {
            Color color = new Color();
            color.a = (float)data.a;
            color.b = (float)data.b;
            color.g = (float)data.g;
            color.r = (float)data.r;
            return color;
        }

        public static ColorDouble ColorToDouble(Color vec)
        {
            ColorDouble vecd = new ColorDouble();
            vecd.r = vec.r;
            vecd.g = vec.g;
            vecd.b = vec.b;
            vecd.a = vec.a;
            return vecd;
        }

        public static ColorDouble white
        {
            get
            {
                ColorDouble mZero = new ColorDouble();
                return mZero;
            }
        }
        #endregion
    }
    #endregion

    #region 数据存储
    public class PrefabData
    {
        public List<NodeData> nodes = new List<NodeData>();

        public List<LabelIDData> labelIds;

        public PrefabData()
        {

        }


    }

    public class LabelIDData
    {
        public string labelPath;
        public int labelID;
    }

    public class NodeData
    {
        public int depth;
        public string name;
        public TransformFixData transformData = null;
        public UILabelFixData uiLableData = null;
        public UIFixSpriteData uiSpriteData = null;
        public NodeData()
        {
        }
    }

    public class UILabelFixData
    {
        public int height = -1;
        public int width = -1;
        public int depth = -1;
        public int pivot = -1;


        public ColorDouble color;
        public double aspectRatio = 0.0;
        public int AspectType = -1;
        public bool hasWidget = false;

        public int fontSize = 16;
        public int mOverflow = (int)UILabel.Overflow.ShrinkContent;
        public int mSpacingX = 0;
        public int mSpacingY = 0;
        public bool isGradient = false;
        public int alignMent = (int)NGUIText.Alignment.Automatic;
        public int keepCrisp = (int)UILabel.Crispness.Always;
        public ColorDouble gradientBottom;
        public ColorDouble gradientTop;
        public bool hasUILable = false;

    }

    public class TransformFixData
    {
        public Vector3Double localPos;
        public QuaternionDouble localRotation;
        public Vector3Double localScale;
        public TransformFixData()
        {
        }

    }

    public class UIFixSpriteData
    {
        public int height = -1;
        public int width = -1;
        public int depth = -1;
        public int pivot = -1;
        public ColorDouble color;
        public double aspectRatio = 0.0;
        public int AspectType = -1;

        public bool hasWidget = false;

        public int mType = (int)UISprite.Type.Simple;
        public bool hasUISprite = false;
        public string mAtlas = "";
        public string mSprite = "";
    }
    #endregion

    #region float数据的替换
    public class Vector3Double2
    {
        public double x;
        public double y;
        public double z;

        public Vector3Double2()
        {

        }
    }


    public class Vector3Double
    {
        public double x;
        public double y;
        public double z;

        public Vector3Double()
        {

        }


    }


    public class ColorDouble
    {
        public double r;
        public double g;
        public double b;
        public double a;


        public ColorDouble()
        {

        }


    }


    public class QuaternionDouble
    {
        public double w;
        public double x;
        public double y;
        public double z;

        public QuaternionDouble()
        {

        }


    }



    #endregion
}

