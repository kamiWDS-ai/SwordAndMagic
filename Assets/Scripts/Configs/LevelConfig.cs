using UnityEngine;

[CreateAssetMenu(menuName = "Level/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("基础信息")]
    public string levelName;

    [Header("卡牌尺寸")]
    public float cardWidth = 1f;
    public float cardHeight = 1f;

    [Header("网格塔区")]
    public GridLayoutConfig gridArea;

    [Header("密集堆叠区")]
    public DenseStackLayoutConfig denseArea;

    [Header("卡牌内容")]
    public int cardTypeCount = 10;
    public int minCardCount = 10;
}