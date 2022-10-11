using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelImage", menuName = "ScriptableObjects/LevelImage")]
public class LevelSO : ScriptableObject
{
    public LangSprite[] langSprite;
    public CompleteSprites[] levelSprites;

    [Serializable]
    public class LangSprite
    {
        public Sprite[] levels;
    }

    [Serializable]
    public class CompleteSprites
    {
        public Sprite[] types;
    }
}
