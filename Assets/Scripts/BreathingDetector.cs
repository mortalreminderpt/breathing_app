using System.Collections.Generic;
using UnityEngine;

public class BreathingDetector : MonoBehaviour
{
    // 公共变量
    public float distance = 0f;                 // 当前距离（由外部更新）
    public float movementThreshold = 0.1f;      // 呼吸动作阈值
    public float minBreathTime = 5f;           // 呼吸过快阈值
    public float maxBreathTime = 30f;          // 呼吸过慢阈值
    public int maxBreaths = 3;                 // 记录最近 X 次呼吸
    public float sampleRate = 10f;              // HZ
    private float sampleInterval;
    public GameObject Mushroom;

    // 呼吸状态
    public enum BreathingState { Inhaling, Exhaling, Holding }
    private BreathingState currentState = BreathingState.Holding;

    // 采样相关
    private float prevDistance = 0f;
    private bool isIncreasing = false;

    // 呼吸计时
    private float breathStartTime = 0f;        
    private List<float> breathTimestamps = new List<float>();
    private bool lastBreathStable = true;
    private int totalBreathing = 0;

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
    public int GetBreathingCount() => totalBreathing;

    // 重置呼吸总次数
    public void ResetBreathingCount() => totalBreathing = 0;

    // ======= Unity 入口 =======
    void Start()
    {
        prevDistance = distance;
        breathStartTime = Time.time;
        sampleInterval = 1f / sampleRate;;
    }

    void Update()
    {
        sampleInterval -= Time.deltaTime;
        if (sampleInterval > 0f)
        {
            return;
        }
        sampleInterval = 1f / sampleRate;
        float deltaDistance = distance - prevDistance;
        prevDistance = distance;

        // 根据距离变化判断呼吸方向
        if (deltaDistance > movementThreshold && !isIncreasing)
        {
            isIncreasing = true;
            OnBreathPhaseChange(BreathingState.Inhaling);
        }
        else if (deltaDistance < -movementThreshold && isIncreasing)
        {
            isIncreasing = false;
            OnBreathPhaseChange(BreathingState.Exhaling);
        }
    }

    // ======= 内部逻辑 =======
    private void OnBreathPhaseChange(BreathingState newState)
    {
        float phaseTime = Time.time - breathStartTime;
        
        // 检测上一个阶段（Inhaling或Exhaling或Holding）的时长是否稳定
        CheckStability(phaseTime);

        // 如果本次是从 Inhaling 切到 Exhaling，则表明一次完整呼吸结束
        if (newState == BreathingState.Exhaling)
        {
            breathTimestamps.Add(Time.time);
            if (breathTimestamps.Count > maxBreaths)
                breathTimestamps.RemoveAt(0);

            totalBreathing++;
        }

        currentState = newState;
        breathStartTime = Time.time; // 重置计时
    }

    private void CheckStability(float duration)
    {
        if (duration < minBreathTime)
        {
            lastBreathStable = false;
            ToFast();
        }
        else if (duration > maxBreathTime)
        {
            lastBreathStable = false;
            ToSlow();
        }
        else
        {
            lastBreathStable = true;
        }
    }

    private void ToSlow()
    {
        float scale = Mushroom.GetComponent<MushroomController>().GetTargetScale();
        Mushroom.GetComponent<MushroomController>().SetTargetScale(scale - 0.5f);
    }

    private void ToFast()
    {
        float scale = Mushroom.GetComponent<MushroomController>().GetTargetScale();
        Mushroom.GetComponent<MushroomController>().SetTargetScale(scale + 0.5f);
    }
}
