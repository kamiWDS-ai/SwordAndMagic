using UnityEngine;

public class CardFactory : Singleton<CardFactory>
{
    [SerializeField] private Transform cardRoot;
    [SerializeField] private Card cardPrefab;

    protected override void Awake()
    {
        base.Awake();
        if (cardRoot == null)
        {
            GameObject root = new GameObject("CardRoot");
            cardRoot = root.transform;
        }
    }
    public Card Create(CardData data,CardSpawnData spawnData)
    {
        Card card = Instantiate(
            cardPrefab,
            spawnData.position,
            Quaternion.identity,
            cardRoot
        );

        card.Init(data);
        card.SetPosition(spawnData.position);
        card.SetLayer(spawnData.layer);
        CardRegistry.Instance.Register(card);

        return card;
    }
}