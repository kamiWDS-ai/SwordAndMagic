using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/DenseStackLayoutConfig")]
public class DenseStackLayoutConfig : ScriptableObject
{
    public enum SpawnSide
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [System.Serializable]
    public class DenseStackGroup
    {
        [Header("堆叠数量")]
        public int stackCount = 10;

        [Header("生成在网格哪一侧")]
        public SpawnSide spawnSide = SpawnSide.Right;

        [Header("距离网格的额外间距")]
        public float margin = 1.5f;

        [Header("层级起始值")]
        public int startLayerIndex = 100;
    }

    public bool enable = true;

    public List<DenseStackGroup> stacks = new();
}