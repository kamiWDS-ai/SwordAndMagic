using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotView : Singleton<SlotView>
{
    private SlotManager slotManager;
    [Header("槽位点")]
    [SerializeField] private List<Transform> slotPoints;

    [Header("动画参数")]
    [SerializeField] private float moveDuration = 0.25f;

    [Header("移除功能相关")]
    private bool isRemoveUsed = false;
    [SerializeField] private Transform removeRoot;
    [SerializeField] private float removeSpacing = 1f;//cardWidth
    private List<Card> removedCards = new();

    [Header("撤回功能相关")]
    private Card lastUsedCard;
    
    [Header("移动速度")]
    private Coroutine moveCoroutine;
    
    private void Start()
    {
        slotManager = SlotManager.Instance;
        if (slotManager != null)
        {
            slotManager.OnSlotChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (slotManager != null)
        {
            slotManager.OnSlotChanged -= Refresh;
        }
    }


    /// <summary>
    /// 根据 SlotManager 中的卡牌顺序刷新显示
    /// </summary>
    /// 
    public void Refresh(IReadOnlyList<Card> cards)
    {
        if (cards == null || cards.Count == 0)
            OnAllMoveFinished();
            
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            Transform targetPoint = slotPoints[i];

            bool isLast = (i == cards.Count - 1);

            card.MoveTo(
                targetPoint.position,
                moveDuration,
                isLast ? OnAllMoveFinished : null
            );
        }
    }
    private void OnAllMoveFinished()
    {
        slotManager.OnCardMoveFinished();
    }
    
    /// <summary>
    /// 一次性移除所有槽位卡牌
    /// </summary>
    public void RemoveAllCards()
    {
        //只能使用一次
        if (isRemoveUsed)
            return;

        if (slotManager == null)
            return;

        if (slotManager.SlotCards.Count == 0)
            return;

        // 1️⃣ 取出所有卡牌
        List<Card> cards = slotManager.PopAllCards();

        // 2️⃣ 保存到移除列表（用于以后撤回）
        removedCards.AddRange(cards);

        // 3️⃣ 移动到移除区域
        MoveAllToRemovedArea(cards);
        
        isRemoveUsed = true;
        removedCards.Clear();
    }

    private void MoveAllToRemovedArea(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int index = removedCards.Count - cards.Count + i;

            Vector3 targetPos =
                removeRoot.position +
                new Vector3(index * removeSpacing, 0, 0);

            cards[i].MoveTo(targetPos, moveDuration, null);
            
            cards[i].Data.spawnData.position = targetPos;
        }
    }

    /// <summary>
    /// 撤回最近一张牌
    /// </summary>
    public void WithdrawCard()
    {
        Card card = slotManager.WithdrawCard();
    
        if (card == null)
            return;
    
        CardSpawnData spawnData = slotManager.lastCardSpawnData;
    
        card.SetLayer(spawnData.layer);
    
        card.MoveTo(spawnData.position, moveDuration, null);
    }
}