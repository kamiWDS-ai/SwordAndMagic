using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_simple : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private int maxHealth;
    private int currentHealth;

    private Coroutine smoothCoroutine;

    private void Start()
    {
        if (CardRegistry.Instance != null)
            maxHealth = CardRegistry.Instance.AllCards.Count;

        currentHealth = maxHealth;

        LevelChangedUpdateHealth();

        if (SlotManager.Instance != null)
            SlotManager.Instance.OnTripleMatched += HealthBarUpdate;
        if (GameManager.Instance != null)
            GameManager.Instance.OnLevelChanged += LevelChangedUpdateHealth;
        
    }

    private void OnDisable()
    {
        if (SlotManager.Instance != null)
            SlotManager.Instance.OnTripleMatched -= HealthBarUpdate;
        if (GameManager.Instance != null)
            GameManager.Instance.OnLevelChanged -= LevelChangedUpdateHealth;
        
    }

    // ⭐ 三消事件回调更新血量
    private void HealthBarUpdate()
    {
        if(SandboxManager.Instance != null && SandboxManager.Instance.IsInSandbox)
            return;
        currentHealth -= 3;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (smoothCoroutine != null)
            StopCoroutine(smoothCoroutine);

        smoothCoroutine = StartCoroutine(SmoothUpdate());
    }

    private IEnumerator SmoothUpdate()
    {
        float duration = 0.3f;
        float time = 0f;

        float start = fillImage.fillAmount;
        float target = (float)currentHealth / maxHealth;

        while (time < duration)
        {
            time += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        fillImage.fillAmount = target;
    }
    
    //关卡转换刷新血量上限
    private void LevelChangedUpdateHealth()
    {
        StartCoroutine(UpdateBarForMaxHealth());
    }
    
    private IEnumerator UpdateBarForMaxHealth()
    {
        Debug.Log("关卡转换");
        fillImage.fillAmount = 1f;
        
        yield return new WaitForSeconds(0.1f);
        
        if (CardRegistry.Instance != null)
            maxHealth = CardRegistry.Instance.AllCards.Count;
        
        currentHealth = maxHealth;
        
        float percent = (float)currentHealth / maxHealth;
        fillImage.fillAmount = percent;
    }

    
}
