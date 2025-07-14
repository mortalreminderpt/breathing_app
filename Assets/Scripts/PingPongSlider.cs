using UnityEngine;
using UnityEngine.UI;

public class PingPongSlider : MonoBehaviour
{
    public Slider slider;          // 进度条
    public float halfCycle = 5f;   // 升或降所用时间（秒）

    void Update()
    {
        if (slider == null) return;

        // Mathf.PingPong 会在 0→halfCycle→0 间往返，除以 halfCycle 得到 0-1 区间
        slider.value = Mathf.PingPong(Time.time, halfCycle) / halfCycle;
    }
}