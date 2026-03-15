using System;
using System.Collections.Generic;
using UnityEngine;

public class CardRegistry : Singleton<CardRegistry>
{
    public readonly List<Card> AllCards = new();
    public event Action<List<Card>> OnCardsChanged;

    public void Register(Card card)
    {
        AllCards.Add(card);
        OnCardsChanged?.Invoke(AllCards);
    }

    public void Unregister(Card card)
    {
        AllCards.Remove(card);
        OnCardsChanged?.Invoke(AllCards);
    }
}