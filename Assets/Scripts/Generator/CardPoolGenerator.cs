using System.Collections.Generic;
using UnityEngine;

public class CardPoolGenerator : Singleton<CardPoolGenerator>
{
    [Header("可用卡牌配置库")]
    [Tooltip("所有可能出现的卡牌类型配置")]
    [SerializeField]
    private List<CardConfig> availableCardConfigs;

    /// <summary>
    /// 生成本关卡的卡牌数据池
    /// </summary>
    public List<CardData> Generate(LevelConfig levelConfig,int levelCardCount)
    {
        List<CardData> result = new List<CardData>();

        int totalCount = levelCardCount;
        int typeCount = levelConfig.cardTypeCount;

        // -------- 基础校验 --------
        if (totalCount % 3 != 0)
        {
            Debug.LogError("LevelConfig.totalCardCount 必须是 3 的倍数！");
            return result;
        }

        if (typeCount <= 0 || typeCount > availableCardConfigs.Count)
        {
            Debug.LogError("LevelConfig.cardTypeCount 数量非法或超出 CardConfig 库范围！");
            return result;
        }

        // -------- 选取本关使用的卡牌类型 --------
        List<CardConfig> usedConfigs = PickConfigs(typeCount);

        int groupCount = totalCount / 3;
        int baseGroupPerType = groupCount / typeCount;
        int remainder = groupCount % typeCount;

        // -------- 生成 CardData（三张一组）--------
        for (int i = 0; i < usedConfigs.Count; i++)
        {
            int groupNum = baseGroupPerType;
            if (i < remainder)
                groupNum++;

            for (int j = 0; j < groupNum * 3; j++)
            {
                CardData data = new CardData
                {
                    config = usedConfigs[i],
                    isRemoved = false,
                };

                result.Add(data);
            }
        }

        // -------- 打乱顺序 --------
        Shuffle(result);

        return result;
    }

    #region Helper

    /// <summary>
    /// 从配置库中随机选取本关卡使用的卡牌类型
    /// </summary>
    private List<CardConfig> PickConfigs(int count)
    {
        List<CardConfig> temp = new List<CardConfig>(availableCardConfigs);
        Shuffle(temp);
        return temp.GetRange(0, count);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    #endregion
}
