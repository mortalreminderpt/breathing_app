using System.Collections;
using System.Collections.Generic;
using PolyverseSkiesAsset;
using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public PolyverseSkies polyverseSkies;
    public List<Light> dayLights;

    private float targetTimeOfDay;
    private float transitionSpeed = 0.1f; // 控制过渡速度

    void Start()
    {
        Reset();
    }

    void Update()
    {
        // 平滑过渡到目标值
        if (Mathf.Abs(polyverseSkies.timeOfDay - targetTimeOfDay) > 0.01f)
        {
            polyverseSkies.timeOfDay = Mathf.Lerp(polyverseSkies.timeOfDay, targetTimeOfDay, transitionSpeed * Time.deltaTime);
        }
        else
        {
            polyverseSkies.timeOfDay = targetTimeOfDay; // 确保精度足够时直接设置为目标值
        }
    }

    public void Reset()
    {
        ToNight();
    }

    public void ToDay()
    {
        foreach (Light dayLight in dayLights)
        {
            dayLight.enabled = true;
        }
        targetTimeOfDay = 0; // 设置目标值为白天
    }

    public void ToNight()
    {
        foreach (Light dayLight in dayLights)
        {
            dayLight.enabled = false;
        }
        targetTimeOfDay = 1; // 设置目标值为夜晚
    }
}