
using UnityEngine;


public class SpriteExtension
{
    static Vector2 CenterPivot = new Vector2(0.5f, 0.5f);


    static public Sprite Create(UIAtlas atlas, string name)
    {
        if (atlas == null)
        {
            Debug.LogWarningFormat("SpriteExtension.Create  失败  Atlas : null, Sprite : {0}", name);
            return null;
        }

        Texture2D texture = (Texture2D)atlas.texture;
        UISpriteData spriteData = atlas.GetSprite(name);
        if (texture == null || spriteData == null)
        {
            Debug.LogWarningFormat("SpriteExtension.Create  失败  Atlas : {0}, Sprite : {1}", atlas.name, name);
            return null;
        }

        Rect rect = new Rect();
        rect.x = spriteData.x;
        rect.y = texture.height - spriteData.y - spriteData.height;
        rect.width = spriteData.width;
        rect.height = spriteData.height;

        Vector4 border = new Vector4();
        border.x = spriteData.borderLeft;
        border.y = spriteData.borderBottom;
        border.z = spriteData.borderRight;
        border.w = spriteData.borderTop;

        Sprite sprite = null;

        try
        {
            sprite = Sprite.Create(texture, rect, CenterPivot, 100f, 0, SpriteMeshType.FullRect, border);
        }
        catch (System.Exception)
        {
            Debug.LogWarningFormat("SpriteExtension.Create  异常  Atlas : {0}, Sprite : {1}", atlas.name, name);
        }

        return sprite;
    }
}
