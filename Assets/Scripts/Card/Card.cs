
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    public CardData Data { get; private set; }
    public bool isSelectable { get; private set; }
    public BoxCollider2D Collider { get; private set; }

    private SpriteRenderer spriteRenderer;
    
    [Header("反馈")]
    [SerializeField] private GameObject exploreEffect;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<BoxCollider2D>();
    }

    public void Init(CardData data)
    {
        Data = data;
        spriteRenderer.sprite = data.config.icon;
    }

    public void SetPosition(Vector3 position)
    {
        Data.spawnData.position = position;
    }
    
    public void SetLayer(int layerIndex)
    {
        Data.spawnData.layer = layerIndex;
        
        SpriteRenderer[] renderers =
            GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var sr in renderers)
        {
            sr.sortingOrder = layerIndex;
        }
    }

    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        if (isSelectable || Data.isRemoved)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.gray;
        }
    }
    
    //卡牌移动动画+委托调用三消
    private Coroutine moveCoroutine;

    public void MoveTo(Vector3 targetPos, float duration, System.Action onComplete)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveRoutine(targetPos, duration, onComplete));
    }

    private IEnumerator MoveRoutine(Vector3 target, float duration, System.Action onComplete)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            if (this == null) yield break;

            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        

        onComplete?.Invoke(); // ⭐ 动画完成回调
    }

    public void Consume()
    {
        CardRegistry.Instance.Unregister(this);
        
        if (SandboxManager.Instance != null &&
            SandboxManager.Instance.IsInSandbox)
        {
            SandboxManager.Instance.RecordConsume(this);
            gameObject.SetActive(false);
            return;
        }
        Feedback();
        
        Destroy(gameObject);
    }

    public void Feedback()
    {
        //爆炸特效生成
        Instantiate(exploreEffect, transform.position, Quaternion.identity);
    }
}