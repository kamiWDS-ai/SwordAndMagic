using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Grid Layout")]
public class GridLayoutConfig : ScriptableObject
{
    [Header("塔层数")]
    public int layerCount = 4;

    [Header("生成的矩阵参数")]
    public int minRows = 4;
    public int maxRows = 6;

    public int minCols = 4;
    public int maxCols = 6;

    [Header("层级Z间隔")]
    public float layerZStep = 0f;
}