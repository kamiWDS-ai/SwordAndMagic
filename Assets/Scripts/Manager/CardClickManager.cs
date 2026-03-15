using UnityEngine;

public class CardClickManager : Singleton<CardClickManager>
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryClick();
        }
    }

    private void TryClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        if (hits.Length == 0)
            return;

        Card topCard = null;
        int highestOrder = int.MinValue;

        foreach (var hit in hits)
        {
            Card card = hit.collider.GetComponent<Card>();
            if (card == null)
                continue;

            int order = card.GetComponent<SpriteRenderer>().sortingOrder;

            if (order > highestOrder)
            {
                highestOrder = order;
                topCard = card;
            }
        }

        if (topCard != null)
        {
            CardStackChecker.Instance.TrySelect(topCard);
        }
    }
}