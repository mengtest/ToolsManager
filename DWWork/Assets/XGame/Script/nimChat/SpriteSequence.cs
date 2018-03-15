using System.Collections.Generic;
using UnityEngine;


public class SpriteSequence
{
    static Dictionary<string, SpriteSequence> sSpriteSequences = new Dictionary<string, SpriteSequence>();

    static public SpriteSequence Create(UIAtlas atlas, string name, int begin, int end)
    {
        if (sSpriteSequences.ContainsKey(name))
            return sSpriteSequences[name];

        SpriteSequence seq = new SpriteSequence(name);

        for (int i = begin; i <= end; ++i)
        {
            Sprite sprite = SpriteExtension.Create(atlas, name + i);
            if (sprite != null)
                seq.mSprites.Add(sprite);
        }

        if (seq.Count > 0)
            sSpriteSequences.Add(name, seq);

        return seq;
    }

    static public void Remove(string name)
    {
        sSpriteSequences.Remove(name);
    }

    static public void Clear()
    {
        sSpriteSequences.Clear();
    }


    List<Sprite> mSprites;
    string mName;

    public int Count { get { return mSprites.Count; } }
    public string Name { get { return mName; } }

    public SpriteSequence(string name)
    {
        mSprites = new List<Sprite>();
        mName = name;
    }

    public void Dispose()
    {
        if (mSprites != null)
        {
            Remove(mName);

            mSprites.Clear();
            mSprites = null;
        }
    }

    public Sprite GetSprite(int index)
    {
        if (index < 0 && index >= mSprites.Count)
            return null;

        return mSprites[index];
    }
}
