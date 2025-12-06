using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LongNC
{
    public enum ColorId
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        Pink,
        Orange,
    }
    
    [System.Serializable]
    public class BlockType
    {
        public int colorId;
        [ShowInInspector, ReadOnly] public string nameColor => Enum.GetName(typeof(ColorId), colorId);
    }
    
    [System.Serializable]
    public class PileStructure
    {
        public Vector3 defaultPosition;
        public List<BlockType> blocks;
    }
    
    [CreateAssetMenu(fileName = "DataLevel_0", menuName = "Data/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int level;
        public List<int> collectionSlots = new List<int>(Enumerable.Repeat(-1, 5));
        public List<PileStructure> pileStructures =  new List<PileStructure>();
    }
}