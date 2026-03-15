using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 底部槽位管理器
/// 负责：
/// 1. 接收选中卡牌
/// 2. 三消判断
/// 3. 失败判定
/// </summary>
public class SlotManager : Singleton<SlotManager>
{
    [Header("槽位配置")]
    [SerializeField] private int maxSlotCount = 7;

    [Header("当前槽内卡牌")]
    [SerializeField] private List<Card> slotCards = new();
    public List<Card> SlotCards => slotCards;
    
    //维护最近被添加的卡牌
    public Card lastCard {get; private set;}
    public CardSpawnData lastCardSpawnData {get; private set;}

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    #region Events

    /// <summary>
    /// 槽位发生变化（用于 UI）
    /// </summary>
    public event Action<IReadOnlyList<Card>> OnSlotChanged;

    /// <summary>
    /// 成功三消
    /// </summary>
    public event Action OnTripleMatched;

    /// <summary>
    /// 槽位满导致失败
    /// </summary>
    public event Action OnSlotOverflow;

    #endregion

    #region Public API

    /// <summary>
    /// 添加一张卡牌到槽位
    /// </summary>
    public void AddCard(Card card)
    {
        if (card == null)
            return;

        if (slotCards.Count >= maxSlotCount)
            return;
        
        //沙盘模式记录选牌,增加步数
        if (SandboxManager.Instance != null &&
            SandboxManager.Instance.IsInSandbox)
        {
            if(!SandboxManager.Instance.StepCheck())
                return;
            SandboxManager.Instance.RecordSelect(card);
            SandboxManager.Instance.StepUpdate(+1);
            SandboxManager.Instance.StepCheck();
        }
        
        //如果不是沙盘模式，则记录最近一张牌，用于撤回功能
        if (SandboxManager.Instance != null &&
            !SandboxManager.Instance.IsInSandbox)
        {
            lastCard = card;
            lastCardSpawnData=card.Data.spawnData;
        }
        
        // 标记为已移除（不再参与遮挡）
        card.Data.isRemoved = true;
        
        slotCards.Add(card);
        
        //播放卡牌被选音效
        audioSource.Play();
        
        //相同卡牌排序
        slotCards.Sort((a, b) =>
            a.Data.config.typeId.CompareTo(b.Data.config.typeId)
        );

        
        OnSlotChanged?.Invoke(slotCards);
    }

    #endregion

    #region Core Logic

    /// <summary>
    /// OnSlotChanged后调用，判断三消和胜负
    /// </summary>
    public void OnCardMoveFinished()
    {
        TryMatchTriple();
        CheckFail();
        CheckWin();
    }

    /// <summary>
    /// 尝试检测并消除三张相同类型
    /// </summary>
    private void TryMatchTriple()
    {
        Dictionary<int, List<Card>> typeMap = new();

        foreach (var card in slotCards)
        {
            int typeId = card.Data.config.typeId;

            if (!typeMap.ContainsKey(typeId))
                typeMap[typeId] = new List<Card>();

            typeMap[typeId].Add(card);
        }

        foreach (var pair in typeMap)
        {
            if (pair.Value.Count >= 3)
            {
                RemoveTriple(pair.Value.GetRange(0, 3));
                return; // 一次只处理一组
            }
        }
    }

    /// <summary>
    /// 移除一组三消卡牌
    /// </summary>
    private void RemoveTriple(List<Card> triple)
    {
        //重置最近卡牌为空
        lastCard = null;
        
        foreach (var card in triple)
        {
            slotCards.Remove(card);
            card.Consume();
        }

        OnSlotChanged?.Invoke(slotCards);
        OnTripleMatched?.Invoke();

    }

    /// <summary>
    /// 检查游戏胜负
    /// </summary>
    private void CheckFail()
    {
        if (slotCards.Count >= maxSlotCount)
        {
            OnSlotOverflow?.Invoke();
            GameManager.Instance.ChangeState(GameState.Lose);
        }
    }

    private void CheckWin()
    {
        if (CardRegistry.Instance.AllCards.Count == 0)
        {
            GameManager.Instance.LoadNextLevel();
        }
    }

    /// <summary>
    /// 取出当前所有槽位卡牌（不销毁）
    /// </summary>
    public List<Card> PopAllCards()
    {
        List<Card> cards = new List<Card>(slotCards);

        foreach (var card in cards)
        {
            card.Data.isRemoved = false;
        }
            
        //重置最近卡牌为空
        lastCard = null;
        slotCards.Clear();

        OnSlotChanged?.Invoke(slotCards);

        return cards;
    }

    /// <summary>
    /// 撤回最近一张牌
    /// </summary>
    public Card WithdrawCard()
    {
        if (lastCard == null)
            return null;
    
        Card card = lastCard;
    
        card.Data.isRemoved = false;
    
        slotCards.Remove(card);
    
        OnSlotChanged?.Invoke(slotCards);
    
        // 清空记录
        lastCard = null;
    
        return card;
    }

    #endregion

    #region Utility

    public void ClearAll()
    {
        foreach (var card in slotCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        slotCards.Clear();
        OnSlotChanged?.Invoke(slotCards);
    }

    #endregion
}
