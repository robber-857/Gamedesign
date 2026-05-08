using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[RequireComponent(typeof(Text))]
public class TextFadeEffect : MonoBehaviour
{
    public Text textComponent;
    private string fullText;
      
    public float showDuration = 1f; // 淡入总时长
    public float hideDuration = 1f; // 淡出总时长

    
    // 淡入完成时触发的事件
    public  Action OnFadeInComplete;
    // 淡出完成时触发的事件
    public  Action OnFadeOutComplete;

    private void Awake()
    {
      //  textComponent = GetComponent<Text>();
        fullText = textComponent.text;
        // 初始状态：完全透明
        SetTextAlpha(0f);
    }

    private void Start()
    {
        
    }
    /// <summary>
    /// 开始淡入（渐进显示）
    /// </summary>
    public void StartFadeIn()
    {
        StopAllCoroutines();
       // textComponent.text = fullText; // 确保文本完整
        StartCoroutine(FadeInCoroutine());
    }

    /// <summary>
    /// 开始淡出（渐进隐藏）
    /// </summary>
    public void StartFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine());
    }

    /// <summary>
    /// 直接设置文本透明度（辅助方法）
    /// </summary>
    private void SetTextAlpha(float alpha)
    {
        Color currentColor = textComponent.color;
        textComponent.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }

    /// <summary>
    /// 淡入协程（带事件触发）
    /// </summary>
    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < showDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / showDuration);
            SetTextAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 确保最终完全显示
        SetTextAlpha(1f);
        // 触发淡入完成事件
        OnFadeInComplete?.Invoke();
    }

    /// <summary>
    /// 淡出协程（带事件触发）
    /// </summary>
    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < hideDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / hideDuration);
            SetTextAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 确保最终完全隐藏
        SetTextAlpha(0f);
        // 触发淡出完成事件
        OnFadeOutComplete?.Invoke();
    }

    // 以下是可选的编辑器快捷绑定方法（可通过UI按钮直接调用）
    /// <summary>
    /// 淡入后自动淡出（用于测试或连续动画）
    /// </summary>
    public void FadeInThenOut()
    {
        StartFadeIn();
        OnFadeInComplete += () =>
        {
            // 淡入完成后等待2秒再淡出（可自定义延迟时间）
            StartCoroutine(WaitThenFadeOut(2f));
        };
    }

    private IEnumerator WaitThenFadeOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartFadeOut();
    }
}