using System;
using System.Collections.Generic;
using UnityEngine;

public class CardStackChecker : Singleton<CardStackChecker>
{
    [Range(0f, 1f)]
    public float overlapThreshold;

    private List<Card> allCards => CardRegistry.Instance.AllCards;
    
    
    private void Start()
    {
        if(SlotManager.Instance != null)
            SlotManager.Instance.OnSlotChanged += OnSlotChangedHandler;
        
        RefreshAllCardStates();
    }

    private void OnDisable()
    {
        if(SlotManager.Instance != null)
            SlotManager.Instance.OnSlotChanged -= OnSlotChangedHandler;
    }
    
    private void OnSlotChangedHandler(IReadOnlyList<Card> cards)
    {
        RefreshAllCardStates();
    }
    //刷新所有卡的可选状态
    public void RefreshAllCardStates()
    {
        Debug.Log("刷新所有卡的可选状态");
        foreach (var card in allCards)
        {
            if (card == null)
                continue;

            if (!card.gameObject.activeInHierarchy)
                continue;
            
            bool selectable = CanSelect(card);
            card.SetSelectable(selectable);
        }
    }
    
    public void TrySelect(Card target)
    {
        if (!target.isSelectable)
            return;

        SlotManager.Instance.AddCard(target);
    }

    public bool CanSelect(Card target)
    {
        if (target == null || target.Data == null)
            return false;

        if (target.Data.isRemoved)
            return false;
            
        foreach (var other in allCards)
        {
            if (other == target)
                continue;

            if (other ==null)
                continue;
            
            if (other.Data.spawnData.layer <= target.Data.spawnData.layer)
                continue;

            if (other.Data.isRemoved)
                continue;
            
            if (IsOverlapping(other.Collider, target.Collider))
            {
                //Debug.Log("can't select card");
                return false;
            }
        }

        return true;
    }

    private bool IsOverlapping(BoxCollider2D upper, BoxCollider2D lower)
    {
        Bounds a = upper.bounds;
        Bounds b = lower.bounds;

        if (!a.Intersects(b))
            return false;

        if (overlapThreshold <= 0f)
            return true;

        float overlapArea = GetOverlapArea(a, b);
        float lowerArea = b.size.x * b.size.y;

        return overlapArea / lowerArea >= overlapThreshold;
    }

    private float GetOverlapArea(Bounds a, Bounds b)
    {
        float x = Mathf.Max(0, Mathf.Min(a.max.x, b.max.x) - Mathf.Max(a.min.x, b.min.x));
        float y = Mathf.Max(0, Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y));
        return x * y;
    }
}