using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public float countdownTime = 5f;          // 初始倒计时长
    public bool autoStart = false;          // 执行完后是否自动重启计时
    public bool autoRestart = false;          // 执行完后是否自动重启计时
    public UnityEvent onTimerStart;
    public UnityEvent onTimerReset;
    public UnityEvent onTimerComplete;        // 倒计时完成时会触发的事件

    public float currentTime;                // 当前剩余时间
    private bool isRunning = false;           // 是否在计时中
    private SceneManager sceneManager;

    /// <summary>
    /// 启动计时器
    /// </summary>
    public void StartTimer()
    {
        onTimerStart.Invoke();
        currentTime = countdownTime;
        isRunning = true;
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// 重置计时器（立即将倒计时设为初始时长，但不自动开始）
    /// </summary>
    public void Reset()
    {
        // currentTime = countdownTime;
        onTimerReset.Invoke();
        StartTimer();
    }

    void Start()
    {
        if (autoStart)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (!isRunning) return;

        // 逐帧减少当前时间
        currentTime -= Time.deltaTime;

        // 若计时结束
        if (currentTime <= 0f)
        {
            // 保证不会在调用事件前出现负数
            currentTime = 0f;
            
            // 触发倒计时完成事件
            onTimerComplete?.Invoke();

            if (autoRestart)
            {
                // 自动重新开始计时
                StartTimer();
            }
            else
            {
                // 停止计时
                StopTimer();
            }
        }
    }
}