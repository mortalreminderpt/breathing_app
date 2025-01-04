using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BreathingDetector : MonoBehaviour
{
    // 公共变量
    public float Distance = 0f;                 // 当前距离（由外部更新）
    public float MovementThreshold = 0.1f;      // 呼吸动作阈值
    public float MinBreathTime = 5f;            // 呼吸过快阈值
    public float MaxBreathTime = 30f;           // 呼吸过慢阈值
    public int MaxBreaths = 3;                  // 记录最近 X 次呼吸
    public int UnstableTolerance = 5;
    
    public float ProtectionDuration = 5f;
    public int unstableCount = 0;
    private float protectionTime = 0f; // = unstableCount * ProtectionDuration
    
    public int unstableToleranceCount = 0;
    // public float SampleRate = 10f;              // 采样频率（Hz）

    // 呼吸状态
    public enum BreathingState { Inhaling, Exhaling, Holding }
    public BreathingState currentState = BreathingState.Holding;

    // 采样相关
    private float prevDistance = 0f;
    // private bool isIncreasing = false;
    // filter
    public bool enableFilter = true;
    public float filteredValue = 0f;

    // 调节速率参数
    public float alpha = 0.01f; // 更新背景的学习率，越小越慢
    public float filterThreshold = 0.5f; // 干扰检测的阈值，依据实际情况调整
    // 滤波变量
    private float background = 0f; // 背景基线

    // 呼吸计时
    private float breathStartTime = 0f;        
    public List<float> breathTimestamps = new List<float>();
    private bool lastBreathStable = true;
    private int totalBreaths = 0;
    
    public UnityEvent OnBreathingTooSlow;
    public UnityEvent OnBreathingTooFast;
    public UnityEvent OnUnstableRegular;
    public UnityEvent OnUnstableTolerance;
    
    // ======= 对外接口 =======

    // 获取当前呼吸状态
    public BreathingState GetCurrentBreathingState() => currentState;

    // 获取平均呼吸频率（呼吸/分钟）
    public float GetAverageBreathRate()
    {
        if (breathTimestamps.Count < 2) return 0f;
        float totalTime = breathTimestamps[breathTimestamps.Count - 1] - breathTimestamps[0];
        return (breathTimestamps.Count - 1) / totalTime * 60f;
    }

    // 获取上一次呼吸是否稳定
    public bool GetLastBreathStability() => lastBreathStable;

    // 获取呼吸总次数
    public int GetBreathingCount() => totalBreaths;

    // 重置呼吸总次数
    public void ResetBreathingCount() => totalBreaths = 0;

    public void Reset()
    {
        prevDistance = Distance;
        breathStartTime = Time.time;
        unstableToleranceCount = 0;
        currentState=BreathingState.Holding;
    }

    // ======= Unity 入口 =======
    void Start()
    {
        Reset();
    }

    void Update()
    {                
        protectionTime -= Time.deltaTime;
        float deltaDistance = Distance; // - prevDistance;
        filteredValue = deltaDistance;
        if (enableFilter)
        {
            // 自适应背景减法
            float delta = Mathf.Abs(deltaDistance - background);
            if (delta < filterThreshold)
            {
                // 更新背景值（平滑处理）
                background = Mathf.Lerp(background, deltaDistance, alpha);
            }

            // 计算滤波后的值
            filteredValue = deltaDistance - background;
        }

        // 根据距离变化判断呼吸方向
        // if (deltaDistance > MovementThreshold && !isIncreasing)
        if (filteredValue > MovementThreshold && currentState != BreathingState.Inhaling)
        {
            // isIncreasing = true;
            OnBreathPhaseChange(BreathingState.Inhaling);
            prevDistance = Distance;
        }
        // else if (deltaDistance < -MovementThreshold && isIncreasing)
        else if (filteredValue < -MovementThreshold && currentState != BreathingState.Exhaling)
        {
            // isIncreasing = false;
            OnBreathPhaseChange(BreathingState.Exhaling);
            prevDistance = Distance;
        }
    }

    // ======= 内部逻辑 =======
    private void OnBreathPhaseChange(BreathingState newState)
    {
        float phaseTime = Time.time - breathStartTime;
        
        // 检测上一个阶段（Inhaling或Exhaling或Holding）的时长是否稳定
        CheckStability(phaseTime);

        breathTimestamps.Add(phaseTime);
        if (breathTimestamps.Count > MaxBreaths)
        {
            breathTimestamps.RemoveAt(0);
        }

        // 如果本次是从 Inhaling 切到 Exhaling，则表明一次完整呼吸结束
        if (newState == BreathingState.Exhaling)
        {
            totalBreaths++;
        }

        currentState = newState;
        breathStartTime = Time.time; // 重置计时
    }

    private void CheckStability(float duration)
    {
        if (currentState == BreathingState.Holding)
        {
            lastBreathStable = true;
        }
        else if (duration >= MinBreathTime && duration <= MaxBreathTime)
        {
            lastBreathStable = true;
        }
        else
        {
            if (protectionTime > 0)
            {
                return;
            }
            protectionTime = ProtectionDuration;
            unstableToleranceCount += 1;
            if (unstableToleranceCount >= UnstableTolerance)
            {
                unstableCount += 1;
                protectionTime = unstableCount * ProtectionDuration;
                OnUnstableTolerance.Invoke();
                // Mushroom.GetComponent<MushroomController>().Reset();
                // Tree.GetComponent<DropController>().Reset();
                // Reset();
            }
            else
            {
                OnUnstableRegular.Invoke();
            }
            if (duration < MinBreathTime)
            {
                lastBreathStable = false;
                OnBreathingTooFast.Invoke();
            }
            else if (duration > MaxBreathTime)
            {
                lastBreathStable = false;
                OnBreathingTooSlow.Invoke();
            }
        }
    }
}
