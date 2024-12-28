using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间，用于操作Slider

public class ProgressSlider : MonoBehaviour
{
    public Slider slider; // 用于控制进度的Slider
    public float progressSpeed = 0.2f; // 每秒进度增长速度

    private float progress = 0f;
    private float resetInterval = 5f; // 重置时间间隔
    private float timer = 0f;

    void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider is not assigned! Please assign a UI Slider to the script.");
        }
    }

    void Update()
    {
        if (slider != null)
        {
            timer += Time.deltaTime;

            if (timer >= resetInterval)
            {
                Reset();
            }

            // 增加进度
            progress += progressSpeed * Time.deltaTime;

            // 确保进度不超过1
            progress = Mathf.Clamp01(progress);

            // 更新Slider值
            slider.value = progress;
        }
    }

    public void Reset()
    {
        // 重置进度
        progress = 0f;
        timer = 0f;
    }
}