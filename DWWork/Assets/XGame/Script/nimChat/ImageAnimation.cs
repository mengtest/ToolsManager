
using UnityEngine;
using UnityEngine.UI;


public class ImageAnimation : SpriteAnimation
{
    Image mImage;


    protected override void DoAwake()
    {
        base.DoAwake();

        mImage = GetComponent<Image>();
    }

    protected override void SetSprite(Sprite sprite)
    {
        if (mImage != null)
            mImage.sprite = sprite;
    }

    protected override void SetAlpha(float alpha)
    {
        if (mImage != null)
        {
            Color color = mImage.color;
            color.a = alpha;
            mImage.color = color;
        }
    }
}
