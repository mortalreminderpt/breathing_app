using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBreathingDetector : MonoBehaviour
{
    // 引用 BreathingDetector 组件
    public BreathingDetector breathingDetector;

    // 用于存储呼吸状态的名称
    private string breathingStateName = "Unknown";

    // 用于存储呼吸频率状态的描述
    private string breathingRateStatus = "Normal";

    // 更新频率（秒），避免每帧都输出
    public float updateInterval = 1f;
    private float timeSinceLastUpdate = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // 检查是否已经在 Inspector 中赋值
        if (breathingDetector == null)
        {
            // 尝试在当前对象上查找 BreathingDetector 组件
            breathingDetector = GetComponent<BreathingDetector>();

            if (breathingDetector == null)
            {
                Debug.LogError("BreathingDetector 组件未找到。请确保将 BreathingDetector 组件赋值给 TestBreathingDetector。");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (breathingDetector == null)
            return;

        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            timeSinceLastUpdate = 0f;
            OutputBreathingMetrics();
        }
    }

    // 输出呼吸指标到控制台
    private void OutputBreathingMetrics()
    {
        // 获取当前呼吸状态
        BreathingDetector.BreathingState currentState = breathingDetector.GetCurrentBreathingState();
        breathingStateName = currentState.ToString();

        // 获取平均呼吸频率
        float avgBreathRate = breathingDetector.GetAverageBreathRate();

        // 检查呼吸频率状态
        // int rateStatus = breathingDetector.CheckBreathingRate();
        // switch (rateStatus)
        // {
        //     case -1:
        //         breathingRateStatus = "Too Slow";
        //         break;
        //     case 1:
        //         breathingRateStatus = "Too Fast";
        //         break;
        //     default:
        //         breathingRateStatus = "Normal";
        //         break;
        // }

        // 获取采样时间
        // float sampleTime = breathingDetector.sampleTime;

        // 输出信息
        // Debug.Log($"[Breathing Metrics] State: {breathingStateName}, Avg Rate: {avgBreathRate:F2} BPM, Rate Status: {breathingRateStatus}, Sample Time: {sampleTime:F2}s");
    }
}
