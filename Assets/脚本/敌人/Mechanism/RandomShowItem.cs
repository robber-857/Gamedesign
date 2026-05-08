using UnityEngine;

public class RandomShowItem : MonoBehaviour
{
    [Header("显示设置")]
    [Tooltip("物品初始是否显示")]
    public bool isShowAtStart = false;
    [Tooltip("最小显示间隔时间（秒）")]
    public float minInterval = 5f;
    [Tooltip("最大显示间隔时间（秒）")]
    public float maxInterval = 10f;
    [Tooltip("最小显示持续时间（秒）")]
    public float minShowTime = 2f;
    [Tooltip("最大显示持续时间（秒）")]
    public float maxShowTime = 5f;

    public GameObject item;
    private float _timer;           // 计时器
    private float _currentInterval; // 当前间隔时间
    private float _currentShowTime; // 当前显示时间
    private bool _isShowing;        // 是否正在显示

    private void Start()
    {
        // 初始化物品显示状态
        item.SetActive(isShowAtStart);
        _isShowing = isShowAtStart;

        // 随机生成第一次间隔时间
        if (!_isShowing)
        {
            _currentInterval = Random.Range(minInterval, maxInterval);
            _timer = 0;
        }
        else
        {
            _currentShowTime = Random.Range(minShowTime, maxShowTime);
            _timer = 0;
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (!_isShowing)
        {
            // 不在显示状态，计时到达间隔时间则显示
            if (_timer >= _currentInterval)
            {
                ShowItem();
            }
        }
        else
        {
            // 在显示状态，计时到达显示时间则隐藏
            if (_timer >= _currentShowTime)
            {
                HideItem();
            }
        }
    }

    /// <summary>
    /// 显示物品
    /// </summary>
    private void ShowItem()
    {
        item.SetActive(true);
        _isShowing = true;
        _timer = 0;
        // 随机生成本次显示持续时间
        _currentShowTime = Random.Range(minShowTime, maxShowTime);
    }

    /// <summary>
    /// 隐藏物品
    /// </summary>
    private void HideItem()
    {
        item.SetActive(false);
        _isShowing = false;
        _timer = 0;
        // 随机生成下次显示间隔时间
        _currentInterval = Random.Range(minInterval, maxInterval);
    }

    /// <summary>
    /// 手动触发显示（外部调用）
    /// </summary>
    public void ForceShow()
    {
        ShowItem();
    }

    /// <summary>
    /// 手动触发隐藏（外部调用）
    /// </summary>
    public void ForceHide()
    {
        HideItem();
    }
}