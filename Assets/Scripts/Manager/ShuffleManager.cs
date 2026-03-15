using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShuffleManager : Singleton<ShuffleManager>
{
    [SerializeField] private float shuffleDuration = 0.25f;
    public event Action OnShuffleComplete;

    public void Shuffle()
    {
        List<Card> candidates = GetShuffleCandidates();

        if (candidates.Count <= 1)
            return;

        // 1️⃣ 收集所有 SpawnData
        List<CardSpawnData> spawnDatas = new();

        foreach (var card in candidates)
        {
            spawnDatas.Add(card.Data.spawnData);
        }

        // 2️⃣ 打乱 SpawnData
        ShuffleList(spawnDatas);

        // 3️⃣ 重新分配 SpawnData
        for (int i = 0; i < candidates.Count; i++)
        {
            Card card = candidates[i];
            CardSpawnData newData = spawnDatas[i];

            // 更新 SpawnData
            card.Data.spawnData = newData;

            // 更新位置
            card.SetPosition(newData.position);

            // 更新层级
            card.SetLayer(newData.layer);

            // 动画移动(移动完成调用卡牌可选状态刷新)
            card.MoveTo(newData.position, shuffleDuration, i==candidates.Count - 1 ? CardStackChecker.Instance.RefreshAllCardStates : null);
        }
        
    }

    /// <summary>
    /// 获取可洗牌卡牌（不在槽位中的）
    /// </summary>
    private List<Card> GetShuffleCandidates()
    {
        List<Card> result = new();

        var slotCards = SlotManager.Instance.SlotCards;

        foreach (var card in CardRegistry.Instance.AllCards)
        {
            if (!slotCards.Contains(card) && !card.Data.isRemoved)
            {
                result.Add(card);
            }
        }

        return result;
    }

    /// <summary>
    /// Fisher-Yates 洗牌算法
    /// </summary>
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}