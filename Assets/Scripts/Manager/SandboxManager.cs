using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SandboxManager : Singleton<SandboxManager>
{
    public bool IsInSandbox { get; private set; }

    [Header("步数限制")]
    [SerializeField] private int maxSteps = 5;
    private int currentSteps;

    //沙盒记录数据
    private SandboxRecord record;
    
    //事件
    public event Action onFinish;
    
    [Header("UI控制")]
    public TMP_Text stepText;
    public GameObject SandboxButton;
    public GameObject ExitButton;
    public GameObject SandboxUI;
    public GameObject RemoveButton;
    public GameObject WithDrawButton;
    public GameObject ShuffleButton;
    
    private void Start()
    {
        currentSteps = 0;
        IsInSandbox = false;
        stepText.text = "剩余步数: " + maxSteps;
        ExitButton.SetActive(false);
        UIController(IsInSandbox);
    }
    #region ===== 沙盘控制 =====
    public void EnterSandbox()
    {
        if (IsInSandbox)
            return;

        if (CardRegistry.Instance.AllCards.Count < maxSteps)
            return;
        
        Debug.Log("进入沙盘模式");

        IsInSandbox = true;
        currentSteps = 0;

        record = new SandboxRecord();

        // 记录初始槽位卡牌
        record.initialSlotCards.AddRange(
            SlotManager.Instance.SlotCards
        );
        
        UIController(IsInSandbox);
    }

    public void ExitSandbox()
    {
        if (!IsInSandbox)
            return;

        Debug.Log("退出沙盘模式");
        
        IsInSandbox = false;

        StartCoroutine(DelayedRollback());
        
        UIController(IsInSandbox);
        
        onFinish?.Invoke();
    }

    private IEnumerator DelayedRollback()
    {
        yield return null; // 等一帧，不然最后一步会卡bug

        Rollback();
        record = null;
        
        yield return new WaitForSeconds(0.5f);
        
        //回滚完重新刷新一下状态
        CardStackChecker.Instance.RefreshAllCardStates();
    }
    #endregion

    #region ===== 行为记录 =====

    public void RecordSelect(Card card)
    {
        if (!IsInSandbox) return;

        if (!record.selectedCards.Contains(card))
            record.selectedCards.Add(card);
        
    }

    public void RecordConsume(Card card)
    {
        if (!IsInSandbox) return;

        if (!record.consumedCards.Contains(card))
            record.consumedCards.Add(card);
    }

    #endregion

    #region ===== 回滚逻辑 =====

    private void Rollback()
    {
        Debug.Log("开始回滚沙盘");

        var slotManager = SlotManager.Instance;

        // 1️⃣ 恢复所有被三消卡
        foreach (var card in record.consumedCards)
        {
            if (card == null) continue;

            card.gameObject.SetActive(true);
            CardRegistry.Instance.Register(card);
            card.Data.isRemoved = false;
            card.transform.position = card.Data.spawnData.position;
        }

        // 2️⃣ 清空当前槽位（不销毁）
        var current = slotManager.PopAllCards();

        foreach (var card in current)
        {
            if (card == null) continue;

            card.Data.isRemoved = false;
            card.transform.position = card.Data.spawnData.position;
        }
        

        // 3️⃣ 重建初始槽位
        foreach (var card in record.initialSlotCards)
        {
            if (card == null) continue;

            slotManager.AddCard(card);
        }

        Debug.Log("沙盘回滚完成");
    }

    #endregion

    //===== 步数限制 =====
    public void StepUpdate(int step)
    {
        currentSteps += step;
        stepText.text = "剩余步数: " + (maxSteps-currentSteps);
    }
    
    public bool StepCheck()
    {
        if (currentSteps >= maxSteps)
        {
            Debug.Log("达到沙盘步数限制");
            StartCoroutine(AwakeExitButton());
            return false;
        }

        return true;
    }
    
    //===== UI控制 =====
    public IEnumerator AwakeExitButton()
    {
        yield return new WaitForSeconds(0.5f);
        if(ExitButton != null && !ExitButton.activeInHierarchy)
            ExitButton.SetActive(true);
    }
    public void UIController(bool isSandbox)
    {
        SandboxButton.SetActive(!isSandbox);
        SandboxUI.SetActive(isSandbox);
        RemoveButton.SetActive(!isSandbox);
        WithDrawButton.SetActive(!isSandbox);
        ShuffleButton.SetActive(!isSandbox);
    }
}